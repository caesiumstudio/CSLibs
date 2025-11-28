using UnityEngine;
using Firebase;
using Firebase;
using Firebase.Analytics;
using Firebase.Extensions;
using CS;

public class CSFirebaseAnalytics
{
    public static string APP_OPEN = Firebase.Analytics.FirebaseAnalytics.EventAppOpen;
    public static string SELECT_CONTENT = Firebase.Analytics.FirebaseAnalytics.EventSelectContent;

    public void LogEvent(string evtType, string logMsg)
    {
        Firebase.Analytics.FirebaseAnalytics.LogEvent(evtType, "group_id", "app_open");
    }
}
