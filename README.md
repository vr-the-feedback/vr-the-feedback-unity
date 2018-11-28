# VR The Feedback - HTC Vive.

Virtual Reality Voice Feedback Plugin for Unity & SteamVR.

Learn more: http://www.vrthefeedback.com/ and https://twitter.com/vrthefeedback

## Development

This is an alpha version of voice recording plugin. Contact vrthefeedback@gmail.com for account setup and access to backend / storage (including PROJECT_KEY).

Testing: Open ./Assets/VRTheFeedback/Scenes/Demo2DUI.unity in Unity. Enter "Play Mode". Click "Record Feedback" and start speaking. Click "Upload Feedback". Your message should be recorded as mp3. It won't be uploaded to server unless you configure a valid PROJECT_KEY on VRTheFeedbackManager Game Object.

## Installation

* Download vr-the-feedback-0.3.unitypackage
* Import in Unity (requires SteamVR 1.2.3 and VRTK 3.2.0 for VR scene to work.)
* Drag prefabs into the scene: VRTheFeedbackManager, GrabbableMic
* Configure PROJECT_KEY on VRTheFeedbackManager.
* Set VRTheFeedbackManager on GrabbableMic.
