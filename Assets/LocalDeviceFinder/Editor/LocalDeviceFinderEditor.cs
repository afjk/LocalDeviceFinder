using System.Collections.Generic;
using System.Linq;
using System.Timers;
using UnityEditor;
using UnityEngine;

public class LocalDeviceFinderEditor : EditorWindow
{
    private int sndPort = 8080;
    private int rcvPort = 8081;
    private LocalDeviceFinder finder;
    private List<ReceiveData> deviceList = new(); // List to hold the devices

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

        if (GUILayout.Button("Start Finding"))
        {
            if (finder == null)
            {
                finder = new LocalDeviceFinder();
                finder.Initialize();
            }
            finder.SendBroadcast(sndPort);
            finder.StartReceiving(rcvPort,OnReceiveDeviceData);
            Debug.Log("Finding started");
        }
        
        if (GUILayout.Button("Start Receiver"))
        {
            if (finder == null)
            {
                finder = new LocalDeviceFinder();
                finder.Initialize();
            }
            finder.Ack(rcvPort, sndPort);
            Debug.Log("Receiver started");
        }
        
        if (GUILayout.Button("Stop Receiver"))
        {
            finder?.StopReceiving();
            Debug.Log("Receiver stopped");
        }

        // Display the list of devices
        foreach (var device in deviceList.OrderBy(deviceData => deviceData.FromIPAddress))
        {
            GUILayout.Label($"Device: {device.DeviceName}, IP: {device.FromIPAddress}");
        }
    }
    
    private void OnReceiveDeviceData(ReceiveData data)
    {
        Debug.Log($"OnReceiveDeviceData: {data.DeviceName} {data.IPAddress} {data.FromIPAddress} {data.Message}");
        // Add the received DeviceData to the list
        deviceList.Add(data);
    
        // Sort the list by IP address
        // deviceList = deviceList.OrderBy(deviceData => deviceData.IPAddress).Distinct().ToList();
    
        // Schedule the Repaint to be executed on the main thread
        UnityEditor.EditorApplication.delayCall += Repaint;
    }
}