using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EatResult
{
    public EatResult(string leftoverText, MeshTextPath meshTextPath, AnimatedMesh animatedMesh)
    {
        this.leftoverText = leftoverText;
        this.meshTextPath = meshTextPath;
        this.animatedMesh = animatedMesh;
    }
    
    public string leftoverText;
    public MeshTextPath meshTextPath;
    public AnimatedMesh animatedMesh;
}

public abstract class TextLayouter: MonoBehaviour
{
    public TypeWriter _typeWriter;
    
    private TextLayouter _previousLayouter;
    private TextLayouter _nextLayouter;
    private MeshTextPathElement pathStart;
    private MeshTextPathElement pathEnd;
    private MeshTextPath _rawPath;

    public void RegisterPrevLayouter(TextLayouter previousLayouter)
    {
        if (_previousLayouter != null)
            throw new ArgumentException("The previous layouter was already present.");
        _previousLayouter = previousLayouter;
    }

    public void RegisterNextLayouter(TextLayouter nextLayouter)
    {
        if (_nextLayouter != null)
            throw new ArgumentException("The next layouter was already present.");
        _nextLayouter = nextLayouter;
    }
    public TextLayouter GetPreviousLayouter()
    {
        return _previousLayouter;
    }
    
    public TextLayouter GetNextLayouter()
    {
        return _nextLayouter;
    }

    public MeshTextPathElement GetPathStart()
    {
        return pathStart;
    }
    
    public MeshTextPathElement GetPathEnd()
    {
        return pathEnd;
    }
    
   

    // creat a path using text and details of the text meshes provided by a TypeWriter
    protected abstract MeshTextPath GeneratePath(TypeWriter typeWriter);
    protected abstract bool StandaloneCreatable();
    
    public void BeforeCreate()
    {
        if (StandaloneCreatable())
        {
            _rawPath = GeneratePath(_typeWriter);
            pathStart = _rawPath.GetStart();
            pathEnd = _rawPath.GetEnd();    
        }
    }
    public LinkedList<AnimatedMesh> CreateAnimatedMeshChain(string text)
    {
        // output
        LinkedList<AnimatedMesh> outAnimatedMeshes = new LinkedList<AnimatedMesh>();
        
        // let the typewriter eat part of the text
        if (_typeWriter == null)
            _typeWriter = new DefaultTypeWriter();
       
        if (_rawPath == null)
            _rawPath = GeneratePath(_typeWriter);
        EatResult eatResult = _typeWriter.Eat(_rawPath, text);

        // save important values for further generation
        pathStart = eatResult.meshTextPath.GetStart();
        pathEnd = eatResult.meshTextPath.GetEnd();
        
        // append own animated mesh
        outAnimatedMeshes.AddLast(eatResult.animatedMesh);
        
        // delegate further creations to next layouter
        if (_nextLayouter != null)
        {
            LinkedList<AnimatedMesh> furtherAnimatedMeshes = _nextLayouter.CreateAnimatedMeshChain(eatResult.leftoverText);
            outAnimatedMeshes.AddRange(furtherAnimatedMeshes);
        }

        return outAnimatedMeshes;
    }
}
