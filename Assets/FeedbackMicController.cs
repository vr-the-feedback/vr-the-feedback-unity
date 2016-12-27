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
        audioSource.clip = errorClip;
        audioSource.Play();
        GetComponent<Renderer>().material = readyMaterial;
    }

    private void FeedbackManager_FeedbackSuccessfullyUploaded(object sender, VRTheFeedbackManager.VRTheFeedbackEventArgs e)
    {
        audioSource.clip = successClip;
        audioSource.Play();
        GetComponent<Renderer>().material = readyMaterial;
    }

    private void Interactable_InteractableObjectUngrabbed(object sender, VRTK.InteractableObjectEventArgs e)
    {
        Debug.Log("ungrabbed");
        audioSource.clip = confirmationClip;
        audioSource.Play();
        GetComponent<Renderer>().material = workingMaterial;
        feedbackManager.SaveFeedback();
    }

    private void Interactable_InteractableObjectGrabbed(object sender, VRTK.InteractableObjectEventArgs e)
    {
        Debug.Log("grabbed");
        audioSource.clip = openClip;
        audioSource.Play();
        GetComponent<Renderer>().material = recordingMaterial;
        feedbackManager.RecordFeedback();
    }
}
