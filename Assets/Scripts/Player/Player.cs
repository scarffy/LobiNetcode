using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Unity.Collections;

public class Player : NetworkBehaviour
{
    public NetworkVariable<NetworkString> playerName = new NetworkVariable<NetworkString>();
    public NetworkVariable<int> playerRoles = new NetworkVariable<int>(0);

    [Space]
    public GameObject[] uiObjects;

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
    }

    #region NetworkVariableLogics
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.K) && IsLocalPlayer)
        {
            //playerRoles.Value++;
        }
    }

    void valueChanged(int prevI,int newI)
    {
        //Debug.Log(newI);

        if(IsOwner && IsClient)
        {
            // Update UI code
            //! To do
            //! Transfer from line 69 to 82 in ClientUI.cs and see what it does.
            //! If it doesn't work then put it in UpdateRoleValue function
        }
    }

    /// <summary>
    /// UI Button can call this to debug. Default reset value is 0
    /// </summary>
    /// <param name="value"></param>
    public void UpdateRoleValue(int value = 1)
    {
        if (!IsServer) return;

        if (value != 0)
            playerRoles.Value++;
        else
            playerRoles.Value = value;
    }

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
    [ServerRpc(RequireOwnership = false)]
    public void SpawnServerRpc()
    {
        //! Calling from client to server

        //! Object need to turned on
        GameObject go = Instantiate(
            NetworkController.Instance.clientUI,
            NetworkController.Instance.canvasParent
            );
        //go.transform.parent = NetworkController.Instance.canvasParent;

        go.GetComponent<NetworkObject>().Spawn();
        //! How do I call this from the host+server at start?
        go.GetComponent<ClientUI>().SetupServerRpc(OwnerClientId);
        go.GetComponent<ClientUI>().player = this;

        //! This is da wey
        if (!IsServer) return;
        CallingClientRpc();
    }

    [ClientRpc]
    public void CallingClientRpc()
    {
        //! Calling from server to client

        if (!IsServer) return;

        //! Tukar mak ayah
        uiObjects = GameObject.FindGameObjectsWithTag("Finish");

        for (int i = 0; i < uiObjects.Length; i++)
        {
            //uiObjects[i].name = "Host UI";

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