using UnityEngine;
using System.Collections;

public class FeedbackMicController : MonoBehaviour {

    VRTK.VRTK_InteractableObject interactable;

    public Material recordingMaterial;
    public Material workingMaterial;
    public Material readyMaterial;
    public VRTheFeedbackManager feedbackManager;

    // Use this for initialization
    void Start () {
        interactable = GetComponent<VRTK.VRTK_InteractableObject>();
        interactable.InteractableObjectGrabbed += Interactable_InteractableObjectGrabbed;
        interactable.InteractableObjectUngrabbed += Interactable_InteractableObjectUngrabbed;
    }

    private void Interactable_InteractableObjectUngrabbed(object sender, VRTK.InteractableObjectEventArgs e)
    {
        Debug.Log("ungrabbed");
        GetComponent<Renderer>().material = workingMaterial;
        feedbackManager.SaveFeedback();
    }

    private void Interactable_InteractableObjectGrabbed(object sender, VRTK.InteractableObjectEventArgs e)
    {
        Debug.Log("grabbed");
        GetComponent<Renderer>().material = recordingMaterial;
        feedbackManager.RecordFeedback();
    }
}
