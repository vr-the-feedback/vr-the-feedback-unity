using UnityEngine;
using System.Collections;
using System.IO;
using LitJson;
using System.Net;
using System.Threading;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PresignedResponseJSON
{
    public string url;
}

public class VRTheFeedbackManager : MonoBehaviour
{

	public string PROJECT_KEY = "REPLACE_ME";

    public struct VRTheFeedbackEventArgs
    {
        public string message;
    }

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="sender">this object</param>
    /// <param name="e"><see cref="InteractableObjectEventArgs"/></param>
    public delegate void VRTheFeedbackEventArgsEventHandler(object sender, VRTheFeedbackEventArgs e);

    private AudioSource myAudio;

    string fileName;
    string filePath;
    bool _threadRunning;
    Thread _thread;
    PresignedResponseJSON json;
    private SavWav saveWav;
    private float[] justFeedbackSamples;
    public AudioClip justFeedback;
    private bool _threadSuccessfulNotificationPending = false;
    private bool _threadErrorNotificationPending = false;

    public event VRTheFeedbackEventArgsEventHandler FeedbackSuccessfullyUploaded;
    public event VRTheFeedbackEventArgsEventHandler FeedbackFailedDueToError;

    public bool isRecording = false;

    public virtual void OnFeedbackSuccessfullyUploaded(VRTheFeedbackEventArgs e)
    {
        if (FeedbackSuccessfullyUploaded != null)
        {
            FeedbackSuccessfullyUploaded(this, e);
        }
    }

    public virtual void OnFeedbackFailedDueToError(VRTheFeedbackEventArgs e)
    {
        if (FeedbackFailedDueToError != null)
        {
            FeedbackFailedDueToError(this, e);
        }
    }

    public void Update()
    {
        if (_threadSuccessfulNotificationPending)
        {
            _threadSuccessfulNotificationPending = false;
            OnFeedbackSuccessfullyUploaded(new VRTheFeedbackEventArgs());
        }
        else if (_threadErrorNotificationPending)
        {
            OnFeedbackFailedDueToError(new VRTheFeedbackEventArgs());
            _threadErrorNotificationPending = false;
        }
    }

