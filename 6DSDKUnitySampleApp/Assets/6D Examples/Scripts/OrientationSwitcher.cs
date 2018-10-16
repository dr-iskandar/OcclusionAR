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
using System;

public class OrientationSwitcher : MonoBehaviour
{
    public GameObject landscapeUI;

    public GameObject portraitUI;

    void Update()
    {
        if (Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown)
        {
            landscapeUI.SetActive(false);
            portraitUI.SetActive(true);
        }
        else
        {
            portraitUI.SetActive(false);
            landscapeUI.SetActive(true);
        }
    }
}
