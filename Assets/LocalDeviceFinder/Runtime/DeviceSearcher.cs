using System;
using System.Net;

public class DeviceSearcher
{
    private IReceiveDataFactory receiveDataFactory;
    private UdpCommunicator communicator;
    private int receivePort;

    public DeviceSearcher(IReceiveDataFactory receiveDataFactory, int receivePort)
    {
        this.receiveDataFactory = receiveDataFactory;
        this.receivePort = receivePort;
        this.communicator = new UdpCommunicator();
    }

    public void SendBroadcast(int port)
    {
        communicator.Send(new byte[0], IPAddress.Broadcast.ToString(), port);
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