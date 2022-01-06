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

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            roleDropdown.gameObject.SetActive(false);
        }

        roleName.text = "None";
        clientName.text = $"Player {theValue}";

        SetupClientRpc();

    }


    /// <summary>
    /// Calling from client to server
    /// </summary>
    [ServerRpc]
    public void SetupServerRpc(ulong value)
    {
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

        //! Only accessible on server
        //networkObject = NetworkManager.Singleton.ConnectedClients[theValue].PlayerObject;
        //player = networkObject.GetComponent<Player>();
        
    }

    [ServerRpc]
    public void TestServerRpc(NetworkObjectReference target)
    {
        TryClientRpc(target);
    }

    [ClientRpc]
    public void TryClientRpc(NetworkObjectReference target)
    {
        networkObjected = target;
        //! This get this gameobject
        //player = networkObjected.GetComponent<Player>();
        Debug.Log("Cuba try test");
    }

    [ServerRpc]
    public void TestGiveServerRpc(NetworkObjectReference target)
    {
        TestGiveClientRpc(target);
    }

    public NetworkObject netObj;
    [ClientRpc]
    public void TestGiveClientRpc(NetworkObjectReference target)
    {
        Debug.Log("Cuba try give");
        netObj = target;
        player = netObj.GetComponent<Player>();
    }

    public NetworkObject networkObject;
    public  NetworkObject networkObjected;
    /// <summary>
    /// Calling from server to client
    /// </summary>
    [ClientRpc]
    public void SetupClientRpc(ulong value)
    {
        clientName.text = $"Player {value}";
        theValue = value;

        //gameObject.name = $"Client UI {value}";

        //! Can't find object this way. No idea why
         if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(value, out networkObjected))
         {
            
            //! This is missing in client thus not able to call rpc properly
            player = networkObjected.GetComponent<Player>();
            Debug.Log($"Client have found {value} value ..  {player.name} .. ");
         }
    }

    /// <summary>
    /// Calling from server to client
    /// 
    /// </summary>
    [ClientRpc]
    public void SetupClientRpc()
    {

    }

    /// <summary>
    /// Calling from server to client.
    /// This is called from dropdown menu
    /// </summary>
    /// <param name="value"></param>
    [ClientRpc]
    public void SetupRoleClientRpc(int value)
    {
        //! Only server can call this
        //! This function only work will existing player. Not player who join later
        if(player != null)
            player.UpdateRoleValue(value);
        else
            Debug.Log("player is null");

        //! Later player can't receive value
        //switch (value)
        //{
        //    case 0:
        //        roleName.text = "None";
        //        break;
        //    case 1:
        //        roleName.text = "Trainer";             
        //        break;
        //    case 2:
        //        roleName.text = "Trainee";
        //        break;
        //    case 3:
        //        roleName.text = "Observer";
        //        break;
        //}
    }
}
