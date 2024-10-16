using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using com.afjk.LocalDeviceFinder;
using UnityEngine;
using UnityEngine.UI;

namespace com.afjk.LocalDeviceFinder.sample
{
    class DeviceData
    {
        public DeviceData(string dataDeviceName, string ipAddress)
        {
            DeviceName = dataDeviceName;
            IpAddress = ipAddress;
        }

        public string DeviceName { get; set; }
        public string IpAddress { get; set; }
    }

    public class LocalDeviceFinder : MonoBehaviour
    {
        [SerializeField]
        private int sendPort = 8080;
        [SerializeField]
        private int receivePort = 8081;
        [SerializeField] 
        private Text DiscoveredDevicesText;
        private DeviceSearcher searcher;
        private DeviceResponder responder;
        private List<DeviceData> deviceList = new(); // List to hold the devices
        private bool needUpdateDeviceList=false;
        
        public void StartFinding()
        {
            StopAll();
            deviceList.Clear();
            DiscoveredDevicesText.text = "";
            needUpdateDeviceList = true;
            searcher = new DeviceSearcher(new ReceiveDataFactory(), receivePort);
            searcher.StartReceiving(OnReceiveDeviceData);
            searcher.SendBroadcast(sendPort);
        }
        
        private void OnReceiveDeviceData(IReceiveData receiveData, string ipAddress)
        {
            Debug.Log($"Debug OnReceiveDeviceData:{ipAddress}");
            var data = receiveData as ReceiveData;
            if (!deviceList.Any(d => d.IpAddress == ipAddress))
            {
                deviceList.Add(new DeviceData(data.DeviceName, ipAddress));
            }

            needUpdateDeviceList = true;
        }

        private void Update()
        {
            if (!needUpdateDeviceList) { return;}
            needUpdateDeviceList = false;

            DiscoveredDevicesText.text = "";
            foreach (var deviceData in deviceList)
            {
                DiscoveredDevicesText.text += $"{deviceData.DeviceName}:{deviceData.IpAddress}\n";
            }
        }
        
        private void OnDestroy()
        {
            StopAll();
        }

        void StopAll()
        {
            searcher?.StopReceiving();
            responder?.StopListening();
            searcher = null;
            responder = null;
        }
    }
}