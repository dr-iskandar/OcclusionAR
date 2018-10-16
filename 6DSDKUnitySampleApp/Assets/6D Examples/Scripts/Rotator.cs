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

using System;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float speed = 1.0f;

	void Update () 
	{
        transform.Rotate(Vector3.up, speed * Time.deltaTime);
	}
}

