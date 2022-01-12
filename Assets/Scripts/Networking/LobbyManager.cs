using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class LobbyManager : NetworkBehaviour
{
    public static LobbyManager Instance;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [ServerRpc(RequireOwnership =false)]
    public void CallingServerRpc()
    {
        Debug.Log("Calling server rpc from client");
    }

    [ClientRpc]
    public void CallingClientRpc()
    {
        Debug.Log("Calling client rpc from server");
    }
}
