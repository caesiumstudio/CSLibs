using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;
using GoogleMobileAds.Api;

public class UserPrefs {
    public bool HasKey(string key) {
        return PlayerPrefs.HasKey(key);
    }
    public void DeleteKey(string key) {
        PlayerPrefs.DeleteKey(key);
    }
    public int GetInt(string key, int defValue) {
        return PlayerPrefs.GetInt(key, defValue);
    }
    public void SetInt(string key, int value) {
        PlayerPrefs.SetInt(key, value);
    }

    public decimal GetDecimal(string key, decimal defValue) {
        return (decimal) PlayerPrefs.GetFloat(key, (float)defValue);
    }

    public void SetDecimal(string key, decimal value) {
        PlayerPrefs.SetFloat(key, (float)value);
    }

    public float GetFloat(string key, float defValue) {
        return PlayerPrefs.GetFloat(key, defValue);
    }

    public void SetFloat(string key, float value) {
        PlayerPrefs.SetFloat(key, value);
    }

    public string GetString(string key, string defValue) {
        return PlayerPrefs.GetString(key, defValue);
    }
}