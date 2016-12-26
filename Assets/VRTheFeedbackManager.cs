using UnityEngine;
using System.Collections;

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

}
