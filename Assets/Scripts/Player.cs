using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Unity.Collections;

public class Player : NetworkBehaviour
{
    //! To try.. Network get NetworkVariable?? then what??

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
        Debug.Log(newI);

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
        GameObject go = Instantiate(NetworkController.Instance.hostUI, NetworkController.Instance.canvasParent);
        go.GetComponent<NetworkObject>().Spawn();

        CallingClientRpc();
    }

    [ClientRpc]
    public void CallingClientRpc()
    {
        //if (!IsServer) return;

        
    }
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