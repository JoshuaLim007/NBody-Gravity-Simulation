using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;
using Unity.Rendering;
using System.Collections;
using System.Collections.Generic;

public struct StarIndex : IComponentData
{
    public int index;
}

public class EntitySpawner : MonoBehaviour
{

    [Range(1, 100000)]
    public int amount = 100;


    public enum myEnum // your custom enumeration
    {
        Disc = 1,
        Box = 2,
        FourBox = 3,
        Spiral = 4
    };
    public myEnum SpawnMode;  // this public var should appear as a drop down
    public int intSpawnMode = 0;

    //public GameObject originalObject;
    public Material material;
    public Mesh mesh;
    [Tooltip("x = min, y = max")]
    public Vector2 offsetRange = new Vector2(-100,100);
    public Vector3 AxisRangeFactor = new Vector3(1, 1, 1);
    public bool randomVelocity = true;
    public Vector3 randomFactor;
    public Vector3 InitialVelocity = new Vector3(0, 0, 0);
    public float BaseMass = 1;
    public bool randomMass = false;

    private Transform referencePoint;
    private EntityManager entityManager;
    ComputeShaderManager computeManager;
    OctreeOptimization octreeOptimization;
    public NativeList<Entity> entities;
    private void Awake()
    {
        referencePoint = transform;
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        computeManager = GetComponent<ComputeShaderManager>();
        octreeOptimization = GetComponent<OctreeOptimization>();
    }

