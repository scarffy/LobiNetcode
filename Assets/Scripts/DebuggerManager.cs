using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebuggerManager : MonoBehaviour
{
    public static DebuggerManager Instance;
    public TextMeshProUGUI debugText;

    // Start is called before the first frame update
    void Start()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void DebugMsg(string value)
    {
        debugText.SetText(value);
    }

    public GameObject[] players;
    public void resetCounter()
    {
        players = GameObject.FindGameObjectsWithTag("Player");

        for (int i = 0; i < players.Length; i++)
        {
            //players[i].GetComponent<Player>().playerRoles.Value = 0;
            players[i].GetComponent<Player>().UpdateRoleValue(0);
        }
    }
}
