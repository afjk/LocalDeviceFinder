using System;
using UnityEngine;
using UnityEngine.UI;


namespace com.afjk.LocalDeviceFinder.sample
{
    public class LocalDeviceFinderClient : MonoBehaviour
    {
        [SerializeField] private int sndPort = 8081;
        [SerializeField] private int rcvPort = 8080;
        [SerializeField] private Text receiveText; // 受信データを表示するText要素

        DeviceResponder responder;

        private string receiveMessage = "Receiving...";
        // Start is called before the first frame update
        void Start()
        {
            responder = new DeviceResponder(new ReceiveDataFactory(), rcvPort, sndPort);
            responder.StartListening((bytes, ipAddress) =>
            {
                string message = System.Text.Encoding.Unicode.GetString(bytes);
                receiveMessage = $"Received: {message} from {ipAddress}";
            });
        }

        private void Update()
        {
            receiveText.text = receiveMessage;
        }

        private void OnDestroy()
        {
            responder?.StopListening();
            responder = null;
        }
    }
}