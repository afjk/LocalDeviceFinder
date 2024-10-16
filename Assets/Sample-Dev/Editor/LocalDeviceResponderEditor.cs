using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace com.afjk.LocalDeviceFinder.sample
{
public class LocalDeviceResponderEditor : EditorWindow
    {
        private int sndPort = 8081;
        private int rcvPort = 8080;
        private DeviceSearcher sender;
        private DeviceResponder responder;
        private List<DeviceData> deviceList = new(); // List to hold the devices
        private bool useMulticast = false;
        private string multicastIP = "239.0.0.222"; // デフォルトのマルチキャストアドレス
        private string currentState = "";
        private string receiveMessage = "";

        [MenuItem("Tools/Local Device Finder/Responder")]
        public static void ShowWindow()
        {
            GetWindow<LocalDeviceResponderEditor>("Local Device Finder");
        }

        private void OnGUI()
        {
            GUILayout.Label("Responder", EditorStyles.boldLabel);

            // ポート設定
            rcvPort = EditorGUILayout.IntField("Listen Port", rcvPort);
            sndPort = EditorGUILayout.IntField("Response Port", sndPort);

            bool newUseMulticast = EditorGUILayout.Toggle("Use Multicast", useMulticast);

            if (newUseMulticast)
            {
                multicastIP = EditorGUILayout.TextField("Multicast IP Address", multicastIP);
            }

            if (newUseMulticast != useMulticast)
            {
                StopAll();
                useMulticast = newUseMulticast;
            }

            if (GUILayout.Button("Start Responder"))
            {
                deviceList.Clear();
                StopAll();
                responder = new DeviceResponder(new ReceiveDataFactory(), rcvPort, sndPort,
                    useMulticast ? multicastIP : null);
                responder.StartListening(((bytes, ipAddress) =>
                {
                    var message = Encoding.Unicode.GetString(bytes);
                    if (string.IsNullOrEmpty(message)) return;
                    receiveMessage = $"{message} from {ipAddress}";
                    Debug.Log(receiveMessage);
                    UnityEditor.EditorApplication.delayCall += Repaint;
                }));
                Debug.Log("Responder started");

                currentState = "Responder Running";
            }

            if (GUILayout.Button("Stop"))
            {
                StopAll();
                deviceList.Clear();
                Debug.Log("Responder stopped");

                currentState = "Stop";
            }

            // Display the list of devices or the receiving state based on the current state
            if (currentState == "Responder Running")
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

        private void StopAll()
        {
            sender?.StopReceiving();
            responder?.StopListening();
            sender = null;
            responder = null;
            receiveMessage = "";
        }
        
        private void OnDestroy()
        {
            StopAll();
        }
    }
}