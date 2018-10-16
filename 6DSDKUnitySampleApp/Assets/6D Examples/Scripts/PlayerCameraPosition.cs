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
using SixDegrees;

public class PlayerCameraPosition : NetworkBehaviour
{
    private GameObject mSDCamera;

    void Start()
    {
        if (isLocalPlayer)
        {
            SDCamera sdCamera = FindObjectOfType<SDCamera>();
            if (!sdCamera)
            {
                Debug.LogWarning("No SDCamera found!");
                return;
            }

            mSDCamera = sdCamera.gameObject;
        }
    }

    void Update()
    {
        if (isLocalPlayer && mSDCamera)
        {
            transform.SetPositionAndRotation(mSDCamera.transform.position, mSDCamera.transform.rotation);
        }
    }
}
