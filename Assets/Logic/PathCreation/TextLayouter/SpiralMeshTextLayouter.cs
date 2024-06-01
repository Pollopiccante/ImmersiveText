using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpiralMeshTextLayouter : TextLayouter
{
    private static int _numberOfRaycasts = 3000;
    private static Vector3 _directionNormal = new Vector3(0, 1, 0);
    private static float _planeDistance = 2f;

    private Mesh _mesh;

    protected override bool StandaloneCreatable()
    {
        return true;
    }
    public void Awake()
    {
        _mesh = transform.GetComponent<MeshFilter>().mesh;
    }
    protected override MeshTextPath GeneratePath(TypeWriter typeWriter)
    {
        // convert local vert positions to world space
        Vector3[] worldVerts = new Vector3[_mesh.vertices.Length];
        for(int i=0; i < _mesh.vertices.Length; i ++)
        {
            worldVerts[i] = LocalToWorldSpace(_mesh.vertices[i]);
        }
        
        // define direction plane
        Plane directionPlane = new Plane(_directionNormal, Vector3.zero);
        
        // find start and end vert, calculate total height
        Vector3 startVert = new Vector3();
        Vector3 endVert = new Vector3();
        float maxDistance = Mathf.NegativeInfinity;
        float minDistance = Mathf.Infinity;
        foreach (Vector3 vert in worldVerts)
        {
            float distance = directionPlane.GetDistanceToPoint(vert);
            if (distance > maxDistance)
            {
                maxDistance = distance;
                endVert = vert;
            }else if (distance < minDistance)
            {
                minDistance = distance;
                startVert = vert;
            }
        }
        float fullHeight = Vector3.Distance(startVert, endVert);

        // calculate line delimiters and normals with ray casting
        LinkedList<Vector3> linesDelimiters = new LinkedList<Vector3>();
        LinkedList<Vector3> normals = new LinkedList<Vector3>();
        Tuple<Vector3, Plane> previousHitPointPlane = null;
        for (int i = 0; i < _numberOfRaycasts; i++)
        {
            // target of main axis
            float percentage = i / (float)_numberOfRaycasts;
            Vector3 target = Vector3.Lerp(startVert, endVert, percentage);

            // calculate angle
            float currentLayer = percentage * fullHeight / _planeDistance;
            float angle = currentLayer * -360;

            // create ray
            Vector3 axis = endVert - startVert;
            Vector3 direction = Vector3.Cross(Vector3.forward, axis).normalized;
            direction = Quaternion.AngleAxis(angle, _directionNormal) * direction;
            Ray r = new Ray(target, direction);

            // get hit point and plane
            Tuple<Vector3, Plane> hitPointPlane = GetHitPointPlane(r, _mesh.triangles, worldVerts);

            // first normal is added
            if (normals.Count == 0)
            {
                normals.AddLast(hitPointPlane.Item2.normal);
                linesDelimiters.AddLast(hitPointPlane.Item1);
                previousHitPointPlane = hitPointPlane;
                continue;
            }
            // new normal is found, end of line, and beginning of a new one
            if (normals.Last() != hitPointPlane.Item2.normal && previousHitPointPlane != null)
            {
                normals.AddLast(hitPointPlane.Item2.normal);
                linesDelimiters.AddLast(previousHitPointPlane.Item1);
                linesDelimiters.AddLast(hitPointPlane.Item1);
                previousHitPointPlane = hitPointPlane;
                continue;
            }
            // last iteration, finish the last line, even if no new normal is found
            if (i + 1 == _numberOfRaycasts)
            {
                linesDelimiters.AddLast(hitPointPlane.Item1);
            }
            previousHitPointPlane = hitPointPlane;
        }
        
        // calculate quaternions of lines, save as line objects
        MeshTextPathBuilder meshTextPath = MeshTextPath.Build();
        for (int i = 0; i < normals.Count; i++)
        {
            Vector3 normal = normals.ElementAt(i);
            Vector3 delimStart = linesDelimiters.ElementAt(i * 2);
            Vector3 delimEnd = linesDelimiters.ElementAt(i * 2 + 1);
            
            Vector3 textDirection = delimStart - delimEnd;
            Quaternion letterRotation = Quaternion.LookRotation(normal, Vector3.Cross(-normal, textDirection));

            meshTextPath.Next().SetPathElements(new []
            {
                new MeshTextPathElement(delimStart, letterRotation),
                new MeshTextPathElement(delimEnd, letterRotation)
            });
        }

        // hide the mesh
        transform.gameObject.SetActive(false);
        
        return meshTextPath.Finish();

        // TODO IMPLEMENT LINE SHORTENING
        // shorten every line by the planeDistance, also connect all lines as successors
        // LinkedList<Line> outLines = new LinkedList<Line>();
        // foreach (Line line in lines)
        // {
        //     if (line.GetLength() <= _planeDistance)
        //         continue;
        //     line.Shorten(_planeDistance);
        //     // connect successors
        //     if (outLines.Count > 0)
        //         outLines.ElementAt(outLines.Count - 1).RegisterSuccessor(line);
        //
        //     outLines.AddLast(line);
        // }
    }
    
    private Vector3 LocalToWorldSpace(Vector3 local)
    {
        Matrix4x4 localToWorld = transform.localToWorldMatrix;
        return localToWorld.MultiplyPoint3x4(local);
    }
    
    private Tuple<Vector3, Plane> GetHitPointPlane(Ray ray, int[] triangles, Vector3[] verts)
    {
        // returns the plane of the face that is hit by the ray, and the hit point
        // (the ray must be spawned from inside the mesh)
        float minHit = Mathf.Infinity;
        Vector3 hitPoint = new Vector3();
        Plane hitPlane = new Plane();
        int index = 0;
        while (index + 2 < triangles.Length)
        {
            Vector3 vert1 = verts[triangles[index]];
            Vector3 vert2 = verts[triangles[index + 1]];
            Vector3 vert3 = verts[triangles[index + 2]];
            Plane facePlane = new Plane(vert3, vert2, vert1);
            float hit;
            if (facePlane.Raycast(ray, out hit))
            {
                if (hit < minHit && hit > 0)
                {
                    minHit = hit;
                    hitPoint = ray.GetPoint(hit);
                    hitPlane = facePlane;
                }
            }
            index += 3;
        }
        return new Tuple<Vector3, Plane>(hitPoint, hitPlane);
    }
}
