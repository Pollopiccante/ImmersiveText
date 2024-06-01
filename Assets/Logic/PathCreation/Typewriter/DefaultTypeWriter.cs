using System;
using System.Collections.Generic;
using UnityEngine;

public class DefaultTypeWriter : TypeWriter
{
    private static float _textMeshScaling = 0.1f;

    public override EatResult Eat(MeshTextPath path, string text)
    { 
        // generate text mesh game object, remove first child (alphabet) remove generation script
        GameObject textMeshGo = GameObject.Instantiate(Resources.Load("Simple Helvetica")) as GameObject;
        textMeshGo.name = "TextMeshGo";
        SimpleHelvetica simpleHelveticaScript = textMeshGo.GetComponent<SimpleHelvetica>();
        simpleHelveticaScript.Text = text;
        simpleHelveticaScript.GenerateText();
        Transform alphabetChild = textMeshGo.transform.GetChild(0);
        UnityEngine.Object.DestroyImmediate(alphabetChild.gameObject);
        UnityEngine.Object.DestroyImmediate(simpleHelveticaScript);
        

        // scale mesh children
        foreach (Transform t in textMeshGo.transform)
        {
            t.localScale = new Vector3(_textMeshScaling, _textMeshScaling, _textMeshScaling);
            t.localPosition = new Vector3(t.localPosition.x * _textMeshScaling, 0, 0);
        }
        
        // recalculate center
        Vector3 center = path.GetCenter();
        Vector3 moveToCenter = center - textMeshGo.transform.position;
        textMeshGo.transform.position = center;
        foreach (Transform child in textMeshGo.transform)
        {
            child.localPosition -= moveToCenter;
        }

        // split text into words
        string[] words = text.Split(' ');
        
        // group letters of mesh into words
        LinkedList<Transform> letters = new LinkedList<Transform>();
        for (int i = 1; i < textMeshGo.transform.childCount; i++)
        {
            letters.AddLast(textMeshGo.transform.GetChild(i));
        }
        
        LinkedList<Transform> wordGos = new LinkedList<Transform>();
        foreach (string word in words)
        {
            if (String.IsNullOrEmpty(word.Replace("\n", "")))
                continue;
            if (word.Length > textMeshGo.transform.childCount)
                break;
            
            Transform wordGo = new GameObject(word).transform;
            wordGo.position = textMeshGo.transform.GetChild(0).position;
            foreach (char letter in word)
            {
                Transform letterChild = textMeshGo.transform.GetChild(0);
                letterChild.parent = wordGo;
            }
            wordGos.AddLast(wordGo);
        }
        foreach (Transform wordGo in wordGos)
            wordGo.parent = textMeshGo.transform;

        // push words onto the lines
        int wordsUsed = PushWordMesh(textMeshGo.transform, path);

        // remove overflowing words
        while (textMeshGo.transform.childCount >= wordsUsed + 1)
        {
            UnityEngine.Object.DestroyImmediate(textMeshGo.transform.GetChild(wordsUsed).gameObject);
        }

        // calculate the leftover text
        int cutoffIndex = wordsUsed - 1; // add spaces
        for (int i = 0; i < wordsUsed; i++) // add words
            cutoffIndex += words[i].Length;

        string leftoverText = text.Substring(cutoffIndex);

        // run apply-strategy
        AnimatedMesh animationComponent = animationStrategy.Run(textMeshGo);

        return new EatResult(leftoverText, path, animationComponent) ;
    }

    
    
    public override float GetDistanceBetweenIndex(string inputString, int fromIndex, int toIndex)
    {
        return 0f;
    }

    public override float floatGetHeightOfWord(string inputString)
    {
        return 0f;
    }
    
    public int PushWordMesh(Transform wordTransformParent, MeshTextPath path)
    {
        int wordsUsed = 0;
        MeshTextPath currentPath = path;
        MeshTextPathIterator currentIterator = null;
        float lastX = 0f;
        foreach (Transform wordMesh in wordTransformParent)
        {
            // if no mesh was pushed before, create an iterator and save the last X value of the pushed mesh
            if (lastX < 0 || currentIterator == null)
            {
                lastX = wordMesh.position.x;
                currentIterator = currentPath.IterateDistance();
          
                MeshTextPathElement element = currentIterator.MoveDistance(0f);
                wordMesh.position = element.position;
                wordMesh.rotation = element.rotation;
                wordsUsed++;
            }
            else
            {
                // calculate new distance to travel on the path, set lastX for next round
                float letterXPosition = wordMesh.position.x;
                float newDistance = letterXPosition - lastX;
                lastX = letterXPosition;

                // if the new distance is not on the path segment, go to the next path segment
                if (!currentIterator.HasNextDistance(newDistance))
                {
                    currentPath = currentPath.Next();
                
                    // if there is no next segment, terminate and push the last letters with the fallback strategy
                    if (currentPath == null)
                    {
                        // TODO IMPLEMENT PUSH OF LAST LETTERS
                        
                        // return the number of used words
                        return wordsUsed;
                    }
                    // move to next segment, reset distance to travel to 0f
                    currentIterator = currentPath.IterateDistance();
                    newDistance = 0f;
                }
            
                // push the element
                MeshTextPathElement element = currentIterator.MoveDistance(newDistance);
                if (element != null)
                {
                    wordMesh.position = element.position;
                    wordMesh.rotation = element.rotation;
                    wordsUsed++;
                }
            }
        }
        return wordsUsed;
    }
}
