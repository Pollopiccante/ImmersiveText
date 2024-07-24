using UnityEngine;

public class VfxDataPoint
{
    public char letter = '@';
    public Vector3 positionOffset = Vector3.zero;
    public Quaternion rotationOffset = Quaternion.identity;
    public float scale = 1f;
    public Color color = new Color(0, 0, 0);
    public PathStrategy subPathStrategy;
}
