using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindIpAddress : MonoBehaviour
{
    public string ipAddress;

    // Start is called before the first frame update
    void Start()
    {
        ipAddress = GetIpAddress.GetLocalIpAddress();
    }
}
