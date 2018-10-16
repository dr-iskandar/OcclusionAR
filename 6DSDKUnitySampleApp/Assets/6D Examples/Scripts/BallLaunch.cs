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

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BallLaunch : MonoBehaviour
{
    public GameObject ballReference;

    public GameObject projectorReference;

    private List<GameObject> balls;

    public int maxBallCount = 30;

    void Start()
    {
        balls = new List<GameObject>();
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            LaunchBall(touch);
        }
    }

    void LaunchBall(Touch touch)
    {
        if (EventSystem.current.currentSelectedGameObject == null &&
            touch.phase == TouchPhase.Began)
        {
            Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, .2f);
            position = Camera.main.ScreenToWorldPoint(position);
            GameObject ball = Instantiate(ballReference, position, ballReference.transform.rotation);
            GameObject projector = Instantiate(projectorReference, position, projectorReference.transform.rotation);
            ProjectorBehaviour pb = projector.AddComponent<ProjectorBehaviour>();
            pb.sphere = ball;
            ball.GetComponent<MeshRenderer>().material.color = Random.ColorHSV();
            ball.GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward * 180);
            balls.Add(ball);
            while (balls.Count >= maxBallCount)
            {
                Destroy(balls[0]);
                balls[0] = null;
                balls.RemoveAt(0);
            }
        }
    }

    public void DestroyBall(GameObject ball)
    {
        int i = balls.FindIndex((GameObject obj) => ball == obj);
        if (i >= 0) 
        {
            Destroy(balls[i]);
            balls[i] = null;
            balls.RemoveAt(i);
        }
    }
}
