using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;
using GoogleMobileAds.Api;

public class Timer {
    public IEnumerator SetTicker(float duration, float steps, Action<float> tickerCallback) {
        float step = 1 / steps;
        float totalSteps = 0;

        while (1 > (int)totalSteps) {
            totalSteps += step;
            tickerCallback(totalSteps);
            yield return 0;
            yield return new WaitForSecondsRealtime(duration / steps);
        }
    }
}