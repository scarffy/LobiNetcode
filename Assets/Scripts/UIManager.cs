using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public enum StatusEnum
    {
        Empty,
        NotConnected,
        Connecting,
        Connected
    }
    public StatusEnum connectionStatusEnum = StatusEnum.NotConnected;
    
    [Header("Starter")]
    public static UIManager Instance;

    public GameObject canvasStarter;
    public TMP_InputField ipInput;

    public TextMeshProUGUI statusText;

    [Header("Connected")]
    public GameObject canvasConnected;
    public TextMeshProUGUI statusConnectedText;

    [Header("Connected Host Only")]
    public GameObject panelUsersConnected;

    // Start is called before the first frame update
    void Start()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Button_StartHost()
    {
        if (!string.IsNullOrEmpty(ipInput.text))
        {
            connectionStatusEnum = StatusEnum.Connecting;  
        }
        else
        {

            connectionStatusEnum = StatusEnum.Empty;
        }
        NetworkController.Instance.HostSession();
        ConnectionStatus();
    }

    public void Button_StartClient()
    {
        Unity.Netcode.NetworkManager.Singleton.OnClientConnectedCallback += ConnectionStatus;

        if (!string.IsNullOrEmpty(ipInput.text))
        {
            connectionStatusEnum = StatusEnum.Connecting;
        }
        else
        {
            connectionStatusEnum = StatusEnum.Empty;
        }
        NetworkController.Instance.JoinSession();
        ConnectionStatus();
    }

    /// <summary>
    /// Only run on server and local client
    /// </summary>
    /// <param name="value"></param>
    public void ConnectionStatus(ulong value)
    {
        if (NetworkController.Instance.isServer)
            return;

        connectionStatusEnum = StatusEnum.Connected;
        ConnectionStatus();
    }

    public void ConnectionStatus()
    {
        switch (connectionStatusEnum)
        {
            case StatusEnum.Empty:
                statusText.SetText("Trying to connect");
                break;
            case StatusEnum.Connecting:
                statusText.SetText("Connecting");
                break;
            case StatusEnum.Connected:
                canvasStarter.SetActive(false);
                canvasConnected.SetActive(true);
                statusConnectedText.SetText($"Connected to {NetworkController.Instance.ipAddress}");
                if (NetworkController.Instance.isServer) panelUsersConnected.SetActive(true);
                break;
                
        }
    }
}
