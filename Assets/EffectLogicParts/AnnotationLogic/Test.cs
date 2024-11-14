#if (UNITY_EDITOR)
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Test : MonoBehaviour
{
    [MenuItem("Mapping/Test")]
    static public void tests()
    {
        Mesh cube = Resources.Load<Mesh>("Meshes/MyCube");
        // input: annotated text
        CompleteAnnotation<DefaultTextDimensionsDataPoint> completeAnnotation = new CompleteAnnotation<DefaultTextDimensionsDataPoint>();

        // letters
        GenericCompoundSection letterSection = ScriptableObject.CreateInstance<GenericCompoundSection>();
        List<GenericSection> letterSectionSubsections = new List<GenericSection>();
        GenericEnumerableSection helloSection = ScriptableObject.CreateInstance<GenericEnumerableSection>();
        helloSection.values = ValueWrapper.FromString(String.Concat(Enumerable.Repeat("Hello", 400)));
        letterSectionSubsections.Add(helloSection);
        letterSection.values = ValueWrapper.FromGenericSections(letterSectionSubsections);
        completeAnnotation.Import<LetterEffectDimension>(letterSection);
        
        // humor
        GenericCompoundSection humorSection = ScriptableObject.CreateInstance<GenericCompoundSection>();
        List<GenericSection> humorSectionSubsections = new List<GenericSection>();
        GenericIdentitySection funnySection = ScriptableObject.CreateInstance<GenericIdentitySection>();
        ValueWrapper floatVW = new ValueWrapper();
        floatVW.floatValue = 2f;
        funnySection.value = floatVW;
        funnySection.length = 500;
        humorSectionSubsections.Add(funnySection);
        humorSection.values = ValueWrapper.FromGenericSections(humorSectionSubsections);
        completeAnnotation.Import<HumorDimension>(humorSection);
        
        // path
        GenericCompoundSection pathSection = ScriptableObject.CreateInstance<GenericCompoundSection>();
        List<GenericSection> pathSectionSubsections = new List<GenericSection>();
        GenericIdentitySection cubeSection = ScriptableObject.CreateInstance<GenericIdentitySection>();
        ValueWrapper cubeVW = new ValueWrapper();
        MeshPathStrategy cubeStrat = ScriptableObject.CreateInstance<MeshPathStrategy>();
        cubeStrat.mesh = cube;
        cubeVW.meshPathStrategyValue = cubeStrat;
        cubeSection.value = cubeVW;
        cubeSection.length = 1000;
        pathSectionSubsections.Add(cubeSection);
        pathSection.values = ValueWrapper.FromGenericSections(pathSectionSubsections);
        completeAnnotation.Import<SubPathEffectDimension>(pathSection);

        GenericCompoundSection meshSection = ScriptableObject.CreateInstance<GenericCompoundSection>();
        GenericCompoundSection angrySection = ScriptableObject.CreateInstance<GenericCompoundSection>();
        
        // completeAnnotation.Import<SubPathEffectDimension>(meshSection);
        completeAnnotation.Import<AngryDimension>(angrySection);

        Path testPath = new Path(new[] {new Vector3(0, 0, 0), new Vector3(3000, 0, 0)});
        
        // create effect
        VFXUtil.CreateEffectFromCompleteAnnotation(completeAnnotation, new DefaultMapping(), testPath);
    }
    
    [MenuItem("Mapping/CreateFractal")]
    static public void CreateFractal()
    {
        Path DragonTurn(Path inputPath, Quaternion rotation)
        {
            Path previousPath = inputPath.Copy();
            
            Vector3 rotationPoint = previousPath.GetPoints().Last().pos;

            Path turnedPath = inputPath.Copy().Rotate(rotation, rotationPoint);

            // complete new line split in two parts
            List<Point> pointsToAppend = turnedPath.GetPoints().Reverse().Skip(1).ToList();
            List<Point> outPoints = new List<Point>(previousPath.Copy().GetPoints());

            // corner points
            List<Point> threeCornerPoints = new List<Point>();
            threeCornerPoints.Add(outPoints[outPoints.Count - 2]);
            threeCornerPoints.Add(outPoints[outPoints.Count - 1]);
            threeCornerPoints.Add(pointsToAppend.First());

            // remove corner points before flattening corner
            outPoints = outPoints.Take(outPoints.Count - 2).ToList();
            pointsToAppend = pointsToAppend.Skip(1).ToList();
            
            // flatten corner
            List<Point> flattenedCorner = new List<Point>();
            flattenedCorner.Add(threeCornerPoints[0]);
            flattenedCorner.Add(Point.Lerp(threeCornerPoints[0], threeCornerPoints[1], 0.5f));
            flattenedCorner.Add(Point.Lerp(threeCornerPoints[1], threeCornerPoints[2], 0.5f));
            flattenedCorner.Add(threeCornerPoints[2]);
            
            // connect everything
            outPoints.AddRange(flattenedCorner);
            outPoints.AddRange(pointsToAppend);
            // calc valid path up
            Vector3 pathUp = Vector3.Cross(new Vector3(1,1,1), outPoints[outPoints.Count - 1].pos - outPoints[0].pos);
            return new Path(outPoints.ToArray(), new List<int>(), pathUp);
        }
        
        
        
        Path path = new Path(new[] {new Vector3(0, 0, 0), new Vector3(10, 0, 0)});

        Quaternion yRotation = Quaternion.FromToRotation(new Vector3(1, 0, 0), new Vector3(0, 0, 1));
        Quaternion zRotation = Quaternion.FromToRotation(new Vector3(1, 0, 0), new Vector3(0, 1, 0));
        Quaternion xzRotation = Quaternion.FromToRotation(new Vector3(1, 0, 0), new Vector3(0, 1, 1));
        
        
        // cool combinations
        List<Quaternion> bigDragon = new List<Quaternion>()
        {
            yRotation, yRotation, yRotation, yRotation,
            zRotation,
            yRotation, yRotation, yRotation, yRotation,
            zRotation, zRotation, zRotation,           
        };
        List<Quaternion> bigDragon2 = new List<Quaternion>()
        {
            yRotation, yRotation, yRotation, 
            zRotation, zRotation, zRotation,
            yRotation, yRotation, yRotation, 
            yRotation, yRotation
        };

        // select config
        List<Quaternion> selectedConfig = bigDragon;
        
        // apply config
        selectedConfig.ForEach(rot =>
        {
            path = DragonTurn(path, rot);
        });
        
        // normalize path to the floor
        float offset = path.GetPoints().Select(p => p.pos.y).Min();
        path = path.Move(new Vector3(0, -offset, 0));
        
        PathScriptableObject pathSo = ScriptableObject.CreateInstance<PathScriptableObject>();
        Path pathCopy = path.Copy();
        
        pathSo.points = pathCopy.GetPoints();
        pathSo.pathUp = new Vector3(0,1,1).normalized;
        pathSo.name = "Fractal";
        //pathSo.LoadToScene();
        
        
        
        AssetDatabase.CreateAsset(pathSo, DirConfiguration.Instance.GetPathScriptableObjectDir() + $"Fractal_{DateTime.Today.DayOfYear}_{DateTime.Now.Hour}_{DateTime.Now.Minute}_{DateTime.Now.Second}.asset");
        
    }
}
#endif