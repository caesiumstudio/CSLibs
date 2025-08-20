using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CS;
public class HideForProUser : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (AppData.IS_TABLA_PRO)
        {
            this.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
