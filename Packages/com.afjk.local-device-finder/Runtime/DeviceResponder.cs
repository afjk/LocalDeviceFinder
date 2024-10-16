using System;
using UnityEngine;

public class DeviceResponder
{
    private IReceiveDataFactory receiveDataFactory;
    private UdpCommunicator communicator;
    private int listenPort;
    private int responsePort;
    public string SearcherIpAddress { get; private set; }

    public DeviceResponder(IReceiveDataFactory receiveDataFactory, int listenPort, int responsePort, string multicastIP = null)
    {
        this.receiveDataFactory = receiveDataFactory;
        this.listenPort = listenPort;
        this.responsePort = responsePort;
        this.communicator = new UdpCommunicator(multicastIP);
    }

    public void StartListening(Action<byte[],string> onReceive = null)
    {
        communicator.StartReceiving(listenPort, (bytes, ipAddress) =>
        {
            SearcherIpAddress = ipAddress;
            SendResponse(ipAddress);
            onReceive?.Invoke(bytes, ipAddress);
        });
    }

    private void SendResponse(string targetIP)
    {
        Debug.Log($"Send response to {targetIP}");
        var responseData = receiveDataFactory.Create();
        byte[] message = responseData.Serialize();
        communicator.Send(message, targetIP, responsePort);
    }

    public void StopListening()
    {
        communicator.StopReceiving();
    }
}