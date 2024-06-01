using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnimatedMeshBuild : AnimatedMesh
{
    // input
    public Transform observer;
    public Transform spawn;
    public float travelTime = 1f;
    public float timeToTurn = 0.4f;
    public float startRotationAfterSeconds = 10;
    public int _childIndex = 1; // 1 to skip the first child, which will contain help transforms
    
    // internal
    private LinkedList<TransformInterpolator> _interpolators = new LinkedList<TransformInterpolator>();
    private TransformInterpolator _facingInterpolator;
    private TransformInterpolator _facingInterpolatorGuarantor;
    private InterpolatorInterface _finalInterpolator;
    private Transform _transformFather = null;
    private Vector3? _initialPosition = null;
    private Quaternion? _initialRotation = null;
    private int _totalSteps;
    private float _lifeTime = 0f;
    
    
    public override void RunningFixedUpdate()
    {
        _lifeTime += Time.fixedDeltaTime;
        
        // update all interpolators, remove finished ones
        LinkedList<TransformInterpolator> newInterpolators = new LinkedList<TransformInterpolator>();
        foreach (TransformInterpolator interpolator in _interpolators)
        {
            bool success = interpolator.Execute();
            if (success)
            {
                newInterpolators.AddLast(interpolator);
            }
        }
        _interpolators = newInterpolators;

        // create facing interpolator, if there is a new guarantor (first element interpolator in line)
        if (newInterpolators.Count > 0)
        {
            if (newInterpolators.ElementAt(0) != _facingInterpolatorGuarantor)
            {
                _facingInterpolatorGuarantor = newInterpolators.ElementAt(0);

                Quaternion endRot = _facingInterpolatorGuarantor.GetEndTransform().rotation;
                Vector3 endPos = _facingInterpolatorGuarantor.GetEndTransform().position;
            
                // get quaternion vector
                Quaternion rotationDiff =
                    observer.rotation * Quaternion.Inverse(endRot);
                Vector3 positionDiff = (observer.position + observer.forward.normalized * 40) - endPos;
                
                // create end transform that is no child of the base object
                Transform staticStart = new GameObject("staticStart").transform;
                staticStart.position = transform.position;
                staticStart.rotation = transform.rotation;
                Transform dynamicEnd = new GameObject("dynamicEnd").transform;
                dynamicEnd.position = transform.position + positionDiff;
                dynamicEnd.rotation = rotationDiff * transform.rotation;
                dynamicEnd.parent = observer;

                
                _facingInterpolator = new TransformInterpolator(
                    transform,
                    timeToTurn,
                    staticStart,
                    dynamicEnd,
                    true,
                    (60 / ElementsPerMinute) - timeToTurn
                );
                    
            }
        }
        // remove facing interpolator if rotation ban is not through
        if (_lifeTime < startRotationAfterSeconds)
        {
            _facingInterpolator = null;      
        }
        
        // run observer facing interpolator
        if (_facingInterpolator == null || !_facingInterpolator.Execute())
            _facingInterpolator = null;
    }
     
    public override void ElementAnimationStep()
    {
        // do nothing if there are no more children
        if (_childIndex >= transform.childCount)
            return;
        
        // pick next part (child) to build
        Transform childToBuild = transform.GetChild(_childIndex);
        _childIndex++;

        // create goal point at original position
        Transform goalPoint = new GameObject("goal").transform;
        goalPoint.rotation = childToBuild.rotation;
        goalPoint.position = childToBuild.position;
        goalPoint.parent = _transformFather;
        
        // move the part (child) to the spawning position, rotate according to spawn, make it visible
        childToBuild.position = spawn.position;
        childToBuild.rotation = spawn.rotation;
        childToBuild.gameObject.SetActive(true);
        
        // add an interpolator, from spawn to goal
        _interpolators.AddLast(
            new TransformInterpolator(
                childToBuild,
                travelTime,
                spawn,
                goalPoint
            )
        );
    }
    
    public override void RestartAnimation()
    {
        // reset child index
        _childIndex = 1;
        
        // save initial position and rotation to reset to later
        if (_initialPosition == null || _initialRotation == null)
        {
            _initialPosition = transform.position;
            _initialRotation = transform.rotation;
        }
        // reset to initial position and rotation
        else
        {
            transform.position = (Vector3)_initialPosition;
            transform.rotation = (Quaternion)_initialRotation;
        }
        
        // finish all interpolators, remove them after
        foreach (InterpolatorInterface interpolator in _interpolators)
            interpolator.Finish();
        _interpolators = new LinkedList<TransformInterpolator>();
        
        // delete all animation transform children on restart
        if (_transformFather != null)
        {
            DestroyImmediate(_transformFather.gameObject);
            _transformFather = null;
        }
        
        // create first child to hold temporary transforms, if it does not exist already
        if (transform.childCount > 0)
        {
            _transformFather = new GameObject("transform_father").transform;
            _transformFather.parent = transform;
            _transformFather.SetAsFirstSibling();    
        }

        // hide all children
        foreach (Transform c in transform)
        {
            c.gameObject.SetActive(false);
        }
    }
    
    public override void JumpToFinishedAnimation()
    {
        PauseMeshAnimation();
        
        // set all children to visible
        foreach (Transform c in transform)
        {
            c.gameObject.SetActive(true);
        }
        
        // finish all interpolators, remove them after
        foreach (InterpolatorInterface interpolator in _interpolators)
            interpolator.Finish();
        _interpolators = new LinkedList<TransformInterpolator>();
        
        // reset to initial position
        if (_initialPosition != null && _initialRotation != null)
        {
            transform.position = (Vector3)_initialPosition;
            transform.rotation = (Quaternion)_initialRotation;
        }
    }

    public override bool IsFinished()
    {
        // condition to finish, only on last thing before I go
        if (_childIndex >= transform.childCount && _interpolators.Count == 0 && _facingInterpolator == null)
        {
            // check if final rotaton was never left
            if (_lifeTime < startRotationAfterSeconds)
                return true;
            
            // create final interpolator to turn object to original position
            if (_finalInterpolator == null)
            {
                if (_initialRotation != null)
                {
                    _finalInterpolator = new FixedInterpolator(
                        transform,
                        2f,
                        new Tuple<Vector3, Quaternion>(Vector3.zero, transform.rotation),
                        new Tuple<Vector3, Quaternion>(Vector3.zero, (Quaternion)_initialRotation), 
                        true
                    );
                }
                else
                {
                    throw new Exception("No initial rotation was set at the end of the animation chain.");
                }
            }
            // when final interpolator finishes, all animation ceases to exist and the universe ends
            else{
                if (!_finalInterpolator.Execute())
                    return true;
            }
        }

        return false;
    }
}
