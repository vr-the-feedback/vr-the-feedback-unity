using UnityEngine;
using System.Collections;

public class FeedbackMicController : MonoBehaviour {

    VRTK.VRTK_InteractableObject interactable;

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

    private bool canRecordFeedback = true;

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

    private void Interactable_InteractableObjectUngrabbed(object sender, VRTK.InteractableObjectEventArgs e)
    {
        Debug.Log("ungrabbed");
        if (canRecordFeedback) {
            infoText.text = "Uploading feedback...";
            canRecordFeedback = false;
            audioSource.clip = confirmationClip;
            audioSource.Play();
            GetComponent<Renderer>().material = workingMaterial;
            feedbackManager.SaveFeedback();
        }
    }

    private void Interactable_InteractableObjectGrabbed(object sender, VRTK.InteractableObjectEventArgs e)
    {
        Debug.Log("grabbed");
        if (canRecordFeedback) {
            infoText.text = "Speak now.\nRelease trigger to upload.";
            audioSource.clip = openClip;
            audioSource.Play();
            GetComponent<Renderer>().material = recordingMaterial;
            feedbackManager.RecordFeedback();
        }
    }
}
