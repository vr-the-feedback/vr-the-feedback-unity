using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class FeedbackMetadataProvider : MonoBehaviour {

    public abstract Dictionary<string, string> GetFeedbackMetadata();
}