    // Start is called before the first frame update
    void Start()
    {
        entities = new NativeList<Entity>(Allocator.Persistent);
        //Entity entity = CreateEntity();
        switch ((int)SpawnMode)
        {
            case 1:
                offsetRange = new Vector2(-50, 50);
                AxisRangeFactor = new Vector3(6, 1, 6);
                randomVelocity = false;
                InitialVelocity.x = 5f;
                BaseMass = 1.7f;
                SpawnInDisc(amount, referencePoint.position, offsetRange);//, entity);
                break;
            case 2:
                offsetRange = new Vector2(-50, 50);
                AxisRangeFactor = new Vector3(6, 6, 6);
                InitialVelocity = Vector3.zero;
                BaseMass = 1;
                SpawnInCube(amount, referencePoint.position, offsetRange);//, entity);
                break;
            case 3:
                offsetRange = new Vector2(-50, 50);
                AxisRangeFactor = new Vector3(4, 4, 4);
                InitialVelocity = Vector3.zero;
                BaseMass = 5;
                SpawnFourBox(amount, referencePoint.position, offsetRange);//, entity);
                break;
            case 4:
                offsetRange = new Vector2(-50, 50);
                AxisRangeFactor = new Vector3(6, 1, 6);
                randomVelocity = false;
                InitialVelocity.x = 2.8f;
                BaseMass = 1;
                SpawnInSpiral(amount, referencePoint.position, offsetRange);//, entity);
                break;

        }

        computeManager.InitializeComputeBuffer();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDisable()
    {
        entities.Dispose();
    }
    public int OriginalAmount = 0;
    public bool destroyed = false;
    public void DestroyAndSpawn()
    {

        //for (int i = 0; i < entities.ToArray().Length; i++)
        //{
        //Debug.Log("entity destroyed");
        //entityManager.DestroyEntity(entities[i]);
        //}    
        destroyed = true;
        //Debug.Log(entities.Length);
        /*
        for (int i = 0; i < entities.Length; i++)
        {
            entityManager.DestroyEntity(entities[i]);
            
        }*/
        //entities.Clear();
        computeManager.buffer.Dispose();
        computeManager.pastPositionBuffer.Dispose();
        ComputeShaderManager.data = new StarStruct[amount];
        ComputeShaderManager.outputData = new StarStruct[amount];

        
        //Entity entity = CreateEntity();
        switch (intSpawnMode)
        {
            case 0:
                offsetRange = new Vector2(-50, 50);
                //AxisRangeFactor = new Vector3(6, 1, 6);
                randomVelocity = false;
                InitialVelocity.x = 5f;
                BaseMass = 1.7f;
                SpawnInDisc(amount, referencePoint.position, offsetRange);//, entity);
                break;
            case 1:
                offsetRange = new Vector2(-50, 50);
                //AxisRangeFactor = new Vector3(6, 6, 6);
                InitialVelocity = Vector3.zero;
                BaseMass = 1;
                SpawnInCube(amount, referencePoint.position, offsetRange);//, entity);
                break;
            case 2:
                offsetRange = new Vector2(-50, 50);
                //AxisRangeFactor = new Vector3(4, 4, 4);
                InitialVelocity = Vector3.zero;
                BaseMass = 5;
                SpawnFourBox(amount, referencePoint.position, offsetRange);//, entity);
                break;
            case 3:
                offsetRange = new Vector2(-50, 50);
                //AxisRangeFactor = new Vector3(6, 1, 6);
                randomVelocity = false;
                InitialVelocity.x = 2.8f;
                BaseMass = 1;
                SpawnInSpiral(amount, referencePoint.position, offsetRange);//, entity);
                break;
        }

        computeManager.InitializeComputeBuffer();
        destroyed = false;
    }

    Entity CreateEntity()
    {
        EntityArchetype entityArchetype = entityManager.CreateArchetype(
            typeof(Translation),
            typeof(RenderMesh),
            typeof(LocalToWorld),
            typeof(RenderBounds),
            typeof(Rotation),
            typeof(StarIndex)
        );
        Entity entity = entityManager.CreateEntity(entityArchetype);
        entityManager.SetSharedComponentData(entity, new RenderMesh
        {
            mesh = mesh,
            material = material
        });

        return entity;
    }

    void SpawnInCube(int amount, Vector3 referencePoint, Vector2 OffsetRange)//, Entity entity)
    {
        for (int i = 0; i < amount; i++)
        {

            float x = UnityEngine.Random.Range(OffsetRange.x, OffsetRange.y) * AxisRangeFactor.x;
            float y = UnityEngine.Random.Range(OffsetRange.x, OffsetRange.y) * AxisRangeFactor.y;
            float z = UnityEngine.Random.Range(OffsetRange.x, OffsetRange.y) * AxisRangeFactor.z;
            Vector3 offset = new Vector3(x, y, z);

            /*
            var _entity = entityManager.Instantiate(entity);
            entityManager.SetComponentData(_entity, new Translation
            {
                Value = new float3(referencePoint + offset)
            });
            entityManager.SetComponentData(_entity, new StarIndex
            {
                index = i 
            });*/

            var pos = ComputeShaderManager.data[i].position = new float3(referencePoint + offset);
            if (randomMass)
            {
                ComputeShaderManager.data[i].mass = BaseMass * 0.1f * (Mathf.PerlinNoise(pos.x / 800, pos.y / 800) + 0.1f) * 5.0f;
            }
            else
            {
                ComputeShaderManager.data[i].mass = BaseMass * 0.1f;
            }
            if (randomVelocity)
            {
                ComputeShaderManager.data[i].velocity = new Vector3(UnityEngine.Random.Range(-5,5),
                    UnityEngine.Random.Range(-5, 5),
                    UnityEngine.Random.Range(-5, 5));
                ComputeShaderManager.data[i].velocity.x *= randomFactor.x;
                ComputeShaderManager.data[i].velocity.z *= randomFactor.z;
                ComputeShaderManager.data[i].velocity.y *= randomFactor.y;
            }
            else
            {
                ComputeShaderManager.data[i].velocity = InitialVelocity;
            }
            //entities.Add(_entity);
            //octreeOptimization.RootOctree.Insert(ComputeShaderManager.data[i]);
        }
    }

    void SpawnInDisc(int amount, Vector3 referencePoint, Vector2 OffsetRange)//, Entity entity)
    {
        float angle = 0;
        for (int i = 0; i < amount; i++)
        {
            Vector3 direction = transform.forward;
            direction = Quaternion.AngleAxis(angle, Vector3.up) * direction;

            Vector3 offset = referencePoint + direction * UnityEngine.Random.Range(offsetRange.x, offsetRange.y) * AxisRangeFactor.x;
            offset = offset + Vector3.up * UnityEngine.Random.Range(offsetRange.x, offsetRange.y) * AxisRangeFactor.y;

            /*
            var _entity = entityManager.Instantiate(entity);

            entityManager.SetComponentData(_entity, new Translation
            {
                Value = new float3(referencePoint + offset)
            });
            entityManager.SetComponentData(_entity, new StarIndex
            {
                index = i
            });*/

            var pos = ComputeShaderManager.data[i].position = new float3(referencePoint + offset);
            if (randomMass)
            {
                ComputeShaderManager.data[i].mass = BaseMass * 0.1f * (Mathf.PerlinNoise(pos.x / 800, pos.y / 800) + 0.1f) * 5.0f;
            }
            else
            {
                ComputeShaderManager.data[i].mass = BaseMass * 0.1f;
            }
            var distance = Vector3.Distance(referencePoint, referencePoint + offset);
            ComputeShaderManager.data[i].velocity = Vector3.Cross(Vector3.up, offset - referencePoint ).normalized * InitialVelocity.x * (Mathf.Pow(1.001f,distance) - 1);

            //octreeOptimization.RootOctree.Insert(ComputeShaderManager.data[i]);
            angle += 360.0f / amount;

            //entities.Add(_entity);

            if (angle >= 360)
            {
                angle = 0;
            }
        }
    }

    void SpawnFourBox(int amount, Vector3 referencePoint, Vector2 OffsetRange)//, Entity entity)
    {
        Vector3[] positions = new Vector3[4];
        positions[0] = referencePoint + (transform.forward + transform.right) * 300;
        positions[1] = referencePoint + (transform.forward - transform.right) * 300;
        positions[2] = referencePoint + (-transform.forward + transform.right) * 300;
        positions[3] = referencePoint + (-transform.forward - transform.right) * 300;

        int quarterPoint = amount / 4;
        int rightEnd = quarterPoint;
        int leftOff = 0;
        for (int i = 0; i < 4; i++)
        {
            Debug.Log(leftOff.ToString() + ",to," + rightEnd.ToString());
            int d = 0;

            for (d = leftOff; d < rightEnd; d++)
            {
                //var _entity = entityManager.Instantiate(entity);
                //entities.Add(_entity);
                float x = UnityEngine.Random.Range(OffsetRange.x, OffsetRange.y) * AxisRangeFactor.x;
                float y = UnityEngine.Random.Range(OffsetRange.x, OffsetRange.y) * AxisRangeFactor.y ;
                float z = UnityEngine.Random.Range(OffsetRange.x, OffsetRange.y) * AxisRangeFactor.z;
                Vector3 offset = new Vector3(x, y, z);
                /*
                entityManager.SetComponentData(_entity, new Translation
                {
                    Value = new float3(positions[i] + offset)
                });
                entityManager.SetComponentData(_entity, new StarIndex
                {
                    index = d
                });*/

                //ComputeShaderManager.data[d].position = new float3(positions[i] + offset);
                var pos = ComputeShaderManager.data[d].position = new float3(positions[i] + offset);
                if (randomMass)
                {
                    ComputeShaderManager.data[d].mass = BaseMass * 0.1f * (Mathf.PerlinNoise(pos.x / 800, pos.y / 800) + 0.1f) * 5.0f;
                }
                else
                {
                    ComputeShaderManager.data[d].mass = BaseMass * 0.1f;
                }
                ComputeShaderManager.data[d].velocity = Vector3.zero;

            }

            leftOff = rightEnd;
            rightEnd += quarterPoint;
            //rightEnd = Mathf.Clamp(rightEnd, 0, amount);
            if(i == 2)
            {
                rightEnd = amount;
            }


        }

    }

    void SpawnInSpiral(int amount, Vector3 referencePoint, Vector2 OffsetRange)//, Entity entity)
    {
        float angle = 0;
        for (int i = 0; i < amount; i++)
        {
            Vector3 direction = transform.forward;
            direction = Quaternion.AngleAxis(angle, Vector3.up) * direction;

            Vector3 offset = referencePoint + direction * UnityEngine.Random.Range(offsetRange.x, offsetRange.y) * AxisRangeFactor.x;
            offset = offset + Vector3.up * UnityEngine.Random.Range(offsetRange.x, offsetRange.y) * AxisRangeFactor.y;

            /*
            var _entity = entityManager.Instantiate(entity);
            entityManager.SetComponentData(_entity, new Translation
            {
                Value = new float3(referencePoint + offset)
            });
            entityManager.SetComponentData(_entity, new StarIndex
            {
                index = i
            });
            entities.Add(_entity);
            */

            ComputeShaderManager.data[i].position = new float3(referencePoint + offset);
            var pos = ComputeShaderManager.data[i].position = new float3(referencePoint + offset);
            if (randomMass)
            {
                ComputeShaderManager.data[i].mass = BaseMass * 0.1f * (Mathf.PerlinNoise(pos.x / 800, pos.y / 800) + 0.1f) * 5.0f;
            }
            else
            {
                ComputeShaderManager.data[i].mass = BaseMass * 0.1f;
            }

            var distance = Vector3.Distance(referencePoint, referencePoint + offset);
            ComputeShaderManager.data[i].velocity = Vector3.Cross(Vector3.up, offset - referencePoint).normalized * InitialVelocity.x * (Mathf.Pow(1.001f, distance) - 1);

            //octreeOptimization.RootOctree.Insert(ComputeShaderManager.data[i]);
            angle += amount / 360.0f;

            if (angle >= 360)
            {
                angle = 0;
            }
        }
    }

}

