using UnityEngine;
using System.Collections;

public class FrameRateTest : MonoBehaviour {

	Vector3 pos;
	
	// Update is called once per frame
	void Update () {
		pos = transform.position;
		pos.y = Mathf.Sin (Time.time);
		transform.position = pos;
	}
}
