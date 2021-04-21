using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Mathematics;

[System.Serializable]
[CreateAssetMenu(fileName = "Box Spawn Settings", menuName = "BoxSpawn")]
class BoxSpawn : SpawnTypeBase
{
    public float Scaler;
    public int _BoxCount;
    public Vector3 _BoxHalfExtent;
    public Vector3[] _LocalSpacePositions;
    public bool _RandomVelocity;
    public float _RandomVelocityMag;

    public override void Spawn()
    {
        int amountPerBox = Mathf.CeilToInt(Amount / _BoxCount);
        int dataIndex = 0;
        for (int i = 0; i < _BoxCount; i++)
        {
            for (int j = 0; j <= amountPerBox; j++)
            {
                float x = UnityEngine.Random.Range(-_BoxHalfExtent.x, _BoxHalfExtent.x);
                float y = UnityEngine.Random.Range(-_BoxHalfExtent.y, _BoxHalfExtent.y);
                float z = UnityEngine.Random.Range(-_BoxHalfExtent.z, _BoxHalfExtent.z);
                Vector3 offset = new Vector3(x, y, z);

                var pos = ComputeShaderManager.Data[dataIndex].position = new float3(ReferencePoint + _LocalSpacePositions[i] * Scaler + offset * Scaler);
                SetDataMass(ref ComputeShaderManager.Data[dataIndex], UniformMass, pos.x, pos.z);
                if (_RandomVelocity)
                {
                    ComputeShaderManager.Data[dataIndex].velocity = new Vector3(UnityEngine.Random.Range(-1f, 1f),
                        UnityEngine.Random.Range(-1f, 1f),
                        UnityEngine.Random.Range(-1f, 1f));
                    ComputeShaderManager.Data[dataIndex].velocity.x *= _RandomVelocityMag;
                    ComputeShaderManager.Data[dataIndex].velocity.z *= _RandomVelocityMag;
                    ComputeShaderManager.Data[dataIndex].velocity.y *= _RandomVelocityMag;
                }
                else
                {
                    ComputeShaderManager.Data[dataIndex].velocity = Vector3.zero;
                }
                dataIndex++;
                if(dataIndex >= Amount)
                {
                    return;
                }
            }
        }
        Debug.Log(dataIndex);
        Debug.Log(Amount);

    }
}