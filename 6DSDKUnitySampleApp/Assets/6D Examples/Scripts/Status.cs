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
using SixDegrees;

public class Status : MonoBehaviour
{
    public SDKController sdkController;

    void Start()
    {
        sdkController.OnSaveSucceededEvent += SaveSuccess;
        sdkController.OnLoadSucceededEvent += LoadSuccess;
        sdkController.OnSaveErrorEvent += SaveError;
        sdkController.OnLoadErrorEvent += LoadError;
        sdkController.OnFindingLocationEvent += FindingLocation;
        sdkController.OnUploadingEvent += Uploading;
        sdkController.OnDownloadingEvent += Downloading;
        sdkController.OnRelocalizingEvent += Relocalizing;
        sdkController.OnCancelledEvent += Cancelled;
    }

    void OnDestroy()
    {
        sdkController.OnSaveSucceededEvent -= SaveSuccess;
        sdkController.OnLoadSucceededEvent -= LoadSuccess;
        sdkController.OnSaveErrorEvent -= SaveError;
        sdkController.OnLoadErrorEvent -= LoadError;
        sdkController.OnFindingLocationEvent -= FindingLocation;
        sdkController.OnUploadingEvent -= Uploading;
        sdkController.OnDownloadingEvent -= Downloading;
        sdkController.OnRelocalizingEvent -= Relocalizing;
        sdkController.OnCancelledEvent -= Cancelled;
    }

    public void SaveSuccess()
    {
        if (FeedBackMessages.FeedBackInstance != null)
        {
            FeedBackMessages.FeedBackInstance.PrintStatus("Location #" + SDPlugin.LocationID + "\nsuccessfully saved!");
        }
    }

    public void LoadSuccess()
    {
        if (FeedBackMessages.FeedBackInstance != null)
        {
            FeedBackMessages.FeedBackInstance.PrintStatus("Location #" + SDPlugin.LocationID + "\nsuccessfully loaded!", 3f, true);
        }
    }
    public void SaveError(int saveError)
    {
        SDPlugin.SDSaveError error = (SDPlugin.SDSaveError)saveError;
        if (FeedBackMessages.FeedBackInstance != null)
        {
            FeedBackMessages.FeedBackInstance.PrintStatus("Error saving the location\n" + error.ToString());
        }
    }

    public void LoadError(int loadError)
    {
        SDPlugin.SDLoadError error = (SDPlugin.SDLoadError)loadError;
        if (FeedBackMessages.FeedBackInstance != null)
        {
            FeedBackMessages.FeedBackInstance.PrintStatus("Error loading the location\n" + error.ToString());
        }
    }

    public void FindingLocation()
    {
        if (FeedBackMessages.FeedBackInstance != null)
        {
            FeedBackMessages.FeedBackInstance.PrintStatus("Finding location...", 0f);
        }
    }
    public void Downloading()
    {
        if (FeedBackMessages.FeedBackInstance != null)
        {
            FeedBackMessages.FeedBackInstance.PrintStatus("Loading from AR Cloud...", 0f);
        }
    }

    public void Uploading()
    {
        if (FeedBackMessages.FeedBackInstance != null)
        {
            FeedBackMessages.FeedBackInstance.PrintStatus("Saving to AR Cloud...", 0f);
        }
    }

    public void Relocalizing()
    {
        if (FeedBackMessages.FeedBackInstance != null)
        {
            FeedBackMessages.FeedBackInstance.PrintStatus("Relocalizing...", 0f);
        }
    }

    public void Cancelled()
    {
        if (FeedBackMessages.FeedBackInstance != null)
        {
            FeedBackMessages.FeedBackInstance.PrintStatus("Cancelled");
        }
    }
}
