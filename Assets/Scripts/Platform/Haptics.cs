using UnityEngine;

/// <summary>
/// Short system haptic tick for UI feedback.
/// Handheld.Vibrate() is too long for this, so on Android it calls View.performHapticFeedback,
/// which also respects the user's system haptics setting. No-op on other platforms.
/// </summary>
public static class Haptics
{
#if UNITY_ANDROID && !UNITY_EDITOR
    private const int VIRTUAL_KEY = 1; // android.view.HapticFeedbackConstants.VIRTUAL_KEY

    private static AndroidJavaObject activity;
#endif

    public static void Tick()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (activity == null)
        {
            using AndroidJavaClass unityPlayer = new("com.unity3d.player.UnityPlayer");
            activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        }

        // Views must be touched from Android's UI thread
        activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
        {
            using AndroidJavaObject window = activity.Call<AndroidJavaObject>("getWindow");
            using AndroidJavaObject decorView = window.Call<AndroidJavaObject>("getDecorView");
            decorView.Call<bool>("performHapticFeedback", VIRTUAL_KEY);
        }));
#endif
    }
}
