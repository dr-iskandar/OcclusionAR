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
using UnityEngine.Networking;
using System.Runtime.InteropServices;
using System.Text;
using SixDegrees;

public class GameController : NetworkBehaviour
{
#if UNITY_IOS
    [DllImport("__Internal")]
    public static extern void GetAPIKey(StringBuilder apiKey, int bufferSize);
#else
    public static void GetAPIKey(StringBuilder apiKey, int bufferSize) { }
#endif
    private static GameController _LocalPlayerInstance = null;

    private static List<GameController> controllers = new List<GameController>();

    public static GameController LocalPlayerInstance
    {
        get
        {
            return _LocalPlayerInstance;
        }
    }

    private ColorWheelControl colorWheel;

    public GameObject segmentPrefab;

    private GameObject segment;

    private FileControl fileControl;

    private SDKController sdkController;

    private static string apiKey = "";

    private string filename;

    public struct Position
    {
        public Vector3 position;
        public Position(Vector3 pos)
        {
            this.position = pos;
        }
    }

    public struct Line
    {
        public GameObject line;
        public Color color;
        public Line(GameObject line, Color col)
        {
            this.line = line;
            this.color = col;
            this.line.GetComponent<LineRenderer>().material.color = col;
        }
    }

    public class SyncListPositions : SyncListStruct<Position>
    {
    }

    public class SyncListLines : SyncListStruct<Line>
    {
    }

    public SyncListPositions mPositions = new SyncListPositions();

    public SyncListLines mLines = new SyncListLines();

    void Start()
    {
        fileControl = GameObject.FindObjectOfType<FileControl>();
        colorWheel = GameObject.FindObjectOfType<ColorWheelControl>();
        sdkController = GameObject.FindObjectOfType<SDKController>();
        if (isLocalPlayer)
        {
            _LocalPlayerInstance = this;
            sdkController.OnSaveSucceededEvent += SaveCSV;
            sdkController.OnLoadSucceededEvent += RetrieveFile;
            Color newColor = new Color();
            newColor = colorWheel.Selection;
            CmdChangePlayerColor(newColor);
        }
        controllers.Add(this);
    }

    void OnDestroy()
    {
        controllers.Remove(this);
        if (isLocalPlayer)
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

    public override void OnStartClient()
    {
        mPositions.Callback = OnPositionsChanged;
        mLines.Callback = OnLinesChanged;
    }

    private void OnPositionsChanged(SyncListPositions.Operation op, int index)
    {
        LineRenderer lastLineRenderer = mLines[mLines.Count - 1].line.GetComponent<LineRenderer>();
        int positionCount = lastLineRenderer.positionCount;
        lastLineRenderer.positionCount = positionCount + 1;
        lastLineRenderer.SetPosition(positionCount, mPositions[index].position);
    }

    private void OnLinesChanged(SyncListLines.Operation op, int index)
    {
        mLines[index].line.GetComponent<LineRenderer>().material.color = mLines[index].color;
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
        if (!EventSystem.current.IsPointerOverGameObject(id))
        {
            if (touch.phase == TouchPhase.Began)
            {
                Vector3 mPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, .5f);
                mPosition = Camera.main.ScreenToWorldPoint(mPosition);
                if (isLocalPlayer)
                {
                    Color newColor = new Color();
                    newColor = colorWheel.Selection;
                    CmdChangePlayerColor(newColor);
                    CmdSpawnLine(mPosition, newColor);
                }
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                Vector3 mPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, .5f);
                mPosition = Camera.main.ScreenToWorldPoint(mPosition);
                if (isLocalPlayer)
                {
                    CmdAddLines(mPosition);
                }
            }
        }
    }

    [Command]
    public void CmdAddLines(Vector3 mPosition)
    {
        Position x = new Position(mPosition);
        mPositions.Add(x);
    }

    [Command]
    public void CmdSpawnLine(Vector3 mPosition, Color col)
    {
        segment = (GameObject)Instantiate(segmentPrefab, mPosition, Quaternion.identity);
        NetworkServer.Spawn(segment);
        Line x = new Line(segment, col);
        mLines.Add(x);
    }

    [Command]
    public void CmdSpawnL()
    {
        NetworkServer.Spawn(segment);
        Line x = new Line(segment, segment.GetComponent<LineRenderer>().material.color);
        mLines.Add(x);
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
        for (int k = 0; k < controllers.Count; k++)
        {
            writer.WriteLine(controllers[k].mLines.Count);
            for (int i = 0; i < controllers[k].mLines.Count; i++)
            {
                writer.WriteLine(controllers[k].mLines[i].line.transform.position.x + "," + controllers[k].mLines[i].line.transform.position.y + "," + controllers[k].mLines[i].line.transform.position.z);
                writer.WriteLine(controllers[k].mLines[i].line.GetComponent<LineRenderer>().positionCount);
                for (int j = 0; j < controllers[k].mLines[i].line.GetComponent<LineRenderer>().positionCount; j++)
                {
                    Vector3 line_positions = controllers[k].mLines[i].line.GetComponent<LineRenderer>().GetPosition(j);
                    writer.WriteLine(line_positions.x + "," + line_positions.y + "," + line_positions.z);
                    writer.WriteLine(controllers[k].mLines[i].line.GetComponent<LineRenderer>().material.color.r + "," + controllers[k].mLines[i].line.GetComponent<LineRenderer>().material.color.g + "," + controllers[k].mLines[i].line.GetComponent<LineRenderer>().material.color.b + "," + controllers[k].mLines[i].line.GetComponent<LineRenderer>().material.color.a);
                }
            }
        }
        writer.Flush();
        writer.Close();

        StartCoroutine(fileControl.UploadFileCoroutine(filename));
    }

    public void ReadTextFile(string csv)
    {
        StringReader reader = new StringReader(csv);
        string line = reader.ReadLine();
        int lineCount;
        int controllerCount = int.Parse(line);
        for (int k = 0; k < controllerCount; k++)
        {
            line = reader.ReadLine();
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
                segment = (GameObject)Instantiate(segmentPrefab, linePosition, Quaternion.identity);
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

                if (isLocalPlayer)
                {
                    CmdSpawnL();
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

        StartCoroutine(fileControl.GetTextCoroutine(filename));
    }

    [Command]
    public void CmdChangePlayerColor(Color col)
    {
        RpcChangePlayerColor(col);
    }

    [ClientRpc]
    void RpcChangePlayerColor(Color col)
    {
        foreach (Transform child in this.transform)
        {
            child.gameObject.GetComponent<Renderer>().material.color = col;
        }
    }
}

