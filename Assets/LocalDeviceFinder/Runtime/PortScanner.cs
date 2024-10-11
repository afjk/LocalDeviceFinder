using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class PortScanner
{
    public string LocalIPAddress { get; private set; }
    public List<string> DiscoveredDevices { get; private set; }
    public int PortToScan { get; private set; }
    private SemaphoreSlim semaphore;

    public PortScanner(int portToScan)
    {
        LocalIPAddress = GetLocalIPAddress();
        DiscoveredDevices = new List<string>();
        PortToScan = portToScan;
        semaphore = new SemaphoreSlim(100); // Adjust this number based on your system's limit
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

    public async Task ScanNetwork()
    {
        string baseIP = LocalIPAddress.Substring(0, LocalIPAddress.LastIndexOf(".") + 1);
        string subnetMask = GetSubnetMask(LocalIPAddress);
        var (start, end) = GetIPRangeFromSubnetMask(subnetMask);
        List<Task> tasks = new List<Task>();
        for (int i = start; i <= end; i++)
        {
            string ip = baseIP + i.ToString();
            tasks.Add(ScanIPWithPort(ip, PortToScan));
        }
        await Task.WhenAll(tasks);
    }
    private async Task ScanIPWithPort(string ip, int port)
    {
        await semaphore.WaitAsync();
        try
        {
            using (TcpClient tcpClient = new TcpClient())
            {
                try
                {
                    var connectTask = tcpClient.ConnectAsync(ip, port);
                    await Task.WhenAny(connectTask, Task.Delay(500)); // 500ミリ秒のタイムアウト

                    if (connectTask.IsCompleted && !connectTask.IsFaulted)
                    {
                        DiscoveredDevices.Add(ip);
                    }
                }
                catch
                {
                    // 接続に失敗した場合は何もしない
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            throw;
        }
        finally
        {
            semaphore.Release();
        }
    }
    
    public string GetSubnetMask(string ipAddress)
    {
        IPAddress[] addresses = Dns.GetHostAddresses(Dns.GetHostName());
        Debug.Log(Dns.GetHostName());
        Debug.Log(addresses);
        
        foreach (IPAddress address in addresses)
        {
            if (address.ToString() == ipAddress)
            {
                foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
                {
                    foreach (UnicastIPAddressInformation unicastIPAddressInformation in networkInterface.GetIPProperties().UnicastAddresses)
                    {
                        if (unicastIPAddressInformation.Address.ToString() == ipAddress)
                        {
                            Debug.Log($"subnet mask:{unicastIPAddressInformation.IPv4Mask.ToString()}");
                            return unicastIPAddressInformation.IPv4Mask.ToString();
                        }
                    }
                }
            }
        }
        throw new Exception("No subnet mask found for IP address " + ipAddress);
    }

    public (int start, int end) GetIPRangeFromSubnetMask(string subnetMask)
    {
        var maskParts = subnetMask.Split('.');
        var binaryStr = string.Join("", maskParts.Select(s => Convert.ToString(int.Parse(s), 2).PadLeft(8, '0')));
        var zeroIndex = binaryStr.IndexOf('0');
        if (zeroIndex < 0)
        {
            return (1, 254); // Default range for /24 subnet
        }
        var numberOfHosts = (int)Math.Pow(2, 32 - zeroIndex) - 2; // -2 for network and broadcast addresses
        Debug.Log($"GetIPRangeFromSubnetMask:1-{numberOfHosts}");
        return (1, numberOfHosts);
    }
}