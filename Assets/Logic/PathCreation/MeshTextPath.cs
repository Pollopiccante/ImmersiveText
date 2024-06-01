using System;
using System.Linq;
using UnityEngine;

public class MeshTextPathElement
{
    public Vector3 position;
    public Quaternion rotation;
    
    public MeshTextPathElement(Vector3 position, Quaternion rotation)
    {
        this.position = position;
        this.rotation = rotation;
    }
}

public class MeshTextPath
{
    private MeshTextPath _next;
    private MeshTextPathElement[] _pathElements;
    private float _firstXValue;

    public void RegisterNext(MeshTextPath next)
    {
        _next = next;
    }
    
    public void SetPathElements(MeshTextPathElement[] pathElements)
    {
        if (pathElements.Length < 2)
        {
            throw new ArgumentException("A single point is not a path.");
        }
        _pathElements = pathElements;
    }
    
    public MeshTextPathElement GetEnd()
    {
        MeshTextPath finalPath = this;
        while (finalPath._next != null)
            finalPath = finalPath._next;
        return finalPath._pathElements.Last();
    }

    public MeshTextPathElement GetStart()
    {
        return _pathElements[0];
    }

    public static MeshTextPathBuilder Build()
    {
        return new MeshTextPathBuilder();
    }

    public float GetLength()
    {
        float length = 0f;
        for (int i=1; i>_pathElements.Length; i++)
            length += Vector3.Distance(_pathElements[i].position, _pathElements[i - 1].position);
        return length;
    }
    public MeshTextPath Next()
    {
        return _next;
    }

    public MeshTextPathIterator IterateDistance()
    {
        return new MeshTextPathIterator(_pathElements);
    }

    public MeshTextPathElement[] GetPathElements()
    {
        return _pathElements;
    }

    public Vector3 GetCenter()
    {
        // iterate all positions
        Vector3 pos = new Vector3();
        int counter = 0;

        MeshTextPath currentSegment = this;
        while (currentSegment != null)
        {
            foreach (MeshTextPathElement elem in currentSegment.GetPathElements())
            {
                pos += elem.position;
                counter++;
            }

            currentSegment = currentSegment.Next();
        }
        return pos / counter;
    }
}


public class MeshTextPathBuilder
{
    private MeshTextPath _rootPath;
    private MeshTextPath _currentMeshTextPath;

    public MeshTextPath Next()
    {
        MeshTextPath newPath = new MeshTextPath();

        if (_rootPath == null)
        {
            _rootPath = newPath;
            _currentMeshTextPath = newPath;
            return _rootPath;
        }
        else
        {
            _currentMeshTextPath.RegisterNext(newPath);
            _currentMeshTextPath = newPath;
            return _currentMeshTextPath;
        }
    }

    public MeshTextPath Finish()
    {
        return _rootPath;
    }
}

public class MeshTextPathIterator
{
    private float _distance = 0f;

    private MeshTextPathElement[] _pathElements;
    private int _currentStartElementIndex;
    private float _currentStartElementDistance;
    private float _currentEndElementDistance;
    private float _totalDistance;

    
    public MeshTextPathIterator(MeshTextPathElement[] pathElements)
    {
        _pathElements = pathElements;
        _currentStartElementIndex = 0;
        _currentStartElementDistance = 0f;
        _currentEndElementDistance = Vector3.Distance(pathElements[0].position, pathElements[1].position);
        // calculate total distance
        _totalDistance = 0f;
        for (int i = 0; i + 1 < pathElements.Length; i++)
            _totalDistance += Vector3.Distance(pathElements[i].position, pathElements[i + 1].position);
    }

    public bool HasNextDistance(float distance)
    {
        return _distance + distance <= _totalDistance;
    }
    
    public MeshTextPathElement MoveDistance(float distance)
    {
        _distance += distance;
        
        // iterate until the offset is between the current elements
        while (!(_currentStartElementDistance <= _distance && _distance <= _currentEndElementDistance))
        {
            _currentStartElementIndex++;

            if (_currentStartElementIndex + 1 >= _pathElements.Length)
            {
                return null;
            }
                
            MeshTextPathElement startElement = _pathElements[_currentStartElementIndex];
            MeshTextPathElement endElement = _pathElements[_currentStartElementIndex + 1];

            _currentStartElementDistance = _currentEndElementDistance;
            _currentEndElementDistance += Vector3.Distance(startElement.position, endElement.position);
        }
        if (_currentStartElementIndex + 1 >= _pathElements.Length)
        {
            return null;
        }
        // if new letter mesh is between current two elements
        float overshotDistance = _distance - _currentStartElementDistance;
        float distanceBetween = (_currentEndElementDistance - _currentStartElementDistance);

        // don't divide by zero
        float progressBetween = 0f;
        if (distanceBetween != 0f)
            progressBetween = overshotDistance / (_currentEndElementDistance - _currentStartElementDistance);

        // interpolate position and rotation
        Vector3 letterPosition = Vector3.Lerp(_pathElements[_currentStartElementIndex].position, _pathElements[_currentStartElementIndex + 1].position,
            progressBetween);
        Quaternion letterRotation = Quaternion.Lerp(_pathElements[_currentStartElementIndex].rotation, _pathElements[_currentStartElementIndex + 1].rotation,
            progressBetween);

        return new MeshTextPathElement(letterPosition, letterRotation);
    }

    public float GetDistance()
    {
        return _distance;
    }
    
    public float GetTotalDistance()
    {
        return _totalDistance;
    }
    
}