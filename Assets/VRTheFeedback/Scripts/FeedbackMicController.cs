using UnityEngine;
using System.Collections;

public class FeedbackMicController : MonoBehaviour {

    VRTK.VRTK_InteractableObject interactable;

    public FeedbackMetadataProvider feedbackMetadataProvider;
    public Material recordingMaterial;
    public Material workingMaterial;
    public Material readyMaterial;
    public VRTheFeedbackManager feedbackManager;
    public AudioSource audioSource;
    public AudioClip openClip;
    public AudioClip successClip;
    public AudioClip errorClip;
    public AudioClip confirmationClip;
    public TextMesh infoText;
    public float waitTimeForStartRecording = 4;
    public bool isGrabbed = false;
    private bool canRecordFeedback = true;
    private bool shouldUploadFeedback = false;
    private float timeGrabbed;
    private float micRecordingTime;

    // Use this for initialization
    void Start () {
        interactable = GetComponent<VRTK.VRTK_InteractableObject>();
        interactable.InteractableObjectGrabbed += Interactable_InteractableObjectGrabbed;
        interactable.InteractableObjectUngrabbed += Interactable_InteractableObjectUngrabbed;
        feedbackManager.FeedbackSuccessfullyUploaded += FeedbackManager_FeedbackSuccessfullyUploaded;
        feedbackManager.FeedbackFailedDueToError += FeedbackManager_FeedbackFailedDueToError;
    }

    private void FeedbackManager_FeedbackFailedDueToError(object sender, VRTheFeedbackManager.VRTheFeedbackEventArgs e)
    {
        infoText.text = "Something went wrong.\nPlease try again.";
        audioSource.clip = errorClip;
        audioSource.Play();
        GetComponent<Renderer>().material = readyMaterial;
        canRecordFeedback = true;
    }

    private void FeedbackManager_FeedbackSuccessfullyUploaded(object sender, VRTheFeedbackManager.VRTheFeedbackEventArgs e)
    {
        infoText.text = "Thank you!\nGrab again to record another message.";
        audioSource.clip = successClip;
        audioSource.Play();
        GetComponent<Renderer>().material = readyMaterial;
        canRecordFeedback = true;
    }

    public void Update()
    {
        if (isGrabbed && canRecordFeedback)
        {
            if (!feedbackManager.isRecording)
            {
                if (timeGrabbed + waitTimeForStartRecording < Time.time)
                {
                    infoText.text = "Speak now.\nRelease trigger to upload.";
                    audioSource.clip = openClip;
                    audioSource.Play();
                    GetComponent<Renderer>().material = recordingMaterial;
                    micRecordingTime = Time.time;
                    feedbackManager.RecordFeedback();
                    shouldUploadFeedback = true;
                    canRecordFeedback = false;
                } else
                {
                    infoText.text = "Start speaking in " + (waitTimeForStartRecording + (timeGrabbed - Time.time)).ToString("0.00");
                }

            }

        }
    }

    private void Interactable_InteractableObjectUngrabbed(object sender, VRTK.InteractableObjectEventArgs e)
    {
        isGrabbed = false;
        if (shouldUploadFeedback) {
            infoText.text = "Uploading feedback...";
            shouldUploadFeedback = false;
            audioSource.clip = confirmationClip;
            audioSource.Play();
            GetComponent<Renderer>().material = workingMaterial;
            var metadata = feedbackMetadataProvider.GetFeedbackMetadata();
            metadata.Add("mic-holding-time", (Time.time - micRecordingTime).ToString());
            feedbackManager.SaveFeedback(metadata);
        } else
        {
            infoText.text = "Grab me to\nrecord voice feedback";
        }
    }

    private void Interactable_InteractableObjectGrabbed(object sender, VRTK.InteractableObjectEventArgs e)
    {
        isGrabbed = true;
        timeGrabbed = Time.time;
        if (!PermissionHelper.CheckForPermission(PermissionHelper.Permissions.RECORD_AUDIO))
        {
            PermissionHelper.RequestPermission(PermissionHelper.Permissions.RECORD_AUDIO);
        }
    }
}
