using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;
using GoogleMobileAds.Api;
using CS;
public class Social
{
    private bool sharingInProgress = false;

    public IEnumerator Share()
    {
        if (!sharingInProgress) yield return null;

        sharingInProgress = true;
        NativeShare nativeShare = new NativeShare();
        nativeShare.SetTitle("Tabla App");
        nativeShare.SetSubject("Hey there!!");
        nativeShare.SetText("Check out this app");
        if (Utils.IsIPhone())
        {
            nativeShare.SetUrl(AppData.APP_LINK_IOS);
        }
        else
        {
            nativeShare.SetUrl(AppData.APP_LINK_ANDROID);
        }

        nativeShare.Share();
        sharingInProgress = false;
    }
}
