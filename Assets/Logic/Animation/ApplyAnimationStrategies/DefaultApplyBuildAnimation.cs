using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultApplyBuildAnimation : ApplyAnimationStrategy
{
    public AnimatedMesh Run(GameObject go)
    {
        AnimatedMeshBuild animationComponent = go.AddComponent<AnimatedMeshBuild>();
        animationComponent.spawn = new GameObject("spawn").transform;
        animationComponent.observer = Camera.main.transform;
        return animationComponent;
    }
}
