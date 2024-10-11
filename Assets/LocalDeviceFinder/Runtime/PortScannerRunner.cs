using UnityEngine;
using System.Threading.Tasks;

public class PortScannerRunner : MonoBehaviour
{
    [SerializeField]
    int portToScan = 80; // スキャンしたいポート番号を指定します

    private PortScanner scanner;

    async void Start()
    {
        scanner = new PortScanner(portToScan);
        await scanner.ScanNetwork();

        // スキャンが完了したら、発見したデバイスのIPアドレスを表示します
        foreach (string ip in scanner.DiscoveredDevices)
        {
            Debug.Log($"Found device at {ip}");
        }
    }
}