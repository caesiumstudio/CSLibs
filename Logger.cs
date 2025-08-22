using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

public class Logger
{
    public string tag = "";
    public static bool LOG_LEVEL_DEBUG = true;
    public static bool LOG_LEVEL_INFO = true;
    public static bool LOG_LEVEL_ERROR = true;

    public Logger(string tag) {
        this.tag = tag;
    }

    public void Log(string tag, string msg)
    {
        Debug.Log("CS_LOG: <" + tag + "> " + msg);
    }

    public void Log(string msg)
    {
        Debug.Log(tag + ": " + msg);
    }

    public void Info(string msg)
    {
        Debug.Log("CS_INFO: <" + tag + "> " + msg);
    }

    public void Error(string msg)
    {
        Debug.Log("CS_ERROR: <" + tag + "> " + msg);
    }
}