using UnityEngine;

public class LocalDeviceFinderClient : MonoBehaviour
{
    [SerializeField] private int sndPort = 8080;
    [SerializeField] private int rcvPort = 8080;

    DeviceResponder responder;
    
    // Start is called before the first frame update
    void Start()
    {
        responder = new DeviceResponder(new ReceiveDataFactory(), rcvPort, sndPort);
        responder.StartListening();
    }
}
