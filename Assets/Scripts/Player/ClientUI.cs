using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using UnityEngine.UI;

public class ClientUI : NetworkBehaviour
{
    public ulong theValue;      //! This doesn't sync for later player
    public TextMeshProUGUI clientName;
    public TextMeshProUGUI roleName;
    public TMP_Dropdown roleDropdown;

    [Space]
    public Player player;   // To relay back to the player of their role //! Missing host


    public NetworkVariable<ulong> playerId = new NetworkVariable<ulong>(); // Store playerId on server
    public NetworkObject netObj;    // passing network object across client

    //! Dont remember what these do
    public NetworkObject networkObject; 
    public NetworkObject networkObjected;

    public Player[] players;    // Get and temporary store player objects

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            roleDropdown.gameObject.SetActive(false);
        }

        roleName.text = "None";
        clientName.text = $"Player {theValue}";
    }


    /// <summary>
    /// Calling from client to server
    /// Storing playerId on the server
    /// </summary>
    [ServerRpc]
    public void SetupServerRpc(ulong value)
    {
        playerId.Value = value;
        clientName.text = $"Player {value}";
        theValue = value;
        SetupClientRpc(value);

        //! Can't find object this way. No idea why
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(value, out networkObject))
        {
            //! This is missing in client thus not able to call rpc properly (fixed)
            //! Still missing in host object || remote still doesn't work

            player = networkObject.GetComponent<Player>();
            Debug.Log($"Host have found {value} value");
        }
    }

    [ServerRpc]
    public void TestGiveServerRpc(NetworkObjectReference target)
    {
        TestGiveClientRpc(target);
    }

    [ClientRpc]
    public void TestGiveClientRpc(NetworkObjectReference target)
    {
        Debug.Log("Cuba try give");
        netObj = target;
        player = netObj.GetComponent<Player>();
    }

    /// <summary>
    /// Calling from server to client
    /// </summary>
    [ClientRpc]
    public void SetupClientRpc(ulong value)
    {
        clientName.text = $"Player {value}";
        theValue = value;

        //! Can't find object this way. No idea why
         if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(value, out networkObjected))
         {
            //! This is missing in client thus not able to call rpc properly
            player = networkObjected.GetComponent<Player>();
            Debug.Log($"Client have found {value} value ..  {player.name} .. ");
         }
    }

    /// <summary>
    /// Calling from server to client.
    /// This is called from dropdown menu
    /// </summary>
    /// <param name="value"></param>
    [ClientRpc]
    public void SetupRoleClientRpc(int value)
    {
        if (player != null)
            player.UpdateRoleValue(value);
        else
        {
            players = FindObjectsOfType<Player>();

            for (int i = 0; i < players.Length; i++)
            {
                Debug.Log($"Searching netId {players[i].NetworkObject.OwnerClientId} ,{playerId.Value}");
                if (players[i].OwnerClientId == playerId.Value)
                {
                    player = players[i];
                    player.UpdateRoleValue(value);
                    netObj = player.GetComponent<Player>().NetworkObject;
                   
                    player.clientUI = this;
                    player.netObject = GetComponent<NetworkObject>();
                    player.roleName = roleName;
                   
                    player.UpdateRoleValue(value);
                    return;
                }
            }
            
            //! Find the player with same id
            Debug.Log("player is null");
        }
    }
}
