using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="NBody Settings", menuName = "NBody")]
public class NBodyAttributes : ScriptableObject
{
    public static NBodyAttributes Instance { get; private set; }
    NBodyAttributes()
    {
        Instance = this;
    }
    [ColorUsage(false, true)]
    public Color Color;    
    [ColorUsage(false, true)]
    public Color SecondaryColor;
    public float DistanceScaler;
    public float Radius;
    public float TimeScale;
    [Tooltip("x = x frequency, y = y frequency, z = mass multipler")]
    public Vector3 RandomMass;
}
