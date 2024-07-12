using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathDemo : MonoBehaviour
{
    public LineRenderer pathLineRenderer;
    public LineRenderer subPathLineRenderer;

    private void Test()
    {
        Vector3[] pathPositions = new Vector3[pathLineRenderer.positionCount];
        Vector3[] subPathPositions = new Vector3[subPathLineRenderer.positionCount];
        for (int i = 0; i < pathLineRenderer.positionCount; i++)
            pathPositions[i] = pathLineRenderer.GetPosition(i);
        for (int i = 0; i < subPathLineRenderer.positionCount; i++)
            subPathPositions[i] = subPathLineRenderer.GetPosition(i);

        // create rotations
        float[] rotations = new float[pathPositions.Length];
        float rotSpeed = 10f;
        float currAngle = 0f;
        for (int i = 0; i < pathPositions.Length; i++)
        {
            rotations[i] = currAngle;
            currAngle += rotSpeed;
        }
        
        Path path = new Path(pathPositions, rotations, rotations, Vector3.forward);
        Path subPath = new Path(subPathPositions, Vector3.forward);

        int counter = 10000;
        Debug.Log($"SUB PATH AXIS: {subPath.GetMainAxis()}");

        while (path.InsertSubPath(subPath))
        {
            Debug.Log($"SUB PATH AXIS: {subPath.GetMainAxis()}");
            counter--;
            if (counter <= 0)
                break;
        }

        // path.InsertSubPath(subPath);
        // path.InsertSubPath(subPath);
        // path.InsertSubPath(subPath);
        // path.InsertSubPath(subPath);
        
        // draw new path
        GameObject newLine = new GameObject();
        LineRenderer lr = newLine.AddComponent<LineRenderer>();
        lr.SetWidth(0.1f, 0.1f);
        lr.positionCount = path.GetPoints().Length;

        Vector3[] positions = new Vector3[path.GetPoints().Length];
        for (int i = 0; i < path.GetPoints().Length; i++)
            positions[i] = path.GetPoints()[i].pos;
        lr.SetPositions(positions);
        
        // remove old path
        Destroy(pathLineRenderer.gameObject);
        
        // assign new path as path to substitute into
        pathLineRenderer = lr;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Test();
        }
    }
}