    public bool MyRemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        bool isOk = true;
        // If there are errors in the certificate chain, look at each error to determine the cause.
        if (sslPolicyErrors != SslPolicyErrors.None)
        {
            for (int i = 0; i < chain.ChainStatus.Length; i++)
            {
                if (chain.ChainStatus[i].Status != X509ChainStatusFlags.RevocationStatusUnknown)
                {
                    chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                    chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                    chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                    chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                    bool chainIsValid = chain.Build((X509Certificate2)certificate);
                    if (!chainIsValid)
                    {
                        isOk = false;
                    }
                }
            }
        }
        return isOk;
    }

    void Start()
    {
        ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
        myAudio = GetComponent<AudioSource>();
        saveWav = new SavWav();
    }

    public void RecordFeedback()
    {
        isRecording = true;
        myAudio.clip = Microphone.Start(null, false, 300, 44100);
    }

    public void SaveFeedback()
    {   
        SaveFeedback (new Dictionary<string, string> ());   
    }

	public void SaveFeedback(Dictionary<string, string> metadata) {
		int lastTime = Microphone.GetPosition(null);
		if (lastTime == 0) return;
		Microphone.End(null);
        isRecording = false;
        float[] samples = new float[myAudio.clip.samples];
		myAudio.clip.GetData(samples, 0);
		float[] ClipSamples = new float[lastTime];
		Array.Copy(samples, ClipSamples, ClipSamples.Length - 1);
		AudioClip newClip = AudioClip.Create("playRecordClip", ClipSamples.Length, 1, 44100, false, false);
		newClip.SetData(ClipSamples, 0);

		AudioClip.Destroy(myAudio.clip);
		myAudio.clip = newClip;

		filePath = Path.Combine(Application.persistentDataPath, "test.mp3");
		justFeedback = saveWav.TrimSilence(myAudio.clip, 0.001f);
        if (justFeedback.length < 1)
        {
            OnFeedbackFailedDueToError(new VRTheFeedbackEventArgs { message = "Feedback too short." });
            return;
        }
        //saveWav.Save(filePath + ".wav", justFeedback);

        if (filePath != null)
		{
			StartCoroutine(UploadToServer(metadata));
		}
	}

	private IEnumerator UploadToServer(Dictionary<string, string> metadata)
    {
		if (PROJECT_KEY == "REPLACE_ME" || PROJECT_KEY == "CHANGE_ME") {
			Debug.Log ("You have to set a vaild PROJECT_KEY. Create account: http://www.vrthefeedback.com/");
			Debug.Log ("Creating MP3 that normally would be uploaded to server.");
			justFeedbackSamples = new float[justFeedback.samples * justFeedback.channels];
			justFeedback.GetData(justFeedbackSamples, 0);
			_thread = new Thread(Mp3Encoding);
			_thread.Start();
			return false;
		}
		string url = "https://www.vrthefeedback.com/upload/presign";

		metadata.Add("game-play-time", Time.time.ToString());
		metadata.Add("game-scene", SceneManager.GetActiveScene().name);
		metadata.Add("feedback-length", justFeedback.length.ToString());

		string metadataJson = JsonMapper.ToJson(metadata);
		string ourPostData = "{\"secret_key\": \""+ PROJECT_KEY +"\", \"metadata\": "+metadataJson+"}";
		System.Collections.Generic.Dictionary<string, string> headers = new System.Collections.Generic.Dictionary<string, string>();
		headers.Add("Content-Type", "application/json");
		byte[] pData = System.Text.Encoding.ASCII.GetBytes(ourPostData.ToCharArray());
		WWW www = new WWW(url, pData, headers);
        yield return www;
        if (www.error == null)
        {
            ParseResponseAndUploadToS3(www);
        }
        else
        {
            Debug.Log("ERROR - will retry in 3s: " + www.error);
            yield return new WaitForSeconds(3);
			www = new WWW(url, pData, headers);
            yield return www;
            if (www.error == null)
            {
                ParseResponseAndUploadToS3(www);
            } else
            {
                Debug.Log("ERROR - failed again...: " + www.error);
                OnFeedbackFailedDueToError(new VRTheFeedbackEventArgs());
            }
        }

    }

    private void ParseResponseAndUploadToS3(WWW www)
    {
        Debug.Log("Server response on presign: " + www.text);
        json = ParsePresignedResponseJSON(www.text);
        justFeedbackSamples = new float[justFeedback.samples * justFeedback.channels];
        justFeedback.GetData(justFeedbackSamples, 0);
        _thread = new Thread(FeedbackUploadThread);
        _thread.Start();
    }

	private void Mp3Encoding() {
		EncodeMP3.convert(justFeedbackSamples, filePath, 128);
		Debug.Log("Find mp3 file under: " + filePath);
	}

    private void FeedbackUploadThread()
    {
        _threadRunning = true;
        Debug.Log("Starting upload on separate thread.");
		Mp3Encoding();
        UploadObject(json.url, filePath);
        justFeedbackSamples = null;
        justFeedback = null;
        Debug.Log("Upload done..");
        _threadSuccessfulNotificationPending = true;
        _threadRunning = false;
    }

    private PresignedResponseJSON ParsePresignedResponseJSON(string jsonString)
    {
        JsonData jsonvale = JsonMapper.ToObject(jsonString);
        PresignedResponseJSON parsejson;
        parsejson = new PresignedResponseJSON();
        parsejson.url = jsonvale["url"].ToString();
        return parsejson;
    }

    private void UploadObject(string url, string filePath)
    {
        try
        {
            HttpWebRequest httpRequest = WebRequest.Create(url) as HttpWebRequest;
            httpRequest.Method = "PUT";
            using (Stream dataStream = httpRequest.GetRequestStream())
            {
                byte[] buffer = new byte[8000];
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    int bytesRead = 0;
                    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        dataStream.Write(buffer, 0, bytesRead);
                    }
                }
            }
            HttpWebResponse response = httpRequest.GetResponse() as HttpWebResponse;
            response.Close();
            Debug.Log("Response from server: " + response.StatusCode);
        }
        catch (System.Exception ex)
        {
            _threadErrorNotificationPending = true;
            Debug.LogError("something went wrong.. " + ex);
        }
    }


    void OnDisable()
    {
        if (_threadRunning)
        {
            Debug.Log("Upload still in progress - waiting to finish...");
            _threadRunning = false;
            _thread.Join();
        }
    }

}
