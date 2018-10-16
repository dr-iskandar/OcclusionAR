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
using SixDegrees;
using System;

public class Launcher : Photon.PunBehaviour
{
    public byte MaxPlayersPerRoom = 4;

    public GameObject playerPrefab;

    string _gameVersion = "1";

    public SDKController sdkController;

    public bool loaded;

    public bool saving;

    void Awake()
    {
        loaded = false;
        saving = false;
        sdkController.OnSaveSucceededEvent += StartGameSave;
        sdkController.OnLoadSucceededEvent += StartGameLoad;
    }

    void StartGameSave()
    {
        if (!PhotonNetwork.inRoom)
        {
            PhotonNetwork.autoJoinLobby = false;
            PhotonNetwork.automaticallySyncScene = true;
            saving = true;
            Connect();
        }
    }

    void StartGameLoad()
    {
        if (!PhotonNetwork.inRoom)
        {
            PhotonNetwork.autoJoinLobby = false;
            PhotonNetwork.automaticallySyncScene = true;
            loaded = true;
            Connect();
        }
    }

    void OnDestroy()
    {
        sdkController.OnSaveSucceededEvent -= StartGameSave;
        sdkController.OnLoadSucceededEvent -= StartGameLoad;
    }

    public void Connect()
    {
        if (PhotonNetwork.connected)
        {
            PhotonNetwork.JoinOrCreateRoom(SDPlugin.LocationID, new RoomOptions() { MaxPlayers = MaxPlayersPerRoom }, null);
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings(_gameVersion);
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("DemoAnimator/Launcher: OnConnectedToMaster() was called by PUN");
        PhotonNetwork.JoinOrCreateRoom(SDPlugin.LocationID, new RoomOptions() { MaxPlayers = MaxPlayersPerRoom }, null);
    }

    public override void OnDisconnectedFromPhoton()
    {
        Debug.LogWarning("DemoAnimator/Launcher: OnDisconnectedFromPhoton() was called by PUN");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("DemoAnimator/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
        GameObject temp = PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero,
    Quaternion.identity, 0);
        if (loaded == true)
        {
            temp.GetComponent<GameControllerPhoton>().loaded = true;
        }
        if (saving == true)
        {
            temp.GetComponent<GameControllerPhoton>().saving = true;
        }
    }
}