using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BasicTransitionLayouter : TextLayouter
{
    public int steps;
    
    protected override bool StandaloneCreatable()
    {
        return false;
    }
    protected override MeshTextPath GeneratePath(TypeWriter typeWriter)
    {
        _typeWriter.animationStrategy = new TransitionApplyBuildAnimation();
        
        // get start and endpoint 
        MeshTextPathElement start = GetPreviousLayouter().GetPathEnd();
        MeshTextPathElement end = GetNextLayouter().GetPathStart();
        
        // generate direction rotation
        Vector3 direction = end.position - start.position;
        Vector3 normal = Vector3.Cross(direction, Vector3.down);
        
        Quaternion rotation = Quaternion.LookRotation(-normal, Vector3.Cross(-normal, direction));

        
        // iterate by steps, create single line path
        MeshTextPathBuilder builder = MeshTextPath.Build();
        LinkedList<MeshTextPathElement> elements = new LinkedList<MeshTextPathElement>();
        for (int i = 0; i < steps; i++)
        {
            float progress = i / (float)steps;
            Vector3 position = Vector3.Lerp(start.position, end.position, progress);

            elements.AddLast(new MeshTextPathElement(position, rotation));
        }
        builder.Next().SetPathElements(elements.ToArray());
        return builder.Finish();
    }
}
