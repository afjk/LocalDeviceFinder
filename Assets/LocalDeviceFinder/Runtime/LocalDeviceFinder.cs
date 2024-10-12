using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class LocalDeviceFinder
{
    private string deviceName;
    
    public void Initialize()
    {
        deviceName = SystemInfo.deviceName;
    }
    
    public void SendBroadcast(int port)
    {
        SendTo(port, "", IPAddress.Broadcast.ToString());
    }
    
    public void SendTo(int port, string message, string targetIP)
    {
        UdpClient client = new UdpClient();
        client.MulticastLoopback = false;
        IPEndPoint ip = new IPEndPoint(IPAddress.Parse(targetIP), port);
        byte[] bytes = Encoding.ASCII.GetBytes(message);
        client.Send(bytes, bytes.Length, ip);
        client.Close();
    }
    
    private bool isReceiving = false;
    private Thread receiveThread;

    private UdpClient rcClient;
    public void StartReceiving(int port, Action<ReceiveData> onReceiveData)
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
                byte[] bytes = rcClient.Receive(ref ip);
                string message = Encoding.ASCII.GetString(bytes);
                ReceiveData data = new ReceiveData(deviceName, GetLocalIPAddress(), ip.Address.ToString(), message);
                onReceiveData?.Invoke(data);
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
        StartReceiving(rcvPort, data =>
        {
            Debug.Log($"Received message from {data.DeviceName} {data.FromIPAddress}: {data.Message}");
            var rcvData = new ReceiveData(deviceName, GetLocalIPAddress(), data.FromIPAddress, "ack");
            SendTo(sndPort, rcvData.ToJson(), data.FromIPAddress);
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
