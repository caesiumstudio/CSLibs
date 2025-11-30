using System;
using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using CS;

/* ******************* IMPORTANT *************************
Adding app state event notifier needs a lifecycle androidx.lifecycle:lifecycle-process:2.6.0 dependency that GoogleMobileSDK has removed.
So temporarily we put it back as a dependency in /home/ravi/Projects/Unity/Tabla-Real-Sounds/Assets/GoogleMobileAds/Editor/GoogleMobileAdsDependencies.xml
<androidPackage spec="androidx.lifecycle:lifecycle-process:2.6.0">
    <repositories>
        <repository>https://maven.google.com/</repository>
    </repositories>
</androidPackage>

The version might need more adjustments
*/
public class AppOpenAdController : MonoBehaviour
{
    private Logger logger = new Logger("AppOpenAdController");
    private string _adUnitId = "LOADED ON START";

    private DateTime _expireTime;
    private AppOpenAd appOpenAd;

    public void Start()
    {
        _adUnitId = AppData.GetAppOpenAdUnitId();
        logger.Log("AppOpenAd UnitID: " + _adUnitId);
        logger.Log("Adding state change listener");

        AppStateEventNotifier.AppStateChanged += OnAppStateChanged;
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            logger.Log("App Open Ad Sdk Initialized");
            this.LoadAppOpenAd();
        });
    }

    void OnAppStateChanged(AppState state)
    {
        logger.Log("App State changed to : " + state);

        if (state == AppState.Foreground)
        {
            if (IsAdAvailable() && AppData.SHOW_APP_OPEN_ADS)
            {
                logger.Log("Showing app open ad");
                this.ShowAppOpenAd();
            }
            else
            {
                logger.Log("Ad not available.. loading new ad");
                this.LoadLandscapeAd();
            }
        }
        else if (!IsAdAvailable())
        {
            logger.Log("App moved to background and ad not available.. loading new ad");
            this.LoadLandscapeAd();
        }
    }

    private void LoadLandscapeAd()
    {
        if (Screen.orientation == ScreenOrientation.LandscapeRight || Screen.orientation == ScreenOrientation.LandscapeLeft)
        {
            this.LoadAppOpenAd();
        }
        else
        {
            logger.Error("Not loading ad as orientation is not landscape");
        }
    }

    public void ShowAppOpenAd()
    {
        if (AppData.IsProVersion())
        {
            logger.Log("Not showing for paid user");
            return;
        }

        if (appOpenAd != null && appOpenAd.CanShowAd())
        {
            logger.Log("Showing app open ad.");
            appOpenAd.Show();
        }
        else
        {
            logger.Error("App open ad is not ready yet.");
        }
    }



    public void LoadAppOpenAd()
    {
        if (AppData.PAID_USER)
        {
            logger.Log("Not loading for paid user");
            return;
        }
        // Clean up the old ad before loading a new one.
        if (appOpenAd != null)
        {
            appOpenAd.Destroy();
            appOpenAd = null;
        }

        logger.Log("Loading the app open ad.");

        // Create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        AppOpenAd.Load(_adUnitId, adRequest, (AppOpenAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    logger.Error("app open ad failed to load an ad with error : " + error);
                    return;
                }

                logger.Log("App open ad loaded with response : "
                          + ad.GetResponseInfo());

                // App open ads can be preloaded for up to 4 hours.
                _expireTime = DateTime.Now + TimeSpan.FromHours(4);

                appOpenAd = ad;
                RegisterEventHandlers(ad);
            });
    }

    public bool IsAdAvailable()
    {

        logger.Log("appOpenAd: " + appOpenAd);

        if (appOpenAd != null)
            logger.Log("appOpenAd.IsLoaded(): " + appOpenAd.CanShowAd());

        logger.Log("DateTime.Now < _expireTime: " + (DateTime.Now < _expireTime));
        return appOpenAd != null
               && appOpenAd.CanShowAd()
               && DateTime.Now < _expireTime;

    }

    private void RegisterEventHandlers(AppOpenAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            logger.Log(String.Format("App open ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            logger.Log("App open ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            logger.Log("App open ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            logger.Log("App open ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            logger.Log("App open ad full screen content closed.");
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            logger.Error("App open ad failed to open full screen content " +
                           "with error : " + error);
        };
    }



    void OnDestroy()
    {
        AppStateEventNotifier.AppStateChanged -= OnAppStateChanged;
        if (appOpenAd != null) appOpenAd.Destroy();
    }


}