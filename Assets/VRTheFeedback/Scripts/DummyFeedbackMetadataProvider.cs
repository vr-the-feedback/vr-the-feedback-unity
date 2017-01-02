using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class DummyFeedbackMetadataProvider : FeedbackMetadataProvider
{
    public override Dictionary<string, string> GetFeedbackMetadata()
    {
        return new Dictionary<string, string>();
    }
}
