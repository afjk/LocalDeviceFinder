using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;


namespace com.afjk.LocalDeviceFinder.sample
{
    public class LocalDeviceResponder : MonoBehaviour
    {
        [SerializeField]
        private int listenPort = 8080;
        [SerializeField] 
        private int responsePort = 8081;
        [SerializeField] private Text receiveText; // 受信データを表示するText要素

        DeviceResponder responder;
        private bool needUpdateDisplay=false;

        private string receiveMessage = "Receiving...";
        // Start is called before the first frame update
        void Start()
        {
            responder = new DeviceResponder(new ReceiveDataFactory(), listenPort, responsePort);
            responder.StartListening((bytes, ipAddress) =>
            {
                needUpdateDisplay = true;
                string message = System.Text.Encoding.Unicode.GetString(bytes);
                receiveMessage = $"Received: {message} from {ipAddress}";
            });
        }

        private void Update()
        {
            if (!needUpdateDisplay) { return;}
            needUpdateDisplay = false;

            receiveText.text = receiveMessage;
        }

        private void OnDestroy()
        {
            responder?.StopListening();
            responder = null;
        }
    }
}