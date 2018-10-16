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
using UnityEngine.UI;

public class AutoNetworkManager : NetworkManager
{
    public AutoNetworkDiscovery discovery;

    public bool isHost = false;

    public bool startedClient = false;

    public AutoNetworkMigrationManager autoNetworkMigrationManager;

    public void Awake()
    {
        discovery.manager = this;
    }

    public override void OnStartHost()
    {
        Debug.Log("AutoNetworkDiscovery Starting Host");
        isHost = true;
        discovery.StopBroadcast(); // stop discovery client
        discovery.StartAsServer();
    }

    public override void OnStopHost()
    {
        Debug.Log("AutoNetworkDiscovery Stopping Host");
        isHost = false;
        discovery.StopBroadcast(); // stop discover server     
        autoNetworkMigrationManager.LostHostOnHost();
    }

    public override void OnStartClient(NetworkClient client)
    {
        startedClient = true;
        Debug.Log("AutoNetworkDiscovery Starting Client, isHost:" + isHost);
        if (!isHost)
        {
            discovery.StopBroadcast(); // stop discovery client
        }
        autoNetworkMigrationManager.Initialize(client, matchInfo);
    }

    public override void OnStopClient()
    {
        Debug.Log("AutoNetworkDiscovery Stopping Client");
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);
        autoNetworkMigrationManager.SendPeerInfo();
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        autoNetworkMigrationManager.SendPeerInfo();
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        autoNetworkMigrationManager.LostHostOnClient(conn);
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        base.OnServerAddPlayer(conn, playerControllerId);
        autoNetworkMigrationManager.SendPeerInfo();
    }

    public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController player)
    {
        base.OnServerRemovePlayer(conn, player);
        autoNetworkMigrationManager.SendPeerInfo();
    }
}
