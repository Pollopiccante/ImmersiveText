using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RouteManager : MonoBehaviour
{
    public BookScriptableObject book;

    public Transform[] routeParts;

    private AnimatedMesh[] _animatedMeshes;

    private void Start()
    {
        // build route
        for (int i = 0; i < routeParts.Length; i++)
        {
            if (i + 1 < routeParts.Length)
                routeParts[i].GetComponent<TextLayouter>().RegisterNextLayouter(routeParts[i + 1].GetComponent<TextLayouter>());
            if (i - 1 >= 0)
                routeParts[i].GetComponent<TextLayouter>().RegisterPrevLayouter(routeParts[i - 1].GetComponent<TextLayouter>());
        }
        
        // call standalone elements
        for (int i = 0; i < routeParts.Length; i++)
        {
            routeParts[i].GetComponent<TextLayouter>().BeforeCreate();
        }
        
        // get animated mesh chain
        if (routeParts.Length > 0)
        {
            string text = TextUtil.ToSingleLine(book.chapters[0].content);

            LinkedList<AnimatedMesh> animatedMeshes = routeParts[0].GetComponent<TextLayouter>().CreateAnimatedMeshChain(text);

            // combine all and start
            AnimatedMesh currentAnimatedMesh = animatedMeshes.ElementAt(0);
            for (int i = 1; i < animatedMeshes.Count; i++)
            {
                AnimatedMesh nextAnimatedMesh = animatedMeshes.ElementAt(i);
                currentAnimatedMesh.RegisterFinishAction(() => nextAnimatedMesh.RunMeshAnimation());
                currentAnimatedMesh = nextAnimatedMesh;
            }
            
            animatedMeshes.ElementAt(0).RunMeshAnimation();

            _animatedMeshes = animatedMeshes.ToArray();
        }
    }

    public void SetSpeed(float elementsPerMinute)
    {
        for (int i = 0; i < _animatedMeshes.Length; i++)
        {
            _animatedMeshes[i].ElementsPerMinute = elementsPerMinute;
        }
    }

    public void SkipToEnd()
    {
        for (int i = 0; i < _animatedMeshes.Length; i++)
        {
            _animatedMeshes[i].JumpToFinishedAnimation();
        }
    }

    public void RestartAnimation()
    {
        for (int i = 0; i < _animatedMeshes.Length; i++)
        {
            _animatedMeshes[i].PauseMeshAnimation();
            _animatedMeshes[i].RestartAnimation();
        }
        _animatedMeshes[0].RunMeshAnimation();
    }
}
