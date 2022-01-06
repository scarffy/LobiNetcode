using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Unity.Collections;
using TMPro;

public class Player : NetworkBehaviour
{
    public NetworkVariable<NetworkString> playerName = new NetworkVariable<NetworkString>();
    public NetworkVariable<int> playerRoles = new NetworkVariable<int>(0);

    [Space]
    public GameObject go;
    public GameObject[] uiObjects;

    [Space]
    public ClientUI clientUI;
    public NetworkObject netObject;

    /// <summary>
    /// This get called everytime
    /// </summary>
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            playerName.Value = $"Player {OwnerClientId}";
            gameObject.name = $"Player {OwnerClientId}";

            if (IsOwner && IsClient)
            {
                gameObject.name = $"Local player {OwnerClientId}";
            }
        }
        else
        {
            gameObject.name = $"Player {OwnerClientId}";

            if (IsOwner && IsLocalPlayer)
            {
                gameObject.name = $"Local player {OwnerClientId}";
                //NetworkController.Instance.SpawnServerRpc();
                
            }
        }


        playerRoles.OnValueChanged += valueChanged;

        if (IsLocalPlayer)
        {
            Debug.Log("OnNetworkSpawn isLocalPlayer");
            SpawnServerRpc();
        }
        else if(!IsLocalPlayer && IsServer){
            Debug.Log("OnNetworkSpawn !IsLocalPlayer");
        }
    }

    #region NetworkVariableLogics
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.K) && IsLocalPlayer)
        {
            //playerRoles.Value++;
        }
    }

    public TextMeshProUGUI roleName;

    void valueChanged(int prevI,int newI)
    {
        //Debug.Log(newI);

        if(IsOwner && IsClient)
        {
            // Update UI code
            //! To do
            //! Transfer from line 69 to 82 in ClientUI.cs and see what it does.
            //! If it doesn't work then put it in UpdateRoleValue function
            if (roleName == null)
                roleName = GameObject.Find("Something").GetComponent<TextMeshProUGUI>();


            Debug.Log($"Setup Role {newI}");

            switch (newI)
            {
                case 0:
                    roleName.text = "None";
                    break;
                case 1:
                    roleName.text = "Trainer";
                    break;
                case 2:
                    roleName.text = "Trainee";
                    break;
                case 3:
                    roleName.text = "Observer";
                    break;
            }
        }
    }

    /// <summary>
    /// UI Button can call this to debug. Default reset value is 0
    /// </summary>
    /// <param name="value"></param>
    public void UpdateRoleValue(int value = 1)
    {
        if (!IsServer) return;

        //if (value != 0)
        //    playerRoles.Value++;
        //else
        //    playerRoles.Value = value;
        playerRoles.Value = value;
    }

    //! Calling from client to server
    [ServerRpc]
    public void CallingServerRpc()
    {
        //Debug.Log("Calling 'Update Role Value' from client to server");

        //GameObject go = Instantiate(NetworkController.Instance.hostUI, NetworkController.Instance.canvasParent);
        //go.GetComponent<NetworkObject>().Spawn();
        UpdateRoleValue();
    }
    #endregion

    #region SetupPlayerObject
    /// <summary>
    /// Calling from client to server
    /// </summary>
    [ServerRpc(RequireOwnership = false)]
    public void SpawnServerRpc()
    {
        //! Object need to turned on
        go = Instantiate(
            NetworkController.Instance.clientUI,
            NetworkController.Instance.canvasParent
            );

        //go.name = "Local UI clone";
        if (IsLocalPlayer)
            go.name = "Local UI";
        else
            go.name = $"Client UI {OwnerClientId}";
        
        netObject = go.GetComponent<NetworkObject>();
        netObject.Spawn();

        clientUI = go.GetComponent<ClientUI>();
        roleName = clientUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        if(IsLocalPlayer)
            roleName.gameObject.name = "Local UI";

        //! How do I call this from the host+server at start?
        clientUI.SetupServerRpc(OwnerClientId);
        //! This work for local but not for remote

        clientUI.TestServerRpc(go);
        clientUI.TestGiveServerRpc(gameObject);

        //! This is da wey
        //if (!IsServer) return;
        CallingClientRpc();

        SpawnedClientRpc(go);
    }

    /// <summary>
    /// Calling from server to client
    /// </summary>
    /// <param name="target"></param>
    [ClientRpc]
    public void SpawnedClientRpc(NetworkObjectReference target)
    {
        go = target;
        clientUI = go.GetComponent<ClientUI>();
        roleName = clientUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        if (IsLocalPlayer)
            roleName.gameObject.name = "Local UI";
    }

    /// <summary>
    /// Calling from server to client
    /// </summary>
    [ClientRpc]
    public void CallingClientRpc()
    {
        //if (!IsServer) return;

        //! If client doesn't sync properly, it might be tag is not set correctly
        uiObjects = GameObject.FindGameObjectsWithTag("Finish");

        for (int i = 0; i < uiObjects.Length; i++)
        {
            uiObjects[i].transform.SetParent(NetworkController.Instance.dummyParent);
            uiObjects[i].transform.SetParent(NetworkController.Instance.canvasParent);
        }


    }
    #endregion
}

public struct NetworkString : INetworkSerializable
{
    public FixedString32Bytes info;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref info);
    }

    public override string ToString()
    {
        return info.ToString();
    }

    public static implicit operator string(NetworkString s) => s.ToString();
    public static implicit operator NetworkString(string s) => new NetworkString() { info = new FixedString32Bytes(s) };
}