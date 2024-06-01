using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionApplyBuildAnimation : ApplyAnimationStrategy
{
    public AnimatedMesh Run(GameObject go)
    {
        AnimatedMeshBuild animationComponent = go.AddComponent<AnimatedMeshBuild>();
        animationComponent.spawn = new GameObject("spawn").transform;
        animationComponent.observer = Camera.main.transform;
        animationComponent.startRotationAfterSeconds = Mathf.Infinity;
        return animationComponent;
    }
}
