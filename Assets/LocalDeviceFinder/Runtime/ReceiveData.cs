using System;
using UnityEngine;

public class ReceiveData
{
    public string DeviceName { get; set; }
    public string IPAddress { get; set; }
    public string FromIPAddress { get; set; }
    public string Message { get; set; }

    public ReceiveData(string deviceName, string ipAddress, string fromIPAddress, string message)
    {
        DeviceName = deviceName;
        IPAddress = ipAddress;
        FromIPAddress = fromIPAddress;
        Message = message;
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