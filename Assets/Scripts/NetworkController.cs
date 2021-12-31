using UnityEngine;
using Unity.Netcode;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode.Transports.UNET;

public class NetworkController : NetworkBehaviour
{
    public static NetworkController Instance;

    public string ipAddress = "127.0.0.1";

    [Space]
    public UNetTransport unetTransport;

    [Space]
    public bool isServer = false;

    [Header("Object to spawn")]
    public Transform canvasParent;
    public GameObject hostUI;
    public GameObject clientUI;

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        NetworkManager.Singleton.OnServerStarted += IsConnectedServer;
        NetworkManager.Singleton.OnClientConnectedCallback += IsConnectedCallback;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HostSession()
    {
        unetTransport.ConnectAddress = ipAddress;
        NetworkManager.Singleton.StartHost();
    }

    public void JoinSession()
    {
        unetTransport.ConnectAddress = ipAddress;
        NetworkManager.Singleton.StartClient();
    }

    /// <summary>
    /// This project function. Call only on server side
    /// </summary>
    public void IsConnectedServer()
    {
        //Debug.Log("Connected to server");
        UIManager.Instance.connectionStatusEnum = UIManager.StatusEnum.Connected;
        UIManager.Instance.ConnectionStatus();
        isServer = true;
    }

    /// <summary>
    /// Will call on server and local client
    /// </summary>
    /// <param name="value"></param>
    void IsConnectedCallback(ulong value)
    {
        if (isServer)
        {
            DebuggerManager.Instance.DebugMsg("Is server");
            return;
        }

        DebuggerManager.Instance.DebugMsg("Is not server");
        NetworkLog.LogInfoServer($"Client {value} has connected");
    }
}
