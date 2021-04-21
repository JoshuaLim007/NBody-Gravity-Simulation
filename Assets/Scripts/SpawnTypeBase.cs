using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public abstract class SpawnTypeBase : ScriptableObject
{
    public int Amount => DataSpawner.Instance.Amount;
    public bool UniformMass => DataSpawner.Instance.UniformMass;
    public Vector3 ReferencePoint => DataSpawner.Instance.ReferencePoint;
    public ComputeShaderManager ComputeShaderManager => DataSpawner.Instance.ComputeManager;

    public abstract void Spawn();

    protected void SetDataMass(ref StarStruct data, bool uniformMass = false, float x = 0, float y = 0)
    {
        if (!uniformMass)
        {
            data.mass = 1 + Mathf.PerlinNoise(x / NBodyAttributes.Instance.RandomMass.x, y / NBodyAttributes.Instance.RandomMass.y) * NBodyAttributes.Instance.RandomMass.z;
        }
        else
        {
            //
            data.mass = 1;
        }
    }

}