using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Orbit Black Hole Settings", menuName = "BlackHoleSpawn")]
public class BlackHoleSpawn : SpawnTypeBase
{
    public int NumberOfBlackHoles = 3;
    public Vector3[] BlackHolePositions;
    public Vector3[] BlackHoleRotations;
    public Vector3[] BlackHoleInitialVelocity;

    public float Radius = 500;
    public float BlackHoleMass = 500000;
    public float HeightDisplacement = 5;
    public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        Vector3 dir = point - pivot; // get point direction relative to pivot
        dir = Quaternion.Euler(angles) * dir; // rotate it
        point = dir + pivot; // calculate rotated point
        return point; // return it
    }

    public override void Spawn()
    {
        int datIndex = 0;

        for (int i = 0; i < NumberOfBlackHoles && i < Amount; i++)
        {
            Vector3 rawBlackHolePos = ReferencePoint + BlackHolePositions[i];
            ComputeShaderManager.Data[i].position = rawBlackHolePos;
            ComputeShaderManager.Data[i].mass = BlackHoleMass;
            ComputeShaderManager.Data[i].velocity = BlackHoleInitialVelocity[i];
            datIndex++;
        }

        int amountPerBlackHole = Mathf.CeilToInt(Amount / NumberOfBlackHoles);
        
        for (int i = 0; i < NumberOfBlackHoles; i++)
        {
            for (int j = 0; j <= amountPerBlackHole && datIndex < Amount; j++)
            {
                var point = UnityEngine.Random.insideUnitCircle;
                point *= Radius;
                Vector3 point3 = new Vector3(point.x, UnityEngine.Random.Range(-HeightDisplacement, HeightDisplacement), point.y);

                Vector3 rawBlackHolePoint = ReferencePoint + BlackHolePositions[i];
                Vector3 rawPoint = point3 + rawBlackHolePoint;
                rawPoint = RotatePointAroundPivot(rawPoint, rawBlackHolePoint, BlackHoleRotations[i]);

                ComputeShaderManager.Data[datIndex].position = rawPoint;
                SetDataMass(ref ComputeShaderManager.Data[datIndex], UniformMass, rawPoint.x, rawPoint.z);

                Vector3 directionToBlackHole = (rawBlackHolePoint - rawPoint).normalized;
                //Vector3 PerpendicularVector = Vector3.Project(directionToBlackHole, -directionToBlackHole);
                Vector3 PerpendicularVector = Vector3.Cross(directionToBlackHole, Quaternion.Euler(BlackHoleRotations[i]) * Vector3.up).normalized;
                Vector3 initialVelocity = PerpendicularVector * Mathf.Sqrt(BlackHoleMass / (Vector3.Distance(rawPoint, rawBlackHolePoint) * NBodyAttributes.Instance.DistanceScaler));

                ComputeShaderManager.Data[datIndex].velocity = initialVelocity + BlackHoleInitialVelocity[i];

                datIndex++;
            }
        }

    }
}
