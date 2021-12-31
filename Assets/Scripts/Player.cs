using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Unity.Collections;

public class Player : NetworkBehaviour
{
    public NetworkVariable<NetworkString> playerName = new NetworkVariable<NetworkString>();
    public NetworkVariable<int> playerRoles = new NetworkVariable<int>(0);

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
                //NetworkController.Instance.SpawnClientRpc();
                //CallingServerRpc();
                //LobbyManager.Instance.CallingClientRpc();
                //LobbyManager.Instance.CallingServerRpc();
                //Debug.Log("OnNetworkSpawn isOwner and isClient");
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

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.K) && IsLocalPlayer)
        {
            playerRoles.Value++;
        }
    }

    void valueChanged(int prevI,int newI)
    {
        //Debug.Log(newI);

        if(IsOwner && IsClient)
        {
            // Update UI code
        }
    }

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

    [ServerRpc(RequireOwnership = false)]
    public void SpawnServerRpc()
    {
        //! Calling from client to server

        //! Object need to turned on
        GameObject go = Instantiate(NetworkController.Instance.hostUI, NetworkController.Instance.canvasParent);
        go.transform.parent = NetworkController.Instance.canvasParent;

        go.GetComponent<NetworkObject>().Spawn();
        ulong itemNetId = go.GetComponent<NetworkObject>().NetworkObjectId;
        NetworkObject netObj = go.GetComponent<NetworkObject>();

        //! Not da wey
        CallingClientRpc();
    }

    //[Space]
    public GameObject[] uiObjects;

    [ClientRpc]
    public void CallingClientRpc()
    {
        //! Calling from server to client

        if (!IsServer) return;
        //! Even calling from server pon tak boleh... kenapa?
        uiObjects = GameObject.FindGameObjectsWithTag("Finish");

        for (int i = 0; i < uiObjects.Length; i++)
        {
            uiObjects[i].name = "Host UI";
            //! Cannot call from host because not a server... Which is weird because Host also is a server
            uiObjects[i].transform.SetParent(null);
            uiObjects[i].transform.SetParent(NetworkController.Instance.canvasParent);
            //uiObjects[i].transform.parent = NetworkController.Instance.canvasParent;
        }
    }

    // How about do thing non networking way? Cannot.. Not like photon..
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