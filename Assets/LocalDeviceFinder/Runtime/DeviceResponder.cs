public class DeviceResponder
{
    private IReceiveDataFactory receiveDataFactory;
    private UdpCommunicator communicator;
    private int listenPort;
    private int responsePort;
    private string multicastIP;

    public DeviceResponder(IReceiveDataFactory receiveDataFactory, int listenPort, int responsePort, string multicastIP = null)
    {
        this.receiveDataFactory = receiveDataFactory;
        this.listenPort = listenPort;
        this.responsePort = responsePort;
        this.multicastIP = multicastIP;
        this.communicator = new UdpCommunicator(multicastIP);
    }

    public void StartListening()
    {
        communicator.StartReceiving(listenPort, (bytes, ipAddress) =>
        {
            // 応答メッセージを送信
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