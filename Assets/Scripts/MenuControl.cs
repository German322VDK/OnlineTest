using Networking;
using System;
using System.Text.RegularExpressions;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEditor.PackageManager;
using UnityEngine;

public class MenuControl : MonoBehaviour
{
    [SerializeField] private TMP_Text ipAddressText;
    [SerializeField] private TMP_Text port;
    private string lobbySceneName = "Game";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        var uptTransport = (UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;

        var portNum = ushort.Parse(port.text.Remove(port.text.Length - 1));

        if (uptTransport)
        {
            uptTransport.SetConnectionData(Sanitize(ipAddressText.text), portNum);
        }

        
        if (NetworkManager.Singleton.StartHost())
        {
            SceneTranspositionHandler.SceneTranspositionHandlerInst.RegisterCallBacks();
            SceneTranspositionHandler.SceneTranspositionHandlerInst.SwitchScene(lobbySceneName);
        }
        else
        {
            Debug.LogError("Failed to start host");
        }
    }

    public void JoinGame()
    {
        var uptTransport = (UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
        var portNum = ushort.Parse(port.text.Remove(port.text.Length - 1));

        if (uptTransport)
        {
            uptTransport.SetConnectionData(Sanitize(ipAddressText.text), portNum);
        }

        if (!NetworkManager.Singleton.StartClient())
        {
            Debug.LogError("Failed to join");
        }
    }

    public void PintCLiendCount()
    {
        var clients = NetworkManager.Singleton.ConnectedClientsList;
        
        print(clients.Count);

        foreach (var client in clients)
        {
            print($"{client.ClientId}");
        }
    }

    private string Sanitize(string dirtyStr) =>
        Regex.Replace(dirtyStr, "[^A-Za-z0-9.]", "");
}
