using UnityEngine;
using System.Collections;
using System.IO;
using LitJson;
using System.Net;

public class PresignedResponseJSON
{
	public string url;
}

public class VRTheFeedbackManager : MonoBehaviour {

    private AudioSource myAudio;

	// Use this for initialization
	void Start () {
        myAudio = GetComponent<AudioSource>();

    }
	
    public void RecordFeedback()
    {
        myAudio.clip = Microphone.Start(null, false, 10, 44100);
    }

    public void SaveFeedback()
    {
        string filePath = new SavWav().Save("test.wav", myAudio.clip);
		if (filePath != null) {
			
			Debug.Log ("would upload from " + filePath);
			StartCoroutine(UploadToServer (filePath));
		}			
    }

	public IEnumerator UploadToServer(string filepath) {
		string url = "https://vrthefeedback.com/upload/presign";
		Debug.Log ("ready to presign url");
		WWW www = new WWW(url);
		yield return www;
		if (www.error == null)
		{
			Debug.Log ("will parse response" + www.data);
			PresignedResponseJSON json = ParsePresignedResponseJSON(www.data);
			UploadObject (json.url, filepath);
		}
		else
		{
			Debug.Log("ERROR: " + www.error);
		}  

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
		Debug.Log ("done uploading to " + url);
	}

}
