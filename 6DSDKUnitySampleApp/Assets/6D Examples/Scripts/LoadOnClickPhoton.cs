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
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class LoadOnClickPhoton : Photon.PunBehaviour
{
    public void LoadScene(int level)
    {
        SceneManager.LoadSceneAsync(level, LoadSceneMode.Single);
    }

    public void PunLeaveRoom()
    {
        if (PhotonNetwork.inRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            LoadScene(0);
        }
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        LoadScene(0);
    }
}