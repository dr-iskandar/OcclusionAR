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

public class BallBehaviour : MonoBehaviour
{

    public BallLaunch ballLaunch;

	public float maxBallDistance = 30.0f;

    void Start()
    {
        ballLaunch = FindObjectOfType<BallLaunch>();
    }

    void Update()
    {
        if (Vector3.Distance(this.transform.position, Camera.main.transform.position) > maxBallDistance)
        {
            ballLaunch.DestroyBall(this.gameObject);
        }
    }
}
