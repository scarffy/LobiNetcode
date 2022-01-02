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
    public Player player;   // To relay back to the player of their role
    //[SerializeField] NetworkObject netObject;   // No idea what to do with this yet

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            roleDropdown.gameObject.SetActive(false);
        }
        //netObject = GetComponent<NetworkObject>();


        roleName.text = "None";
    }

    /// <summary>
    /// Calling from client to server
    /// </summary>
    [ServerRpc]
    public void SetupServerRpc(ulong value)
    {
        //! Calling from client to server

        clientName.text = $"Player {value}";
        SetupClientRpc(clientName.text);

        NetworkObject networkObject;
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(value, out networkObject))
        {
            player = networkObject.GetComponent<Player>();
        }
    }

    /// <summary>
    /// Calling from server to client
    /// </summary>
    [ClientRpc]
    public void SetupClientRpc(string value)
    {
        //! Calling from server to client
        clientName.text = value;

        
    }

    /// <summary>
    /// Calling from server to client
    /// </summary>
    /// <param name="value"></param>
    [ClientRpc]
    public void SetupRoleClientRpc(int value)
    {
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
