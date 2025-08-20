using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CS;
public class HideOnAndroid : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (Utils.IsAndroid())
        {
            this.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
