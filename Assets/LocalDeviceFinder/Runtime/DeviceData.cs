using System;
using UnityEngine;

public class DeviceData
{
    public string DeviceName { get; set; }
    public string IPAddress { get; set; }
    public string FromIPAddress { get; set; }

    public DeviceData(string deviceName, string ipAddress, string fromIPAddress)
    {
        DeviceName = deviceName;
        IPAddress = ipAddress;
        FromIPAddress = fromIPAddress;
    }

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public static DeviceData FromJson(string jsonString)
    {
        return JsonUtility.FromJson<DeviceData>(jsonString);
    }
}