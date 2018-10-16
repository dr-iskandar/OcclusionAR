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
using UnityEngine.UI;
using SixDegrees;

public class CheckIcons : MonoBehaviour
{

    public GameObject meshingScene;

    public GameObject ballScene;

    void Update()
    {
        if (SDPlugin.IsSDKReady)
        {
            if (!SDPlugin.SixDegreesSDK_HasRealTimeMesh())
            {
                meshingScene.SetActive(false);
                ballScene.SetActive(false);
				Destroy(this);
            }
        }
    }
}
