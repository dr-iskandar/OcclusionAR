/***********************************************************
* Copyright (C) 2018 6degrees.xyz Inc.
*
* This file is part of the 6D.ai Beta SDK and is not licensed
* for commercial use.
*
* The 6D.ai Beta SDK can not be copied and/or distributed without
* the express permission of 6degrees.xyz Inc.
*
* Contact developers@6d.ai for licensing requests.
***********************************************************/

using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class AutoNetworkDiscovery : NetworkDiscovery
{
    // will need to override NetworkDiscoveryEditor.OnInspectorGUI() to show those two
    public NetworkManager manager;
    public float discoveryTimeout = 3.0f;

    public void Start()
    {
        Initialize();

        bool listening = StartAsClient();
        Debug.Log("Start As Client: " + listening);
        if (listening)
        {
            Invoke("Timeout", discoveryTimeout);
        }
    }

    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        Debug.Log("Received Broadcast: from Address " + fromAddress + " data " + data);
        CancelInvoke(); // will prevent timeout
        manager.networkAddress = fromAddress;
        manager.StartClient();
    }

    protected void Timeout()
    {
        Debug.Log("Didn't Receive Broadcast: Start Hosting");
        manager.StartHost();
    }
}

