using System;
using UnityEngine;

public class ReceiveData
{
    public string DeviceName { get; set; }

    public ReceiveData(string deviceName)
    {
        DeviceName = deviceName;
    }

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public static ReceiveData FromJson(string jsonString)
    {
        return JsonUtility.FromJson<ReceiveData>(jsonString);
    }
}