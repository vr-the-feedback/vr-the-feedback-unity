using UnityEngine;

public static class PermissionHelper
{
    private static string ACTIVITY_NAME = "com.unity3d.player.UnityPlayer";
    private static string CONTEXT = "currentActivity";
    private static string PERMISSION_HELPER_CLASS = "com.marineverse.permissionhelper.PermissionHelper";

    private static AndroidJavaObject permissionHelper = null;

    private static void InitIfNeeded()
    {
    #if UNITY_ANDROID
        if (permissionHelper == null)
        {
            AndroidJavaObject context = new AndroidJavaClass(ACTIVITY_NAME).GetStatic<AndroidJavaObject>(CONTEXT);
            permissionHelper = new AndroidJavaObject(PERMISSION_HELPER_CLASS, context);
        }
    #endif
    }

    public static class Permissions
    {
        public static string INTERNET = "android.permission.INTERNET";
        public static string ACCESS_NETWORK_STATE = "android.permission.ACCESS_NETWORK_STATE";
        public static string RECORD_AUDIO = "android.permission.RECORD_AUDIO";
        public static string READ_EXTERNAL_STORAGE = "android.permission.READ_EXTERNAL_STORAGE";
        public static string WRITE_EXTERNAL_STORAGE = "android.permission.WRITE_EXTERNAL_STORAGE";
    };

    public static void RequestPermission(string permission)
    {
        InitIfNeeded();
#if UNITY_ANDROID
        permissionHelper.Call("RequestPermission", permission);
#endif
    }
    public static bool CheckForPermission(string permission)
    {
        InitIfNeeded();
#if UNITY_ANDROID
        return permissionHelper.Call<bool>("CheckForPermission", permission);
#else
          return false;
#endif
    }
}