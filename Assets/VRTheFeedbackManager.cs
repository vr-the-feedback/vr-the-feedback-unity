using UnityEngine;
using System.Collections;
using System.IO;
using System.Net;

public class VRTheFeedbackManager : MonoBehaviour {

    private AudioSource myAudio;

	// Use this for initialization
	void Start () {
        myAudio = GetComponent<AudioSource>();

    }
	
    public void RecordFeedback()
    {
        myAudio.clip = Microphone.Start(null, false, 120, 44100);
    }

    public void SaveFeedback()
    {
        string filePath = new SavWav().Save("test.wav", myAudio.clip);
		if (filePath != null) {
			
			Debug.Log ("would upload from " + filePath);
			UploadObject ("REPLACE_ME", filePath);
			Debug.Log ("done");
		}			
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
	}

}
