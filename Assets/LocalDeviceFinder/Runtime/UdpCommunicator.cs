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

    public void Send(byte[] message, string targetIP, int port)
    {
        using (UdpClient client = new UdpClient())
        {
            client.EnableBroadcast = true;
            client.MulticastLoopback = false;
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(targetIP), port);
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
                udpClient = new UdpClient(port);
                udpClient.EnableBroadcast = true;
                udpClient.MulticastLoopback = false;
                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, port);

                while (isReceiving)
                {
                    try
                    {
                        byte[] bytes = udpClient.Receive(ref ipEndPoint);
                        string senderIP = ipEndPoint.Address.ToString();
                        if (senderIP == GetLocalIPAddress())
                        {
                            continue; // 自分自身からのメッセージは無視
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
