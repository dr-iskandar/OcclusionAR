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
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using SixDegrees;

public class GameControllerPhoton : Photon.MonoBehaviour
{
#if UNITY_IOS
    [DllImport("__Internal")]
    public static extern void GetAPIKey(StringBuilder apiKey, int bufferSize);
#else
    public static void GetAPIKey(StringBuilder apiKey, int bufferSize) { }
#endif
    private static GameControllerPhoton _LocalPlayerInstance = null;

    private static List<GameControllerPhoton> controllers = new List<GameControllerPhoton>();

    public static GameControllerPhoton LocalPlayerInstance
    {
        get
        {
            return _LocalPlayerInstance;
        }
    }

    private ColorWheelControl colorWheel;

    public GameObject segmentPrefab;

    private GameObject segment;

    private FileControlPhoton fileControlPhoton;

    private SDKController sdkController;

    private static string apiKey = "";

    private string filename;

    public Dictionary<GameObject, Vector3[]> mLines = new Dictionary<GameObject, Vector3[]>();

    public bool loaded;

    public bool saving;

    void Start()
    {
        fileControlPhoton = GameObject.FindObjectOfType<FileControlPhoton>();
        colorWheel = GameObject.FindObjectOfType<ColorWheelControl>();
        sdkController = GameObject.FindObjectOfType<SDKController>();
        if (photonView.isMine)
        {
            _LocalPlayerInstance = this;
            sdkController.OnSaveSucceededEvent += SaveCSV;
            sdkController.OnLoadSucceededEvent += RetrieveFile;
            if (saving == true)
            {
                SaveCSV();
            }
            if (loaded == true)
            {
                RetrieveFile();
            }
            Color newColor = new Color();
            newColor = colorWheel.Selection;
            Vector3 colorValues = new Vector3(newColor.r, newColor.g, newColor.b);
            ChangeCameraColor(colorValues, newColor.a);
        }
        controllers.Add(this);
    }

    void OnDestroy()
    {
        controllers.Remove(this);
        if (photonView.isMine)
        {
            sdkController.OnSaveSucceededEvent -= SaveCSV;
            sdkController.OnLoadSucceededEvent -= RetrieveFile;
        }
    }

