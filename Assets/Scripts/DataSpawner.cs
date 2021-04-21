//using Unity.Entities;
using Unity.Mathematics;
//using Unity.Transforms;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;
using Unity.Rendering;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(ComputeShaderManager))]
public class DataSpawner : MonoBehaviour
{
    static public DataSpawner Instance { get; set; }
    [Range(1, 100000)]
    public int Amount = 100;
    public bool UniformMass = false;

    public enum EspawnType
    {
        OrbitBlackHole,
        Box,
    };

    [SerializeField] private SpawnTypeBase SpawnType;
    [SerializeField] private BlackHoleSpawn BlackHole;
    [SerializeField] private BoxSpawn Box;
    //[Tooltip("x = min, y = max")]
    //private Vector2 offsetRange = new Vector2(-100,100);
    //private bool randomVelocity = true;
    //private Vector3 randomFactor;
    //private Vector3 InitialVelocity = new Vector3(0, 0, 0);
    //private float BaseMass = 1;
    [System.Obsolete]
    public Vector3 AxisRangeFactor { get; set; }

    public Vector3 ReferencePoint { get; set; }
    public ComputeShaderManager ComputeManager { get; set; }

    private void Awake()
    {
        Instance = this;
        ReferencePoint = transform.position;
        ComputeManager = GetComponent<ComputeShaderManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        SetSpawnType(EspawnType.OrbitBlackHole);
    }

    public void SetSpawnType(EspawnType type)
    {
        SpawnType = null;
        switch (type)
        {
            case EspawnType.OrbitBlackHole:
                SpawnType = BlackHole;
                break;
            case EspawnType.Box:
                SpawnType = Box;
                break;
        }
    }

    public void DestroyAndSpawn()
    { 
        ComputeManager.DestroyBuffer();
        ComputeManager.CreateBuffer(Amount);
        Spawn();
        ComputeManager.InitializeComputeShader();
    }
    void Spawn()
    {
        SpawnType.Spawn();
    }




    [System.Obsolete]
    public void SetSpawnSettings(EspawnType spawnType, bool randomMass)
    {
        //SpawnMode = spawnType;
        //this.UniformMass = !randomMass;
    }
    [System.Obsolete]
    public void SetSpawnSettings(EspawnType spawnType)
    {
        //SpawnMode = spawnType;
    }
    [System.Obsolete]
    public void SetSpawnSettings(bool randomMass)
    {
        //this.UniformMass = !randomMass;
    }
    [System.Obsolete]
    void SpawnInCube(int amount, Vector2 OffsetRange)
    {
        /*
        for (int i = 0; i < amount; i++)
        {

            float x = UnityEngine.Random.Range(OffsetRange.x, OffsetRange.y) * AxisRangeFactor.x;
            float y = UnityEngine.Random.Range(OffsetRange.x, OffsetRange.y) * AxisRangeFactor.y;
            float z = UnityEngine.Random.Range(OffsetRange.x, OffsetRange.y) * AxisRangeFactor.z;
            Vector3 offset = new Vector3(x, y, z);

            var pos = ComputeShaderManager.Data[i].position = new float3(referencePoint + offset);
            //SetDataMass(ref ComputeShaderManager.Data[i], UniformMass, pos.x, pos.y);
            if (randomVelocity)
            {
                ComputeShaderManager.Data[i].velocity = new Vector3(UnityEngine.Random.Range(-5,5),
                    UnityEngine.Random.Range(-5, 5),
                    UnityEngine.Random.Range(-5, 5));
                ComputeShaderManager.Data[i].velocity.x *= randomFactor.x;
                ComputeShaderManager.Data[i].velocity.z *= randomFactor.z;
                ComputeShaderManager.Data[i].velocity.y *= randomFactor.y;
            }
            else
            {
                ComputeShaderManager.Data[i].velocity = InitialVelocity;
            }
            //entities.Add(_entity);
            //octreeOptimization.RootOctree.Insert(ComputeShaderManager.data[i]);
        }
        */
    }
    [System.Obsolete]
    void SpawnInDisc(int amount, Vector2 OffsetRange)
    {
        /*
        float angle = 0;
        for (int i = 0; i < amount; i++)
        {
            Vector3 direction = transform.forward;
            direction = Quaternion.AngleAxis(angle, Vector3.up) * direction;

            Vector3 offset = referencePoint + direction * UnityEngine.Random.Range(offsetRange.x, offsetRange.y) * AxisRangeFactor.x;
            offset = offset + Vector3.up * UnityEngine.Random.Range(offsetRange.x, offsetRange.y) * AxisRangeFactor.y;

            var pos = ComputeShaderManager.Data[i].position = new float3(referencePoint + offset);
            //SetDataMass(ref ComputeShaderManager.Data[i], UniformMass, pos.x, pos.y);
            var distance = Vector3.Distance(referencePoint, referencePoint + offset);
            ComputeShaderManager.Data[i].velocity = Vector3.Cross(Vector3.up, offset - referencePoint ).normalized * InitialVelocity.x * (Mathf.Pow(1.001f,distance) - 1);

            //octreeOptimization.RootOctree.Insert(ComputeShaderManager.data[i]);
            angle += 360.0f / amount;

            //entities.Add(_entity);

            if (angle >= 360)
            {
                angle = 0;
            }
        }
        */

    }
}

