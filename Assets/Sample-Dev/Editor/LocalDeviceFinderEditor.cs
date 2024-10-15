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

    [MenuItem("Tools/Local Device Finder")]
    public static void ShowWindow()
    {
        GetWindow<LocalDeviceFinderEditor>("Local Device Finder");
    }

    private void OnGUI()
    {
        GUILayout.Label("Local Device Finder", EditorStyles.boldLabel);

        sndPort = EditorGUILayout.IntField("Send Port", sndPort);
        rcvPort = sndPort;

        if (GUILayout.Button("Start Finding"))
        {
            if (searcher == null)
            {
                searcher = new DeviceSearcher(new ReceiveDataFactory(), rcvPort);
            }
            searcher.SendBroadcast(sndPort);
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
                responder = new DeviceResponder(new ReceiveDataFactory(), rcvPort, sndPort);
            }
            responder.StartListening();
            Debug.Log("Receiver started");
        }
        
        if (GUILayout.Button("Stop Receiver"))
        {
            searcher?.StopReceiving();
            responder?.StopListening();
            Debug.Log("Receiver stopped");
        }

        // Display the list of devices
        foreach (var device in deviceList)
        {
            GUILayout.Label($"Device: {device.DeviceName}, IP: {device.IpAddress}");
        }
    }
    
    private void OnReceiveDeviceData(IReceiveData idata, string ipAddress)
    {
        var data = idata as ReceiveData;
        Debug.Log($"OnReceiveDeviceData: {data.DeviceName}");
        // Add the received DeviceData to the list
        deviceList.Add(new DeviceData(data.DeviceName, ipAddress));
    
        // Schedule the Repaint to be executed on the main thread
        UnityEditor.EditorApplication.delayCall += Repaint;
    }
}