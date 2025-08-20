using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using CS;
public class CSNetwork : MonoBehaviour
{
    private Logger logger = new Logger("CSNetwork");
    void Start() {
    }

    public string GetJSON(string url) {
        StartCoroutine(GetText(url));
        return "";
    }

    IEnumerator GetText(string url) {
        UnityWebRequest webRequest = UnityWebRequest.Get(url);
        yield return webRequest.SendWebRequest();

        if (webRequest.isNetworkError || webRequest.isHttpError) {
            logger.Log(webRequest.error);
        } else {
            logger.Log(webRequest.downloadHandler.text);
            // Or retrieve results as binary data
            //byte[] results = webRequest.downloadHandler.data;
        }
    }

    //IEnumerator GetRequest(string uri) {
    //    using (UnityWebRequest webRequest = UnityWebRequest.Get(uri)) {
    //        // Request and wait for the desired page.
    //        yield return webRequest.SendWebRequest();

    //        string[] pages = uri.Split('/');
    //        int page = pages.Length - 1;

    //        if (webRequest.isNetworkError) {
    //            Debug.Log(pages[page] + ": Error: " + webRequest.error);
    //        } else {
    //            Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
    //        }
    //    }
    //}
}