    private void GetFilename()
    {
        if (string.IsNullOrEmpty(apiKey))
        {
            StringBuilder sb = new StringBuilder(32);
            GetAPIKey(sb, 32);
            apiKey = sb.ToString();
        }

        if (string.IsNullOrEmpty(apiKey))
        {
            Debug.Log("API Key cannot be found");
            filename = "";
        }

        if (string.IsNullOrEmpty(SDPlugin.LocationID))
        {
            Debug.Log("Location ID is missing");
            filename = "";
        }

        filename = apiKey + "-" + SDPlugin.LocationID;
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            CreateLine();
        }
    }

    public void CreateLine()
    {
        Touch touch = Input.GetTouch(0);
        int id = touch.fingerId;
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            if (touch.phase == TouchPhase.Began)
            {
                Vector3 mPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, .5f);
                mPosition = Camera.main.ScreenToWorldPoint(mPosition);
                if (photonView.isMine)
                {
                    Color newColor = new Color();
                    newColor = colorWheel.Selection;
                    Vector3 colorValues = new Vector3(newColor.r, newColor.g, newColor.b);
                    SpawnLine(mPosition, colorValues, newColor.a);
                    ChangeCameraColor(colorValues, newColor.a);
                }
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                Vector3 mPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, .5f);
                mPosition = Camera.main.ScreenToWorldPoint(mPosition);
                if (photonView.isMine)
                {
                    SpawnSegments(mPosition);
                }
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                if (photonView.isMine)
                {
                    Vector3[] positions = new Vector3[segment.GetComponent<LineRenderer>().positionCount];
                    SendLine(positions);
                }
            }
        }
    }

    [PunRPC]
    public void SendLine(Vector3[] positions)
    {
        segment.GetComponent<LineRenderer>().GetPositions(positions);
        mLines.Add(segment, positions);
        PhotonView photonView = PhotonView.Get(this);
        if (photonView.isMine)
            photonView.RPC("SendLine", PhotonTargets.OthersBuffered, positions);
    }

    [PunRPC]
    public void SpawnLine(Vector3 mPosition, Vector3 colorValues, float alpha)
    {
        segment = (GameObject)PhotonNetwork.Instantiate(segmentPrefab.name, mPosition, Quaternion.identity, 0);
        segment.GetComponent<LineRenderer>().material.color = new Color(colorValues.x, colorValues.y, colorValues.z, alpha);
        PhotonView photonView = PhotonView.Get(this);
        if (photonView.isMine)
            photonView.RPC("SpawnLine", PhotonTargets.OthersBuffered, mPosition, colorValues, alpha);
    }

    [PunRPC]
    public void SpawnSegments(Vector3 mPosition)
    {
        LineRenderer lastLineRenderer = segment.GetComponent<LineRenderer>();
        int positionCount = lastLineRenderer.positionCount;
        lastLineRenderer.positionCount = positionCount + 1;
        lastLineRenderer.SetPosition(positionCount, mPosition);
        PhotonView photonView = PhotonView.Get(this);
        if (photonView.isMine)
            photonView.RPC("SpawnSegments", PhotonTargets.OthersBuffered, mPosition);
    }

    [PunRPC]
    public void ChangeCameraColor(Vector3 colorValues, float alpha)
    {
        foreach (Transform child in this.transform)
        {
            child.gameObject.GetComponent<Renderer>().material.color = new Color(colorValues.x, colorValues.y, colorValues.z, alpha); ;
        }
        PhotonView photonView = PhotonView.Get(this);
        if (photonView.isMine)
            photonView.RPC("ChangeCameraColor", PhotonTargets.OthersBuffered, colorValues, alpha);
    }

    public void SaveCSV()
    {
        GetFilename();

        if (string.IsNullOrEmpty(filename))
        {
            Debug.Log("Error evaluating the filename, will not save content CSV");
            return;
        }
        string filePath = GetPath();
        StreamWriter writer = new StreamWriter(filePath);
        writer.WriteLine(controllers.Count);
        for (int j = 0; j < controllers.Count; j++)
        {
            writer.WriteLine(controllers[j].mLines.Count);
            foreach (KeyValuePair<GameObject, Vector3[]> entry in controllers[j].mLines)
            {
                writer.WriteLine(entry.Key.transform.position.x + "," + entry.Key.transform.position.y + "," + entry.Key.transform.position.z);
                writer.WriteLine(entry.Key.GetComponent<LineRenderer>().positionCount);
                for (int i = 0; i < entry.Key.GetComponent<LineRenderer>().positionCount; i++)
                {
                    Vector3 line_positions = entry.Key.GetComponent<LineRenderer>().GetPosition(i);
                    writer.WriteLine(line_positions.x + "," + line_positions.y + "," + line_positions.z);
                    writer.WriteLine(entry.Key.GetComponent<LineRenderer>().material.color.r + "," + entry.Key.GetComponent<LineRenderer>().material.color.g + "," + entry.Key.GetComponent<LineRenderer>().material.color.b + "," + entry.Key.GetComponent<LineRenderer>().material.color.a);
                }
            }
        }
        writer.Flush();
        writer.Close();

        StartCoroutine(fileControlPhoton.UploadFileCoroutine(filename));
    }

    public void ReadTextFile(string csv)
    {
        StringReader reader = new StringReader(csv);
        string line = reader.ReadLine();
        int controllersCount = int.Parse(line);
        for (int k = 0; k < controllersCount; k++)
        {
            line = reader.ReadLine();
            int lineCount;
            lineCount = int.Parse(line);
            for (int i = 0; i < lineCount; i++)
            {
                line = reader.ReadLine();
                string[] parts = line.Split(',');
                Vector3 linePosition = new Vector3();
                linePosition.x = float.Parse(parts[0]);
                linePosition.y = float.Parse(parts[1]);
                linePosition.z = float.Parse(parts[2]);
                line = reader.ReadLine();
                int vertexCount = int.Parse(line);
                segment = (GameObject)PhotonNetwork.Instantiate(segmentPrefab.name, linePosition, Quaternion.identity, 0);
                segment.GetComponent<LineRenderer>().positionCount = vertexCount;
                for (int j = 0; j < vertexCount; j++)
                {
                    line = reader.ReadLine();
                    string[] vertexParts = line.Split(',');
                    Vector3 vertexPosition = new Vector3();
                    vertexPosition.x = float.Parse(vertexParts[0]);
                    vertexPosition.y = float.Parse(vertexParts[1]);
                    vertexPosition.z = float.Parse(vertexParts[2]);
                    segment.GetComponent<LineRenderer>().SetPosition(j, vertexPosition);
                    line = reader.ReadLine();
                    Color newCol = new Color();
                    string[] colParts = line.Split(',');
                    newCol.r = float.Parse(colParts[0]);
                    newCol.g = float.Parse(colParts[1]);
                    newCol.b = float.Parse(colParts[2]);
                    newCol.a = float.Parse(colParts[3]);
                    segment.GetComponent<LineRenderer>().material.color = newCol;
                }

                if (photonView.isMine)
                {
                    Vector3[] positions = new Vector3[segment.GetComponent<LineRenderer>().positionCount];
                    segment.GetComponent<LineRenderer>().GetPositions(positions);
                    mLines.Add(segment, positions);
                }
            }
        }
        reader.Close();
    }

    public string GetPath()
    {
        return Application.persistentDataPath + "/" + SDPlugin.LocationID + ".csv";
    }

    public void RetrieveFile()
    {
        GetFilename();
        if (string.IsNullOrEmpty(filename))
        {
            Debug.Log("Error evaluating the filename, will not load content CSV");
            return;
        }
        StartCoroutine(fileControlPhoton.GetTextCoroutine(filename));
    }
}

