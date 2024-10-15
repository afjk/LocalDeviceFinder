using UnityEngine;

public class LocalDeviceFinderClient : MonoBehaviour
{
    [SerializeField] private int sndPort = 8081;
    [SerializeField] private int rcvPort = 8080;

    private LocalDeviceFinder finder = new LocalDeviceFinder();
    
    // Start is called before the first frame update
    void Start()
    {
        finder.Ack(rcvPort, sndPort);
    }
}
