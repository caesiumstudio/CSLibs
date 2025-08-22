using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Messaging;
using Firebase.Extensions;

using CS;
public class CSFirebaseMessaging
{
    private Logger logger = new Logger("CSFirebaseMessaging");

    public CSFirebaseMessaging()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == Firebase.DependencyStatus.Available)
            {
                logger.Log("FCM WORKING");
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: ");
            }
        });
    }

    void InitializeFirebase()
    {
        Firebase.Messaging.FirebaseMessaging.TokenRegistrationOnInitEnabled = true;
        Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
        Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;

        this.SubscribeToTopic();
        // On iOS, this will display the notification prompt
        Firebase.Messaging.FirebaseMessaging.RequestPermissionAsync().ContinueWithOnMainThread(
          task =>
          {
              logger.Log("RequestPermissionAsync");
          }
        );
    }

    public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
    {
        UnityEngine.Debug.Log("OnTokenReceived: " + token.Token);
        this.SubscribeToTopic();
    }

    void SubscribeToTopic()
    {
        string topic = Utils.IsAndroid() ? "androidbroadcast" : "iosbroadcast";
        if (!AppData.BUILD_RELEASE)
        {
            topic = "testdebug";
        }

        logger.Log("Subscribing to " + topic);
        Firebase.Messaging.FirebaseMessaging.SubscribeAsync(topic).ContinueWithOnMainThread(task =>
        {
            logger.Log("Subscribe Async Success " + topic);
        });
    }

    public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
    {
        logger.Log("Received Message Recieved");

        FirebaseMessage firebaseMessage = e.Message;
        IDictionary<string, string> notifData = e.Message.Data;
        FirebaseNotification notifNotification = e.Message.Notification;

        if (firebaseMessage.From.Length > 0) logger.Log("from: " + firebaseMessage.From);

        if (firebaseMessage.Notification != null)
        {
            logger.Log("Title: " + notifNotification.Title);
            logger.Log("Body: " + notifNotification.Body);
        }

        if (firebaseMessage.Data.Count > 0)
        {
            string title = null;
            string message = null;
            string url = null;

            foreach (KeyValuePair<string, string> iter in e.Message.Data)
            {
                logger.Log("  " + iter.Key + ": " + iter.Value);
                if (iter.Key == "title") title = iter.Value;
                if (iter.Key == "message") message = iter.Value;
                if (iter.Key == "url") url = iter.Value;
            }

            if (url != null) Utils.OpenURL(url);
        }
    }

}


