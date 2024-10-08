using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

[ExecuteAlways]
public class RandomWalker : MonoBehaviour
{

    public Vector3 basePosition = new Vector3(0,0,0);
    public bool pause = true;
    public float stepDistance = 1;
    public float stepsPerSecond = 1;
    public float maximumAngle = 90;
    public bool leaveTrail = true;
    public Material lineMaterial;
    public Collider[] collidersToAvoid;

    private LineRenderer _trail;
    private float _timeSinceLastStep;
    private Vector3? _previousDirection = null;

    public void Start()
    {
        Reset();
        
        
    }

    static readonly float QUAD = .5f * MathF.PI;
    static readonly float TAU = 2f * MathF.PI;
 
    static public Vector2 FromPolarAngle(float theta)
        => new Vector2(MathF.Cos(theta), MathF.Sin(theta));
 
    static public float ToPolarAngle(Vector2 v)
        => Mod(MathF.Atan2(v.y, v.x), TAU);
 
    /// <summary> Conversion from spherical to cartesian coordinates. </summary>
    /// <param name="theta"> Polar angle 0..Tau (top-down). </param>
    /// <param name="phi"> Azimuthal angle -Pi/2..+Pi/2 where 0 represents equator. </param>
    /// <returns> A unit vector. </returns>
    static public Vector3 FromSpherical(float theta, float phi) {
        var th = FromPolarAngle(theta); var ph = FromPolarAngle(QUAD - phi);
        return new Vector3(th.x * ph.y, ph.x, th.y * ph.y);
    }
 
    static public Vector3 FromSpherical(Vector2 coords)
        => FromSpherical(coords.x, coords.y);
 
    /// <summary> Conversion from cartesian to spherical coordinates.
    /// Returns <see langword="true"/> on success, <see langword="false"/>
    /// otherwise. (Values are defined in any case.) </summary>
    /// <param name="spherical"> The resulting spherical unit coordinates. </param>
    /// <param name="magnitude"> Optional magnitude of the input vector.
    /// Leave at 1 when input vector is unit to avoid normalization. </param>
    static public bool ToSpherical(Vector3 v, out Vector2 spherical, float magnitude = 1f) {
        var theta = MathF.Atan2(v.z, v.x);
        theta = theta < 0f? theta + TAU : theta;
        var im = (magnitude == 1f)? 1f : 1f / SubstZero(MathF.Max(0f, magnitude), float.NaN);
        var phi = QUAD - MathF.Acos(v.y * im);
        var success = true;
        if(float.IsNaN(theta)) { theta = 0f; success = false; }
        if(float.IsNaN(phi)) { phi = 0f; success = false; }
        spherical = new Vector2(theta, phi);
        return success;
    }
 
    static public float SubstZero(float v, float subst, float epsilon = 1E-6f) => MathF.Abs(v) < epsilon? subst : v;
    static public float Mod(float n, float m) => (m <= 0f)? 0f : (n %= m) < 0f? n + m : n;
    
    
    static public Vector2 ToLongLat(Vector3 coords, Vector3 center = default) {
        coords -= center;
        ToSpherical(coords, out var spherical, coords.magnitude);
        if(spherical.x < 0f) spherical.x += 2f * MathF.PI;
        return spherical * (180f / MathF.PI);
    }
    
    static public Vector3 FromLongLat(Vector2 longLat, Vector3 center = default, float radius = 1f)
        => center + radius * FromSpherical(longLat * (MathF.PI / 180f));
    
    // step in random direction
    public void Step()
    {

        // initialize ray, params
        Vector3 direction;
        Vector3 origin = transform.position;
        float distance = stepDistance;
        
        if (_previousDirection != null)
        {
            // get previous longitude and latitude
            Vector2 longLat = ToLongLat(_previousDirection.Value);

            // add random angles
            longLat.x += Random.Range(-maximumAngle / 2f, maximumAngle / 2f);
            longLat.y += Random.Range(-maximumAngle / 2f, maximumAngle / 2f);
            
            // transform back to direction
            direction = FromLongLat(longLat);
        }
        else
        {
            direction = Random.onUnitSphere.normalized;
            _previousDirection = direction;
        }


        // cast ray, bouncing off of colliders
        RaycastHit? minHit;
        do
        {
            minHit = null;
            
            // cast ray
            Ray r = new Ray(origin, direction.normalized);
        
            // check if there is a minimal collision point
            float minDist = Mathf.Infinity;
            foreach (Collider c in collidersToAvoid)
            {
                RaycastHit hit;
                if (c.Raycast(r, out hit, distance))
                    if (hit.distance < minDist)
                    {
                        minDist = hit.distance;
                        minHit = hit;
                    }
            }
            
            // if so, move to the collision point and adjust origin/direction/distance to bounce off
            if (minHit != null)
            {
                origin = minHit.Value.point;
                direction = Vector3.Reflect(direction, minHit.Value.normal).normalized;
                distance -= minHit.Value.distance;
                // draw intermediate bounce-off-point
                AddTrailToPosition(origin);
            }
        } while (minHit != null);
        
        // follow final ray that did not collide
        transform.position = origin + direction.normalized * distance;
        AddTrailToPosition(transform.position);
        // save direction
        _previousDirection = direction;
    }

    public void AddTrailToPosition(Vector3 position)
    {
        if (leaveTrail)
        {
            _trail.positionCount += 1;
            _trail.SetPosition(_trail.positionCount - 1, position);    
        }
    }

    public void Reset()
    {
        // try to get trail
        LineRenderer lineComponent = gameObject.GetComponent<LineRenderer>();
        if (lineComponent != null)
            _trail = lineComponent;
        else
        {
            // initialize trail
            _trail = gameObject.AddComponent<LineRenderer>();
        }

        _trail.startWidth = 0.1f;
        _trail.endWidth = 0.1f;
        _trail.SetMaterials(new List<Material>{lineMaterial});
        AddTrailToPosition(transform.position);
        // reset time
        _timeSinceLastStep = 0f;
        
        _trail.positionCount = 0;
        _trail.SetPositions(new Vector3[] {transform.position});
        AddTrailToPosition(basePosition);
        // reset time
        _timeSinceLastStep = 0f;

        transform.position = basePosition;
    }
    
    public void Update()
    {
        if (pause)
            return;
        
        float timePerStep = 1 / stepsPerSecond;
        _timeSinceLastStep += Time.deltaTime;

        while (_timeSinceLastStep > timePerStep)
        {
            _timeSinceLastStep -= timePerStep;
            Step();
        }
    }

    public Path GetPath()
    {
        Vector3[] positions = new Vector3[_trail.positionCount];
        for (int i = 0; i < positions.Length; i++)
            positions[i] = _trail.GetPosition(i);

        Vector3 mainAxis = positions[positions.Length - 1] - positions[0];
        Vector3 estimatedDir = Vector3.up;

        Vector3 ortagonal = Vector3.Cross(mainAxis, estimatedDir);
        Vector3 pathUp = Vector3.Cross(ortagonal, mainAxis);

        return new Path(positions, pathUp.normalized);
    }

    public void SaveAsScriptableObject()
    {
        PathScriptableObject pathSo = ScriptableObject.CreateInstance<PathScriptableObject>();

        Path pathCopy = GetPath().Copy();
        
        pathSo.points = pathCopy.GetPoints();
        pathSo.pathUp = pathCopy.GetUp().normalized;
        
        AssetDatabase.CreateAsset(pathSo, DirConfiguration.Instance.pathScriptableObjectDir + $"RandomWalker_{DateTime.Today.DayOfYear}_{DateTime.Now.Hour}_{DateTime.Now.Minute}.asset");
    }
}
