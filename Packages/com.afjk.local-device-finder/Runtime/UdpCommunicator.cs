using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace com.afjk.LocalDeviceFinder
{
    public class UdpCommunicator
    {
        private UdpClient udpClient;
        private Thread receiveThread;
        private bool isReceiving = false;
        private readonly object lockObject = new object();

        // マルチキャストアドレス（デフォルトはnull）
        private IPAddress multicastAddress;

        public UdpCommunicator()
        {
        }

        // マルチキャストアドレスを設定するコンストラクタ
        public UdpCommunicator(string multicastIP)
        {
            if (!string.IsNullOrEmpty(multicastIP))
            {
                multicastAddress = IPAddress.Parse(multicastIP);
            }
        }

        public void Send(byte[] message, string targetIP, int port)
        {
            Debug.Log($"Send message to {targetIP} on port: {port}");
            using (UdpClient client = new UdpClient())
            {
                client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                client.EnableBroadcast = targetIP == IPAddress.Broadcast.ToString();
                client.MulticastLoopback = true;

                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(targetIP), port);

                if (multicastAddress != null && multicastAddress.ToString() == targetIP)
                {
                    client.JoinMulticastGroup(multicastAddress);
                }

                client.Send(message, message.Length, ipEndPoint);
            }
        }

        public void StartReceiving(int port, Action<byte[], string> onReceiveData)
        {
            Debug.Log($"StartReceiving on port: {port}");
            lock (lockObject)
            {
                if (isReceiving)
                {
                    Debug.Log("Already receiving data.");
                    return;
                }

                isReceiving = true;
                receiveThread = new Thread(() =>
                {
                    udpClient = new UdpClient();
                    udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

                    IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, port);
                    udpClient.Client.Bind(localEndPoint);

                    if (multicastAddress != null)
                    {
                        // マルチキャストグループに参加
                        udpClient.JoinMulticastGroup(multicastAddress);
                    }
                    else
                    {
                        // ブロードキャスト受信を有効化
                        udpClient.EnableBroadcast = true;
                    }

                    udpClient.MulticastLoopback = true;

                    IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

                    while (isReceiving)
                    {
                        try
                        {
                            byte[] bytes = udpClient.Receive(ref remoteEndPoint);
                            string senderIP = remoteEndPoint.Address.ToString();

                            onReceiveData?.Invoke(bytes, senderIP);
                        }
                        catch (SocketException e)
                        {
                            Debug.Log(e);
                        }
                    }

                    udpClient.Close();
                    Debug.Log($"Stopped receiving on port: {port}");
                });

                receiveThread.Start();
            }
        }

        public void StopReceiving()
        {
            lock (lockObject)
            {
                isReceiving = false;
                udpClient?.Close();
                receiveThread = null;
            }
        }

        public static string GetLocalIPAddress()
        {
            // 既存の実装を使用
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }

            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
    }
}