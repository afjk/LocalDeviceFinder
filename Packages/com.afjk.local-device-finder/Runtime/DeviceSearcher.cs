using System;
using System.Net;
using UnityEngine;

namespace com.afjk.LocalDeviceFinder
{
    public class DeviceSearcher
    {
        private IReceiveDataFactory receiveDataFactory;
        private UdpCommunicator communicator;
        private int receivePort;
        private string multicastIP;

        public DeviceSearcher(IReceiveDataFactory receiveDataFactory, int receivePort, string multicastIP = null)
        {
            this.receiveDataFactory = receiveDataFactory;
            this.receivePort = receivePort;
            this.multicastIP = multicastIP;
            this.communicator = new UdpCommunicator(multicastIP);
        }

        public void SendUnicast(byte[] data, string ipAddress, int port)
        {
            communicator.Send(data, ipAddress, port);
        }

        public void SendBroadcast(int port)
        {
            communicator.Send(new byte[0], IPAddress.Broadcast.ToString(), port);
        }

        public void SendMulticast(int port)
        {
            if (string.IsNullOrEmpty(multicastIP))
            {
                Debug.LogError("Multicast IP is not set.");
                return;
            }

            communicator.Send(new byte[0], multicastIP, port);
        }

        public void StartReceiving(Action<IReceiveData, string> onReceiveData)
        {
            communicator.StartReceiving(receivePort, (bytes, ipAddress) =>
            {
                IReceiveData data = receiveDataFactory.Create().Deserialize(bytes);
                if (data != null)
                {
                    onReceiveData?.Invoke(data, ipAddress);
                }
            });
        }

        public void StopReceiving()
        {
            communicator.StopReceiving();
        }
    }
}