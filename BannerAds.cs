using GoogleMobileAds.Api;
using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;
using CS;

public class BannerAds
{
    BannerView _bannerView;
    AdRequest _bannerAdRequest;
    private Logger logger = new Logger("BannerAds");

    private string TEST_DEVICE_ID_MOBILE = "AD7F868BA605FAB451A8B4D9C9C1D5F5";
    private string TEST_DEVICE_ID_TABLET = "BE2F070AF06CBB8EBA25EEEA6B740174";

    public void RemoveBannerAds()
    {
        if (Application.platform != RuntimePlatform.WindowsEditor) _bannerView.Hide();
    }

    public void RequestBanner()
    {
        _bannerView = new BannerView(AppData.GetBannerAdUnitId(), AdSize.Banner, AdPosition.Top);

        // _bannerView.OnBannerAdLoaded += HandleOnAdLoaded;
        // _bannerView.OnBannerAdLoadFailed += HandleOnAdFailedToLoad;

        _bannerView.OnBannerAdLoaded += () =>
        {
            Debug.Log("Banner view loaded an ad with response : "
                + _bannerView.GetResponseInfo());
        };
        // Raised when an ad fails to load into the banner view.
        _bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            Debug.LogError("Banner view failed to load an ad with error : "
                + error);
        };
        // Raised when the ad is estimated to have earned money.
        _bannerView.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Banner view paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        _bannerView.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Banner view recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        _bannerView.OnAdClicked += () =>
        {
            Debug.Log("Banner view was clicked.");
        };
        // Raised when an ad opened full screen content.
        _bannerView.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Banner view full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        _bannerView.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Banner view full screen content closed.");
        };

        // bannerView.OnAdLeavingApplication += HandleOnAdLeavingApplication;
        // bannerView.OnAdOpening += HandleOnAdLeavingApplication;

        _bannerAdRequest = new AdRequest();
        _bannerView.LoadAd(_bannerAdRequest);
    }

    // public void HandleOnAdLoaded()
    // {
    //     logger.Log("HandleOnAdLoaded event received.");
    // }

    // public void HandleOnAdOpened(object sender, EventArgs args)
    // {
    //     logger.Log("HandleOnAdOpened event received.");
    // }

    // public void HandleOnAdClosed(object sender, EventArgs args)
    // {
    //     logger.Log("HandleOnAdClosed event received.");
    // }

    // public void HandleOnAdFailedToLoad(LoadAdError error)
    // {
    //     logger.Error("HandleOnAdFailedToLoad event received.");
    // }

    // // public void HandleOnAdLeavingApplication(object sender, EventArgs args) {
    // //     logger.Error("HandleOnAdLeavingApplication event received.");
    // // }
}