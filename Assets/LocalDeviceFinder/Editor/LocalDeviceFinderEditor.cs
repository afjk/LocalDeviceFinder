using System.Collections.Generic;
using System.Linq;
using System.Timers;
using UnityEditor;
using UnityEngine;

public class LocalDeviceFinderEditor : EditorWindow
{
    private int sndPort = 8080;
    private int rcvPort = 8081;
    private string message;
    private LocalDeviceFinder finder;
    private List<DeviceData> deviceList = new List<DeviceData>(); // List to hold the devices

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
        message = EditorGUILayout.TextField("Message", message);

        if (GUILayout.Button("Send Broadcast"))
        {
            if (finder == null)
            {
                finder = new LocalDeviceFinder();
                finder.Initialize();
            }
            finder.SendBroadcast(sndPort, message);
        }
        
        if (GUILayout.Button("Start Receiving"))
        {
            finder.StartReceiving(rcvPort, OnReceiveDeviceData);
        }
        
        if (GUILayout.Button("Stop"))
        {
            StopScanning();
        }

        // Display the list of devices
        foreach (var device in deviceList.OrderBy(deviceData => deviceData.IPAddress))
        {
            GUILayout.Label($"Device: {device.DeviceName}, IP: {device.IPAddress}");
        }
    }

    public void StartScanning(int sndPort,int rcvPort, string message)
    {
        Debug.Log("Start scanning...");

        if (finder == null)
        {
            finder = new LocalDeviceFinder();
        }

        finder.StartReceiving(rcvPort, OnReceiveDeviceData); // Set the callback to update the device list

        StartSendingBroadcast(sndPort, message);
    }

    // This method is called when a DeviceData is received
    private void OnReceiveDeviceData(DeviceData data)
    {
        // Add the received DeviceData to the list
        deviceList.Add(data);

        // Sort the list by IP address
        deviceList = deviceList.OrderBy(deviceData => deviceData.IPAddress).Distinct().ToList();

        // Schedule the Repaint to be executed on the main thread
        UnityEditor.EditorApplication.delayCall += Repaint;
    }

    private Timer timer;
    public void StopScanning()
    {
        Debug.Log("Stop scanning...");
        StopSendingBroadcast();
        finder.StopReceiving();
    }
    
    public void StartSendingBroadcast(int port, string message)
    {
        timer = new System.Timers.Timer(1000); // Set the interval to 1 second
        timer.Elapsed += (sender, e) => finder.SendBroadcast(port, message);
        timer.Start();
    }
    
    public void StopSendingBroadcast()
    {
        if (timer != null)
        {
            timer.Stop();
            timer = null;
        }
    }

}