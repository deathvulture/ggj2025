using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using System.Net;
using System.Net.Sockets;
using System;
using TMPro;

public class NetworkManagerUI : MonoBehaviour {
    [SerializeField] private Button serverBtn;
    [SerializeField] private Button clientBtn;
    [SerializeField] private Button hostBtn;
    [SerializeField] private GameObject gameCode;
    [SerializeField] private GameObject connectionBtns;
    [SerializeField] private GameObject joinPanel;
    [SerializeField] private Button joinBtn;
    [SerializeField] private TMP_InputField addressInput;
    [SerializeField] private GameObject gameCanvas;
    [SerializeField] private GameObject panel;

    private TextMeshProUGUI gameCodeText;
    private string ip;


    private void Awake() {


#if !UNITY_EDITOR && UNITY_WEBGL
      serverBtn.gameObject.SetActive(false);
      hostBtn.gameObject.SetActive(false);
#endif
        gameCodeText = gameCode.GetComponent<TextMeshProUGUI>();

#if !UNITY_WEBGL || UNITY_EDITOR
        string localIP = GetLocalIPAddress();
        string encodedIP = EncodeIPToBase36(localIP);
        Debug.Log("Encoded IP36: " + encodedIP);
        Debug.Log("Decoded IP: " + DecodeBase36ToIP(encodedIP));

        serverBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartServer();
            gameCode.SetActive(true);
            gameCodeText.text = "Game Code: " + encodedIP;
            connectionBtns.SetActive(false);
            panel.SetActive(false);
        });

        hostBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
            gameCode.SetActive(true);
            gameCodeText.text = "Game Code: " + encodedIP;
            connectionBtns.SetActive(false);
            gameCanvas.SetActive(true);
            panel.SetActive(false);
        });
#endif

        clientBtn.onClick.AddListener(() => {
            connectionBtns.SetActive(false);
            joinPanel.SetActive(true);
        });

        joinBtn.onClick.AddListener(() => {
            string decodedIp = DecodeBase36ToIP(addressInput.text);
            Debug.Log(addressInput.text == "1HGE0YT");
            Debug.Log($"encoded ip {addressInput.text}");
            Debug.Log($"Decoded ip {decodedIp}");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(decodedIp, (ushort)8080, "0.0.0.0");
            NetworkManager.Singleton.StartClient();
            joinPanel.SetActive(false);
            gameCanvas.SetActive(true);
            panel.SetActive(false);
        });
    }

    private string GetLocalIPAddress() {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList) {
            if (ip.AddressFamily == AddressFamily.InterNetwork) {
                return ip.ToString();
            }
        }
        throw new System.Exception("No network adapters with an IPv4 address in the system!");
    }

    private static string EncodeIP(string ipAddress) {

        IPAddress ip = IPAddress.Parse(ipAddress);
        byte[] bytes = ip.GetAddressBytes();
        string hexIP = BitConverter.ToString(bytes).Replace("-", "");

        return hexIP;
    }

    private static string DecodeHexToIP(string hexIP) {
        byte[] bytes = new byte[hexIP.Length / 2];
        for (int i = 0; i < hexIP.Length; i += 2) {
            bytes[i / 2] = Convert.ToByte(hexIP.Substring(i, 2), 16);
        }
        return new IPAddress(bytes).ToString();
    }


    private static string EncodeIPToBase36(string ipAddress) {
        IPAddress ip = IPAddress.Parse(ipAddress);
        byte[] bytes = ip.GetAddressBytes();
        long value = 0;

        // Combine bytes into a single long value
        for (int i = 0; i < bytes.Length; i++) {
            value |= (long)bytes[i] << (8 * (3 - i));
        }

        // Convert to Base36
        return ToBase36(value);
    }

    private static string DecodeBase36ToIP(string base36IP) {
        long value = FromBase36(base36IP);
        byte[] bytes = new byte[4];

        // Split the long value back into bytes
        for (int i = 0; i < 4; i++) {
            bytes[3 - i] = (byte)((value >> (8 * i)) & 0xFF);
        }

        return new IPAddress(bytes).ToString();
    }

    private static string ToBase36(long value) {
        const string charset = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        string result = string.Empty;
        while (value > 0) {
            result = charset[(int)(value % 36)] + result;
            value /= 36;
        }
        return result;
    }

    private static long FromBase36(string base36) {
        const string charset = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        long result = 0;
        foreach (char c in base36) {
            result = result * 36 + charset.IndexOf(c);
        }
        return result;

    }
}