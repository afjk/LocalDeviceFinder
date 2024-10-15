using System.Collections.Generic;
using System.Linq;
using System.Timers;
using UnityEditor;
using UnityEngine;

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

public class LocalDeviceFinderEditor : EditorWindow
{
    private int sndPort = 8080;
    private int rcvPort = 8080;
    private DeviceSearcher searcher;
    private DeviceResponder responder;
    private List<DeviceData> deviceList = new(); // List to hold the devices
    private bool useMulticast = false;
    private string multicastIP = "239.0.0.222"; // デフォルトのマルチキャストアドレス

    [MenuItem("Tools/Local Device Finder")]
    public static void ShowWindow()
    {
        GetWindow<LocalDeviceFinderEditor>("Local Device Finder");
    }

    private void OnGUI()
    {
        GUILayout.Label("Local Device Finder", EditorStyles.boldLabel);

        sndPort = EditorGUILayout.IntField("Send Port", sndPort);
        rcvPort = EditorGUILayout.IntField("Receive Port", rcvPort);

        useMulticast = EditorGUILayout.Toggle("Use Multicast", useMulticast);

        if (useMulticast)
        {
            multicastIP = EditorGUILayout.TextField("Multicast IP", multicastIP);
        }

        if (GUILayout.Button("Start Finding"))
        {
            if (searcher == null)
            {
                if (useMulticast)
                {
                    searcher = new DeviceSearcher(new ReceiveDataFactory(), rcvPort, multicastIP);
                }
                else
                {
                    searcher = new DeviceSearcher(new ReceiveDataFactory(), rcvPort);
                }
            }

            if (useMulticast)
            {
                searcher.SendMulticast(sndPort);
            }
            else
            {
                searcher.SendBroadcast(sndPort);
            }

            searcher.StartReceiving(OnReceiveDeviceData);
            Debug.Log("Finding started");
            
            Timer timer = new System.Timers.Timer(5000);
            timer.Elapsed += (sender, e) =>
            {
                searcher.StopReceiving();
                Debug.Log("Finding stopped");
            }; // Stop receiving when the timer elapses
            timer.AutoReset = false;
            timer.Start();
        }
        
        if (GUILayout.Button("Start Receiver"))
        {
            if (responder == null)
            {
                if (useMulticast)
                {
                    responder = new DeviceResponder(new ReceiveDataFactory(), rcvPort, sndPort, multicastIP);
                }
                else
                {
                    responder = new DeviceResponder(new ReceiveDataFactory(), rcvPort, sndPort);
                }
            }
            responder.StartListening();
            Debug.Log("Receiver started");
        }
        
        if (GUILayout.Button("Stop Receiver"))
        {
            searcher?.StopReceiving();
            responder?.StopListening();
            searcher = null;
            responder = null;
            Debug.Log("Receiver stopped");
        }

        // Display the list of devices
        GUILayout.Label("Discovered Devices:", EditorStyles.boldLabel);
        foreach (var device in deviceList)
        {
            GUILayout.Label($"Device: {device.DeviceName}, IP: {device.IpAddress}");
        }
    }
    
    private void OnReceiveDeviceData(IReceiveData idata, string ipAddress)
    {
        var data = idata as ReceiveData;
        if (data != null)
        {
            deviceList.Add(new DeviceData(data.DeviceName, ipAddress));
            UnityEditor.EditorApplication.delayCall += Repaint;
        }
    }
}
