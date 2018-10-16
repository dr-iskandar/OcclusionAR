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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class AutoNetworkMigrationManager : NetworkMigrationManager
{
    private PeerInfoMessage m_NewHostInfo = new PeerInfoMessage();

    public AutoNetworkDiscovery discovery;

    public AutoNetworkManager autoNetworkManager;

    private void Awake()
    {
        Debug.Log("Migration Awake call....");
    }

    public override bool FindNewHost(out PeerInfoMessage newHostInfo, out bool youAreNewHost)
    {
        Debug.Log("FindNewHost");
        return base.FindNewHost(out newHostInfo, out youAreNewHost);
    }

    protected override void OnAuthorityUpdated(GameObject go, int connectionId, bool authorityState)
    {
        base.OnAuthorityUpdated(go, connectionId, authorityState);
        Debug.Log("OnAuthorityUpdated");
    }

    protected override void OnClientDisconnectedFromHost(NetworkConnection conn, out SceneChangeOption sceneChange)
    {
        base.OnClientDisconnectedFromHost(conn, out sceneChange);
        Debug.Log("OnClientDisconnectedFromHost");
        DisablePlayerObjects();
        FindNewHost(out m_NewHostInfo, out autoNetworkManager.isHost);
        Debug.Log("New host :- " + autoNetworkManager.isHost);

        if (autoNetworkManager.isHost)
        {
            BecomeNewHost(autoNetworkManager.networkPort);
            discovery.StartAsServer();
            Debug.Log("You become a new host at port :- " + autoNetworkManager.networkPort);
        }

        else
        {
            Reset(this.oldServerConnectionId);
            waitingReconnectToNewHost = true;
            autoNetworkManager.networkAddress = m_NewHostInfo.address;
            autoNetworkManager.client.ReconnectToNewHost(m_NewHostInfo.address, autoNetworkManager.networkPort);
            Debug.Log("You become a new client reconnect port -------> " + autoNetworkManager.networkPort);
            Debug.Log("new server ip :- " + m_NewHostInfo.address);
        }
    }
}

