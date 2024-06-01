using System;
using UnityEngine;

public class FixedInterpolator : InterpolatorInterface
{
    private Transform _objectToMove;
    private float _duration;
    private Tuple<Vector3, Quaternion> _startPosRot;
    private Tuple<Vector3, Quaternion> _endPosRot;
    private bool _rotationOnly;
    private float _additionalLifetime;
    
    private float _timeAlive;

    public FixedInterpolator(
        Transform objectToMove,
        float duration,
        Tuple<Vector3, Quaternion> startPosRot,
        Tuple<Vector3, Quaternion> endPosRot,
        bool rotationOnly=false,
        float additionalLifetime=0f)
    {
        _objectToMove = objectToMove;
        _duration = duration;
        _startPosRot = startPosRot;
        _endPosRot = endPosRot;
        _rotationOnly = rotationOnly;
        _additionalLifetime = additionalLifetime;
            
        _timeAlive = 0f;
    }

    public bool Execute()
    {
        // update time alive, and progress
        _timeAlive += Time.deltaTime;
        float progress = _timeAlive / _duration;
        
        // check if end is reached / overshot, if so set object to final position, and self destruct
        if (progress >= 1)
        {
            if (!_rotationOnly)
                _objectToMove.position = _endPosRot.Item1;
            _objectToMove.rotation = _endPosRot.Item2;
            // life the additional lifetime in piece :)
            if (_timeAlive - _duration < _additionalLifetime)
                return true;
            
            // until your time runs out!
            return false;
        }
            
        // otherwise interpolate position and rotation
        if (!_rotationOnly)
            _objectToMove.position = Vector3.Lerp(_startPosRot.Item1, _endPosRot.Item1, progress);
        _objectToMove.rotation = Quaternion.Lerp(_startPosRot.Item2, _endPosRot.Item2, progress);
        return true;
    }

    public void Finish()
    {
        if (!_rotationOnly)
            _objectToMove.position = _endPosRot.Item1;
        _objectToMove.rotation = _endPosRot.Item2;
    }
    
    public Transform GetMovedObject()
    {
        return _objectToMove;
    }
    
    public Tuple<Vector3, Quaternion> GetEndTransform()
    {
        return _endPosRot;
    }
    
    public Tuple<Vector3, Quaternion> GetStartTransform()
    {
        return _startPosRot;
    }
}
