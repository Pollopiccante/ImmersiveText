using System;
using UnityEngine;

public abstract class AnimatedMesh : MonoBehaviour
{
    public float ElementsPerMinute = 30;

    protected bool Running = false;
    protected float TimeSinceLastElement;

    private Action _finishAction;

    public void RegisterFinishAction(Action action)
    {
        _finishAction = action;
    }
    
    public void RunMeshAnimation()
    {
        Running = true;
    }

    public void PauseMeshAnimation()
    {
        Running = false;
    }
    
    public void SetAnimationSpeed(float elementsPerMinute)
    {
        ElementsPerMinute = elementsPerMinute;
    }

    public abstract void RunningFixedUpdate();
    public abstract void ElementAnimationStep();
    public abstract void RestartAnimation();
    public abstract void JumpToFinishedAnimation();
    public abstract bool IsFinished();

    private void Start()
    {
        RestartAnimation();
    }

    protected void Finish()
    {
        Running = false;
        if (_finishAction != null)
            _finishAction.Invoke();
    }
    
    private void FixedUpdate()
    {
        if (Running)
        {
            RunningFixedUpdate();

            TimeSinceLastElement += Time.fixedDeltaTime;
            float secondsPerSpawn = 60 / ElementsPerMinute;

            while (TimeSinceLastElement > secondsPerSpawn)
            {
                ElementAnimationStep();
                TimeSinceLastElement -= secondsPerSpawn;
            }

            if (IsFinished())
            {
                Finish();
            }
        }
    }
    
}
