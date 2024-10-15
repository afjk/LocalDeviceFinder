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

        // ポート設定
        sndPort = EditorGUILayout.IntField("Send Port", sndPort);
        rcvPort = sndPort;

        bool newUseMulticast = EditorGUILayout.Toggle("Use Multicast", useMulticast);

        if (newUseMulticast)
        {
            multicastIP = EditorGUILayout.TextField("Multicast IP", multicastIP);
        }

        // マルチキャスト設定が変更された場合、searcherとresponderを停止してnullにする
        if (newUseMulticast != useMulticast)
        {
            StopAll(); // 動作中のsearcherやresponderを停止
            useMulticast = newUseMulticast;
        }

        if (GUILayout.Button("Start Finding"))
        {
            deviceList.Clear();
            if (searcher == null)
            {
                searcher = new DeviceSearcher(new ReceiveDataFactory(), rcvPort, useMulticast ? multicastIP : null);
                searcher.StartReceiving(OnReceiveDeviceData);
            }

            if (useMulticast)
            {
                searcher.SendMulticast(sndPort);
            }
            else
            {
                searcher.SendBroadcast(sndPort);
            }

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
                responder = new DeviceResponder(new ReceiveDataFactory(), rcvPort, sndPort, useMulticast ? multicastIP : null);
            }
            responder.StartListening();
            Debug.Log("Receiver started");
        }

        if (GUILayout.Button("Stop Receiver"))
        {
            StopAll();
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
        Debug.Log($"OnReceiveDeviceData: {data.DeviceName}");
        // 既にリストに存在しない場合のみ追加
        if (!deviceList.Any(d => d.IpAddress == ipAddress))
        {
            deviceList.Add(new DeviceData(data.DeviceName, ipAddress));
        }
        UnityEditor.EditorApplication.delayCall += Repaint;
    }

    private void StopAll()
    {
        searcher?.StopReceiving();
        responder?.StopListening();
        searcher = null;
        responder = null;
    }
}
