using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FeedbackDemoUIController : MonoBehaviour {

    public Text instruction;
    public VRTheFeedbackManager vrTheFeedbackManager;
    public Button recordFeedback;
    public Button uploadFeedback;

    public void Start()
    {
        recordFeedback.enabled = true;
        uploadFeedback.enabled = false;
    }

    public void OnClickRecordFeedback()
    {
        Debug.Log("OnClickRecordFeedback");
        recordFeedback.enabled = false;
        uploadFeedback.enabled = true;
        instruction.text = "Please talk - record test feedback.";
        vrTheFeedbackManager.RecordFeedback();
    }

    public void OnClickUploadFeedback()
    {
        Debug.Log("OnClickUploadFeedback");
        recordFeedback.enabled = true;
        uploadFeedback.enabled = false;
        vrTheFeedbackManager.SaveFeedback();
    }

}
