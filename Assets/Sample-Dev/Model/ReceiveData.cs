using System.Text;
using UnityEngine;

namespace com.afjk.LocalDeviceFinder.sample
{
    public class ReceiveDataFactory : IReceiveDataFactory
    {
        string deviceName;

        public ReceiveDataFactory()
        {
            deviceName = SystemInfo.deviceName;
        }

        public IReceiveData Create()
        {
            return new ReceiveData(deviceName);
        }
    }


    /// <summary>
    /// Json format
    /// {"d": "DeviceName"}
    /// </summary>
    public class ReceiveData : IReceiveData
    {
        public string DeviceName
        {
            get => d;
            private set => d = value;
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
}