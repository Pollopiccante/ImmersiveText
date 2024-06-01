using UnityEngine;

// helper class to interpolate positions
class TransformInterpolator : InterpolatorInterface
{
    private Transform objectToMove;
    private float duration;
    private Transform startTransform;
    private Transform endTransform;
    private bool rotationOnly;
    
    private float timeAlive;
    private float additionalLifetime;
    

    public TransformInterpolator(
        Transform objectToMove,
        float duration,
        Transform startTransform,
        Transform endTransform,
        bool rotationOnly=false,
        float additionalLifetime=0f)
    {
        this.objectToMove = objectToMove;
        this.duration = duration;
        this.startTransform = startTransform;
        this.endTransform = endTransform;
        this.rotationOnly = rotationOnly;
        this.additionalLifetime = additionalLifetime;
            
        timeAlive = 0f;
    }

    public bool Execute()
    {
        // update time alive, and progress
        timeAlive += Time.deltaTime;
        float progress = timeAlive / duration;
        
        // check if end is reached / overshot, if so set object to final position, and self destruct
        if (progress >= 1)
        {
            if (!rotationOnly)
                objectToMove.position = endTransform.position;
            objectToMove.rotation = endTransform.rotation;
            // life the additional lifetime in piece :)
            if (timeAlive - duration < additionalLifetime)
                return true;
            
            // until your time runs out!
            return false;
        }
            
        // otherwise interpolate position and rotation
        if (!rotationOnly)
            objectToMove.position = Vector3.Lerp(startTransform.position, endTransform.position, progress);
        objectToMove.rotation = Quaternion.Lerp(startTransform.rotation, endTransform.rotation, progress);
        return true;
    }

    public void Finish()
    {
        if (!rotationOnly)
            objectToMove.position = endTransform.position;
        objectToMove.rotation = endTransform.rotation;
    }
    
    public Transform GetMovedObject()
    {
        return objectToMove;
    }
    
    public Transform GetEndTransform()
    {
        return endTransform;
    }
    
    public Transform GetStartTransform()
    {
        return startTransform;
    }
}
