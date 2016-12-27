using UnityEngine;
using System.Collections;
using System.IO;
using LitJson;
using System.Net;
using System.Threading;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System;

public class PresignedResponseJSON
{
	public string url;
}

public class VRTheFeedbackManager : MonoBehaviour {

    private AudioSource myAudio;

	string fileName;
	string filePath;
	bool _threadRunning;
	Thread _thread;
	PresignedResponseJSON json;
	private SavWav saveWav;
	private float[] justFeedbackSamples;
	public AudioClip justFeedback;

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

    void Start () {
        ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
        myAudio = GetComponent<AudioSource>();
		saveWav = new SavWav ();
    }
	
    public void RecordFeedback()
    {
        myAudio.clip = Microphone.Start(null, false, 300, 44100);
    }

    public void SaveFeedback()
    {
        int lastTime = Microphone.GetPosition(null);
        if (lastTime == 0) return;
        Microphone.End (null);
        float[] samples = new float[myAudio.clip.samples];
        myAudio.clip.GetData(samples, 0);
        float[] ClipSamples = new float[lastTime];
        Array.Copy(samples, ClipSamples, ClipSamples.Length - 1);
        AudioClip newClip = AudioClip.Create("playRecordClip", ClipSamples.Length, 1, 44100, false, false);
        newClip.SetData(ClipSamples, 0);

        AudioClip.Destroy(myAudio.clip);
        myAudio.clip = newClip;

        filePath = Path.Combine (Application.persistentDataPath, "test.mp3");
		justFeedback = saveWav.TrimSilence (myAudio.clip, 0.001f);
        //saveWav.Save(filePath + ".wav", justFeedback);

		if (filePath != null) {
			StartCoroutine(UploadToServer ());
		}	
    }

	public IEnumerator UploadToServer() {
		string url = "https://www.vrthefeedback.com/upload/presign";
		WWW www = new WWW(url);
		yield return www;
		if (www.error == null)
		{
			Debug.Log ("Server response on presign: " + www.data);
			json = ParsePresignedResponseJSON(www.data);
			justFeedbackSamples = new float[justFeedback.samples * justFeedback.channels];
			justFeedback.GetData (justFeedbackSamples, 0);
			_thread = new Thread(FeedbackUploadThread);
			_thread.Start();	
		}
		else
		{
			Debug.Log("ERROR: " + www.error);
		}  

	}

	public void FeedbackUploadThread() {
		_threadRunning = true;
		Debug.Log("Starting upload on separate thread.");
		EncodeMP3.convert(justFeedbackSamples, filePath, 128);
        Debug.Log("Find mp3 file under: " + filePath);
        UploadObject (json.url, filePath);
		justFeedbackSamples = null;
		justFeedback = null;
		Debug.Log("Upload done..");
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

	static void UploadObject(string url, string filePath)
	{
        try {
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
        } catch (System.Exception ex)
        {
            Debug.LogError("something went wrong.. " + ex);
        }
    }


	void OnDisable()
	{
		if(_threadRunning)
		{
			Debug.Log("Upload still in progress - waiting to finish...");
			_threadRunning = false;
			_thread.Join();
		}
	}

}
