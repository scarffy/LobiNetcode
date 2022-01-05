using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using UnityEngine.UI;

public class ClientUI : NetworkBehaviour
{
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
    }

    /// <summary>
    /// Calling from client to server
    /// </summary>
    [ServerRpc]
    public void SetupServerRpc(ulong value)
    {
        clientName.text = $"Player {value}";

        NetworkObject networkObject;
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(value, out networkObject))
        {
            //! This is missing in client thus not able to call rpc properly (fixed)
            //! Still missing in host object
            player = networkObject.GetComponent<Player>();
            Debug.Log($"Have found {value} value");
        }
        SetupClientRpc(value);
    }

    /// <summary>
    /// Calling from server to client
    /// </summary>
    [ClientRpc]
    public void SetupClientRpc(ulong value)
    {
        clientName.text = $"Player {value}";

        NetworkObject networkObject;
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(value, out networkObject))
        {
            //! This is missing in client thus not able to call rpc properly
            player = networkObject.GetComponent<Player>();
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
        //! Only server can call this
        //! This function only work will existing player. Not player who join later
        player.UpdateRoleValue(value);

        switch (value)
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
