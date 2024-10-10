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
    
    public void SendBroadcast(int port, string message)
    {
        SendTo(port, message, IPAddress.Broadcast.ToString());
    }
    
    public void SendTo(int port, string message, string targetIP)
    {
        UdpClient client = new UdpClient();
        IPEndPoint ip = new IPEndPoint(IPAddress.Parse(targetIP), port);
        byte[] bytes = Encoding.ASCII.GetBytes(message);
        client.Send(bytes, bytes.Length, ip);
        client.Close();
    }
    
    private bool isReceiving = false;
    private Thread receiveThread;

    public void StartReceiving(int port, Action<DeviceData> onReceiveData)
    {
        isReceiving = true;
        receiveThread = new Thread(() =>
        {
            UdpClient client = new UdpClient(port);
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, port);

            while (isReceiving)
            {
                byte[] bytes = client.Receive(ref ip);
                string jsonString = Encoding.ASCII.GetString(bytes);
                DeviceData data = new DeviceData(deviceName, GetLocalIPAddress(), ip.Address.ToString());
                Debug.Log($"Received message: {jsonString} from {deviceName} {GetLocalIPAddress()} sender {ip.Address}");
                onReceiveData?.Invoke(data);
            }

            client.Close();
        });

        receiveThread.Start();
    }

    public void StopReceiving()
    {
        isReceiving = false;
        if (receiveThread != null)
        {
//            receiveThread.Join(); // Wait for the thread to finish
            receiveThread = null;
        }
    }
    
    public void Ack(int rcvPort, int sndPort)
    {
        StartReceiving(rcvPort, data =>
        {
            SendTo(sndPort, data.ToJson(), data.IPAddress);
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
