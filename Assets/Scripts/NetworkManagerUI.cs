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
    [SerializeField] private GameObject gameCode;
    [SerializeField] private GameObject connectionBtns;
    [SerializeField] private GameObject joinPanel;
    [SerializeField] private Button joinBtn;
    private string ip;

    private void Awake() {
        string localIP = GetLocalIPAddress();
        string encodedIp = EncodeIP(localIP);
        Debug.Log("Encoded IP: " + encodedIp);
        Debug.Log("Decoded IP: " + DecodeHexToIP(encodedIp));

        

        string encodedIP2 = EncodeIPToBase36(localIP);
        Debug.Log("Encoded IP36: " + encodedIP2);

        string decodedIP2 = DecodeBase36ToIP(encodedIP2);
        Debug.Log("Decoded IP: " + decodedIP2);

        serverBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartServer();
            gameCode.SetActive(true);
            gameCode.GetComponent<TextMeshProUGUI>().text = "Game Code: " + encodedIP2;
            connectionBtns.SetActive(false);
        });

        clientBtn.onClick.AddListener(() => {
            connectionBtns.SetActive(false);
            joinPanel.SetActive(true);
        });

        joinBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData("192.168.0.101", (ushort)1234, "0.0.0.0");

            NetworkManager.Singleton.StartClient();
            connectionBtns.SetActive(false);
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