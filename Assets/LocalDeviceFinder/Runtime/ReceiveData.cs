using System;
using System.Text;
using UnityEngine;

public interface IReceiveData
{
    public byte[] Serialize();
    
    public IReceiveData Deserialize(byte[] bytes);
}

public class ReceiveData : IReceiveData
{
    public string DeviceName
    {
        get => d;
        set => d = value;
    }

    public string d; // Device name. Serialize時のkey名を短くするために使用

    public ReceiveData(string deviceName)
    {
        DeviceName = deviceName;
    }

    public ReceiveData(byte[] bytes)
    {
        var data = Deserialize(bytes) as ReceiveData;
        this.DeviceName = data.DeviceName;
    }
    
    private string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    private static ReceiveData FromJson(string jsonString)
    {
        return JsonUtility.FromJson<ReceiveData>(jsonString);
    }
    
    public byte[] Serialize()
    {
        return Encoding.Unicode.GetBytes(ToJson());
    }

    public IReceiveData Deserialize(byte[] bytes)
    {
        var message = Encoding.Unicode.GetString(bytes);
        return FromJson(message);
    }
}