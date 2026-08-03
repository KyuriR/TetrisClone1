

using System.Collections.Generic;
using NUnit.Framework.Internal;
using UnityEngine;

public class TetrisGrid : MonoBehaviour
{
    public static TetrisGrid Instance {get; private set;  }

    [SerializeField] int width = 10;
    [SerializeField] int height = 20;
    [System.NonSerialized] public Transform[,] grid;

    //initialize grid
    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        Instance = this;
        grid = new Transform[width, height];
    }

    public List<int> GetFullRows()
    {
         var rows = new List<int>();
         for(int y = 0; y < height; y++)
            if(IsRowFull(y))
                rows.Add(y);
        return rows;
    }

    public bool IsRowFull(int y)
    {
        for (int x = 0; x < width; x++)
            if (grid[x, y] == null)
                return false;
        return  true;
    }

    public void ClearRow(int y)
    {
        for (int x = 0; x < width; x++)
        {
            Destroy(grid[x , y].gameObject);
            grid[x, y] = null;
        }
    }

    public void MoveRowDown(int y, int amount)
    {
        for(int x = 0; x < width; x++)
        {
            Transform t = grid[x,y];
            if(t != null)
            {
                grid[x, y] = null;
                grid[x, y - amount] = t;
                t.position += Vector3.down * amount;
            }
        }
    }

    public int ClearFullLines()
    {
        int cleared = 0;
        for (int y = 0; y < height; y++)
        {
            if(IsRowFull(y))
            {
                ClearRow(y);
                cleared++;
            }
            else if (cleared > 0)
            {
                MoveRowDown(y, cleared);
            }
        }
        return cleared;
    }

    //visualize in the unity editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 origin = transform.position + new Vector3(-0.5f, -0.5f, 0f);

        for(int x = 0; x <= width; x++)
        {
            var p1 = origin + new Vector3(x, 0, 0);
            var p2 = origin + new Vector3(x, height, 0);
            Gizmos.DrawLine(p1, p2);
        }

        for(int y = 0; y <= height; y++)
        {
            var p1 = origin + new Vector3(0, y, 0);
            var p2 = origin + new Vector3(width, y, 0);
            Gizmos.DrawLine(p1, p2);
        }
    }

    
}


/*
using UnityEngine;

[ExecuteAlways]
public class TetrisGrid : MonoBehaviour
{
    [SerializeField] int width = 10;
    [SerializeField] int height = 20;
    [SerializeField] Camera targetCamera;
    [SerializeField] float cameraDepth = -10f;
    [SerializeField] float orthoSizeMargin = 0.5f;

    [System.NonSerialized] public Transform[,] grid;

    void Awake()
    {
        grid = new Transform[width, height];
        FitCameraToGrid();
    }

    private void Start()
    {
        FitCameraToGrid();
    }

    private void OnValidate()
    {
        if (width < 1) width = 1;
        if (height < 1) height = 1;
        FitCameraToGrid();
    }

    void FitCameraToGrid()
    {
        Camera cam = targetCamera ? targetCamera : Camera.main;
        if (cam == null) return;

        cam.orthographic = true;
        cam.aspect = 1920f / 1080f;
        cam.orthographicSize = height * 0.5f + orthoSizeMargin;
        cam.transform.position = new Vector3(width % 2 == 0 ? -0.5f : 0f, height % 2 == 0 ? -0.5f : 0f, cameraDepth);

        transform.position = new Vector3(-width * 0.5f + 0.5f, -height * 0.5f + 0.5f, 0f);
    }

    //visualize in the unity editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 origin = transform.position + new Vector3(-0.5f, -0.5f, 0f);

        for (int x = 0; x <= width; x++)
        {
            var p1 = origin + new Vector3(x, 0, 0);
            var p2 = origin + new Vector3(x, height, 0);
            Gizmos.DrawLine(p1, p2);
        }

        for (int y = 0; y <= height; y++)
        {
            var p1 = origin + new Vector3(0, y, 0);
            var p2 = origin + new Vector3(width, y, 0);
            Gizmos.DrawLine(p1, p2);
        }
    }
}
*/ 