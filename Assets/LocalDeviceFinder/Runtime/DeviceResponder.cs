public class DeviceResponder
{
    private IReceiveDataFactory receiveDataFactory;
    private UdpCommunicator communicator;
    private int listenPort;
    private int responsePort;

    public DeviceResponder(IReceiveDataFactory receiveDataFactory, int listenPort, int responsePort)
    {
        this.receiveDataFactory = receiveDataFactory;
        this.listenPort = listenPort;
        this.responsePort = responsePort;
        this.communicator = new UdpCommunicator();
    }

    public void StartListening()
    {
        communicator.StartReceiving(listenPort, (bytes, ipAddress) =>
        {
            // 受信したブロードキャストメッセージに対して応答を送信
            SendResponse(ipAddress);
        });
    }

    private void SendResponse(string targetIP)
    {
        var responseData = receiveDataFactory.Create();
        byte[] message = responseData.Serialize();
        communicator.Send(message, targetIP, responsePort);
    }

    public void StopListening()
    {
        communicator.StopReceiving();
    }
}