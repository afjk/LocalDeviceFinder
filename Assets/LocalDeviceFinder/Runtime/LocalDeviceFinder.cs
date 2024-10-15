using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class LocalDeviceFinder
{
    public void SendBroadcast(int port)
    {
        SendTo(port, new byte[0], IPAddress.Broadcast.ToString());
    }
    
    public void SendTo(int port, byte[] message, string targetIP)
    {
        UdpClient client = new UdpClient();
        client.MulticastLoopback = false;
        IPEndPoint ip = new IPEndPoint(IPAddress.Parse(targetIP), port);
        byte[] bytes = message;
        client.Send(bytes, bytes.Length, ip);
        client.Close();
    }
    
    private bool isReceiving = false;
    private Thread receiveThread;

    private UdpClient rcClient;
    public void StartReceiving(int port, Action<ReceiveData, string> onReceiveData)
    {
        StartClient(port, (receiveData, ipAddress) =>
        {
            ReceiveData data = new ReceiveData(receiveData);
            if(data == null) return;
            onReceiveData?.Invoke(data, ipAddress);
        });
    }
    
    public void StartClient(int port, Action<byte[], string> onReceiveData)
    {
        if (isReceiving)
        {
            Debug.Log("Already receiving data.");
            return;
        }
        
        isReceiving = true;
        receiveThread = new Thread(() =>
        {
            rcClient = new UdpClient(port);
            rcClient.EnableBroadcast = true;
            rcClient.MulticastLoopback = false;
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, port);

            while (isReceiving)
            {
                try
                {
                    byte[] bytes = rcClient.Receive(ref ip);
                    if (ip.Address.ToString() == GetLocalIPAddress())
                    {
                        Debug.Log($"{ip.Address.ToString()} is local IP. Skip.");
                        continue;
                    }
                    onReceiveData?.Invoke(bytes,ip.Address.ToString());
                }
                catch (SocketException e)
                {
                    Debug.Log(e);
                }
            }

            rcClient.Close();
            Debug.Log($"Close port:{port}");
        });

        receiveThread.Start();
    }
    
    public void StopReceiving()
    {
        isReceiving = false;
        if (receiveThread != null)
        {
            receiveThread = null;
        }
        rcClient.Close();
    }
    
    public void Ack(int rcvPort, int sndPort)
    {
        var deviceName = SystemInfo.deviceName;
        StartClient(rcvPort, (message,ipAddress) =>
        {
            Debug.Log($"Received message from {ipAddress}: {message}");
            var rcvData = new ReceiveData( deviceName );
            SendTo(sndPort, rcvData.Serialize(), ipAddress);
        });
    }

    public string GetLocalIPAddress()
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
