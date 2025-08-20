using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;
using GoogleMobileAds.Api;
using Random = System.Random;
namespace CS
{
    public class Utils
    {
        public static UserPrefs userPref = new UserPrefs();
        public static Logger logger = new Logger("Utils");
        public static Timer timer = new Timer();
        public static Social social = new Social();
        public static BannerAds bannerAds = new BannerAds();
        public static InterstitialAds interstitialAds = new InterstitialAds();
        public static FirebaseAnalytics firebaseAnalytics = new FirebaseAnalytics();
        public static CSFirebaseMessaging csFireBaseMessaging = new CSFirebaseMessaging();

        public static GameObject GetGameObjectByTag(string tag)
        {
            GameObject go = GameObject.FindGameObjectWithTag(tag);
            if (go == null) Utils.logger.Error("Object with Tag " + tag + " is NOT found");

            return go;
        }

        public static GameObject GetGameObjectByName(string name)
        {
            GameObject go = GameObject.Find(name);
            if (go == null) Utils.logger.Error("Object with Name " + name + " is NOT found");
            return go;
        }

        public static void ExitApp()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                    activity.Call<bool>("moveTaskToBack", true);
                    break;
                case RuntimePlatform.WindowsEditor:
                    logger.Log("Exiting from Unity Editor");
                    break;
            }
        }

        public static string getRandomString(int length)
        {
            Random random = new Random();
            string randomString = "";
            for (var i = 0; i < length; i++)
            {
                randomString += ((char)(random.Next(1, 26) + 64)).ToString().ToLower();
            }
            return randomString;
        }
        public static bool IsAndroid()
        {
            return Application.platform == RuntimePlatform.Android;
        }

        public static bool IsIPhone()
        {
            return Application.platform == RuntimePlatform.IPhonePlayer;
        }

        public static bool IsEditor()
        {
            return !Utils.IsAndroid() && !Utils.IsIPhone();
        }

        public static void Rate()
        {
            if (Utils.IsAndroid())
            {
                Application.OpenURL(AppData.APP_LINK_ANDROID);
            }
            else if (Utils.IsIPhone())
            {
                Application.OpenURL(AppData.APP_LINK_IOS);
            }
        }

        public static void SendEMail(string email, string sub)
        {
            string subject = EscapeUrl(sub);
            string body = EscapeUrl("");
            Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);
        }

        private static string EscapeUrl(string url)
        {
            return WWW.EscapeURL(url).Replace("+", "%20");
        }

        public static void OpenURL(string url)
        {
            Application.OpenURL(url);
        }

        public static void Like()
        {
            Application.OpenURL("fb://page/1100328256709779");
        }

        public static string GetAppVersion()
        {
            return Application.version + (Debug.isDebugBuild ? " - (debug)" : " - (release)");
        }

        public static bool IsFirstRun()
        {

            if (Utils.userPref.GetInt("FIRST_RUN", 1) == 1)
            {
                Utils.userPref.SetInt("FIRST_RUN", 0);
                return true;
            }
            return false;
        }

        // logger
        public static void Log(string tag, string msg)
        {
            if (Logger.LOG_LEVEL_DEBUG) Utils.logger.Log(tag, msg);
        }

        public static void Log(string msg)
        {
            if (Logger.LOG_LEVEL_DEBUG) Utils.logger.Log(msg);
        }

        public static void Info(string msg)
        {
            if (Logger.LOG_LEVEL_INFO) Utils.logger.Info(msg);
        }
        public static void Error(string msg)
        {
            if (Logger.LOG_LEVEL_ERROR) Utils.logger.Error(msg);
        }
    }
}

