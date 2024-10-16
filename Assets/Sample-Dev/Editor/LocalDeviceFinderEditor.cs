using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    Timer findTimer;
    private string currentState = "";
    private string receiveMessage = "";

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

        if (newUseMulticast != useMulticast)
        {
            StopAll();
            useMulticast = newUseMulticast;
        }

        if (GUILayout.Button("Start Finding"))
        {
            deviceList.Clear();
            StopAll();
            searcher = new DeviceSearcher(new ReceiveDataFactory(), rcvPort, useMulticast ? multicastIP : null);
            searcher.StartReceiving(OnReceiveDeviceData);

            if (useMulticast)
            {
                searcher.SendMulticast(sndPort);
            }
            else
            {
                searcher.SendBroadcast(sndPort);
            }

            Debug.Log("Finding started");

            findTimer = new System.Timers.Timer(5000);
            findTimer.Elapsed += (sender, e) =>
            {
                searcher.StopReceiving();
                searcher = null;
                Debug.Log("Finding stopped");
            };
            findTimer.AutoReset = false;
            findTimer.Start();
            
            currentState = "StartFinding";
        }

        if (GUILayout.Button("Start Receiver"))
        {
            deviceList.Clear();
            StopAll();
            responder = new DeviceResponder(new ReceiveDataFactory(), rcvPort, sndPort, useMulticast ? multicastIP : null);
            responder.StartListening(((bytes, ipAddress) =>
            {
                var message = Encoding.Unicode.GetString(bytes);
                receiveMessage = $"{message} from {ipAddress}";
                Debug.Log(receiveMessage);
            }));
            Debug.Log("Receiver started");
            
            currentState = "StartReceiver";
        }

        if (GUILayout.Button("Stop"))
        {
            StopAll();
            deviceList.Clear();
            Debug.Log("Receiver stopped");
            
            currentState = "Stop";
        }
        
        // Display the list of devices or the receiving state based on the current state
        if (currentState == "StartFinding")
        {
            GUILayout.Label("Say Hello to Discovered Devices:", EditorStyles.boldLabel);
            foreach (var device in deviceList)
            {
                if (GUILayout.Button($"Device: {device.DeviceName}, IP: {device.IpAddress}"))
                {
                    byte[] message = Encoding.Unicode.GetBytes($"Hello");
                    StopAll();
                    searcher = new DeviceSearcher(new ReceiveDataFactory(), rcvPort, useMulticast ? multicastIP : null);
                    searcher.SendUnicast(message, device.IpAddress, sndPort);
                }
            }
        }
        else if (currentState == "StartReceiver")
        {
            GUILayout.Label("Receiving", EditorStyles.boldLabel);
            if (!string.IsNullOrEmpty(receiveMessage))
            {
                GUILayout.Label(receiveMessage, EditorStyles.boldLabel);
            }
        }
        else
        {
            GUILayout.Label("Waiting", EditorStyles.boldLabel);
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
        findTimer?.Stop();
        findTimer?.Dispose();
        findTimer = null;
        
        searcher?.StopReceiving();
        responder?.StopListening();
        searcher = null;
        responder = null;
        receiveMessage = "";
    }
}
