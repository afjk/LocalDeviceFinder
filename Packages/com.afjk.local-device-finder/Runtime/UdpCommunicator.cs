using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

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
        using (UdpClient client = new UdpClient())
        {
            client.EnableBroadcast = targetIP == IPAddress.Broadcast.ToString();
            client.MulticastLoopback = true; // テスト目的で自分自身にも受信させる場合はtrue

            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(targetIP), port);

            if (multicastAddress != null && multicastAddress.ToString() == targetIP)
            {
                // マルチキャストの場合、マルチキャストアドレスを設定
                client.JoinMulticastGroup(multicastAddress);
            }

            client.Send(message, message.Length, ipEndPoint);
        }
    }

    public void StartReceiving(int port, Action<byte[], string> onReceiveData)
    {
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

                udpClient.MulticastLoopback = true; // テスト目的で自分自身にも受信させる場合はtrue

                IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

                while (isReceiving)
                {
                    try
                    {
                        byte[] bytes = udpClient.Receive(ref remoteEndPoint);
                        string senderIP = remoteEndPoint.Address.ToString();
                        
                        if (senderIP == GetLocalIPAddress())
                        {
                            continue;
                        }

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

    private string GetLocalIPAddress()
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
