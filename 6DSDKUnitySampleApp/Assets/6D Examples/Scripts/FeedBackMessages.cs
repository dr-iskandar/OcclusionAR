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

using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FeedBackMessages : MonoBehaviour
{
    public Text feedback;

    private AudioSource chime;

    private Coroutine clearCoroutine = null;

    public static FeedBackMessages FeedBackInstance = null;


    void OnDisable()
    {
        feedback.text = "";
    }

    void OnEnable()
    {
        FeedBackInstance = this;
    }

    void Awake()
    {
        FeedBackInstance = this;
    }

    void Start()
    {
        chime = GetComponent<AudioSource>();
    }

    public void PrintStatus(string status, float seconds = 3f, bool playChime = false)
    {
        feedback.text = status;

        if (clearCoroutine != null)
        {
            StopCoroutine(clearCoroutine);
        }
        clearCoroutine = null;

        if (playChime)
        {
            chime.Play();
        }

        if (seconds > 0f)
        {
            clearCoroutine = StartCoroutine(ClearStatusCoroutine(seconds));
        }
    }

    private IEnumerator ClearStatusCoroutine(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        feedback.text = "";
        yield return null;
    }
}
