using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.Rendering;
using Unity.Collections;
using UnityEngine.UI;

public struct StarStruct
{
    public Vector3 position;
    public Vector3 velocity;
    public float mass;
}

public class ComputeShaderManager : MonoBehaviour
{

    public static StarStruct[] data;
    public static StarStruct[] outputData;

    RenderTexture blankClear;
    public RenderTexture rTexture;
    public Material material;

    private EntitySpawner spawner;
    public ComputeBuffer buffer;
    public ComputeShader compute;
    public ComputeBuffer pastPositionBuffer;

    int kernal;
    static OctreeOptimization octreeOptimization;
    new Camera camera;
    private void Awake()
    {
        camera = Camera.main;
        octreeOptimization = GetComponent<OctreeOptimization>();
        spawner = FindObjectOfType<EntitySpawner>();
        //data is initialized in entity spawner
        data = new StarStruct[spawner.amount];
        outputData = new StarStruct[spawner.amount];
    }

    private void Start()
    {
        compute.SetFloat("width", camera.pixelWidth);
        compute.SetFloat("height", camera.pixelHeight);
        blankClear = new RenderTexture(camera.pixelWidth, camera.pixelHeight, 24);
    }
    //Method called in EntitySpawner script
    public void InitializeComputeBuffer()
    {
        buffer = new ComputeBuffer(data.Length, 28);
        buffer.SetData(data);
        kernal = compute.FindKernel("CSMain");
        compute.SetBuffer(kernal, "starBuffer", buffer);

        pastPositionBuffer = new ComputeBuffer(data.Length, 8);
        Vector2[] posDatas = new Vector2[data.Length];
        pastPositionBuffer.SetData(posDatas);

        compute.SetBuffer(kernal, "pastPosition", pastPositionBuffer);
        //tempPosBuffer.Dispose();

        compute.SetInt("lengthOfBuffer", data.Length);
        compute.SetMatrix("cameraProjection", camera.projectionMatrix * camera.worldToCameraMatrix);
        compute.SetFloat("farPlane", camera.farClipPlane);
        compute.SetFloat("nearPlane", camera.nearClipPlane);

        rTexture = new RenderTexture(camera.pixelWidth, camera.pixelHeight, 24);
        rTexture.enableRandomWrite = true;
        rTexture.Create();

        compute.SetTexture(kernal, "Result", rTexture);
    }


    private void Update()
    {
        if (!spawner.destroyed)
        {
            //rTexture = new RenderTexture(camera.pixelWidth, camera.pixelHeight, 24);
            //rTexture.enableRandomWrite = true;

            compute.SetMatrix("cameraProjection", camera.projectionMatrix * camera.worldToCameraMatrix);
            compute.SetFloat("farPlane", camera.farClipPlane);
            compute.SetFloat("nearPlane", camera.nearClipPlane);

            GL.Clear(true, true, Color.clear);

            RenderTexture rt = RenderTexture.active;
            RenderTexture.active = rTexture;
            GL.Clear(true, true, Color.clear);
            RenderTexture.active = rt;

            compute.Dispatch(kernal, 256, 1, 1);

            compute.SetTexture(kernal, "Result", rTexture);

            buffer.GetData(outputData);

            material.SetTexture("_BaseMap", rTexture);

        }
        #region octree
        //for (int i = 0; i < data.Length; i++)
        //{
        //    var force = octreeOptimization.RootOctree.CalculateForce(data[i]);
        //    data[i].position += data[i].velocity + force;
        //    data[i].velocity += force;
        //}

        /*

        var dataJob = new JobData();
        var handle = dataJob.Schedule(data.Length, 8);
        handle.Complete();

        octreeOptimization.RootOctree = new Octree();
        octreeOptimization.RootOctree.Size = 500;
        octreeOptimization.RootOctree.particle = new List<StarStruct>();
        octreeOptimization.RootOctree.position = transform.position;

        for (int i = 0; i < data.Length; i++)
        {
            octreeOptimization.RootOctree.Insert(data[i]);
        }*/
        #endregion
    }

    private void LateUpdate()
    {

    }

    struct JobData : IJobParallelFor
    {
        public void Execute(int index)
        {
            var force = octreeOptimization.RootOctree.CalculateForce(data[index]);
            data[index].position += data[index].velocity + force;
            data[index].velocity += force;
        }
    }

    private void OnDisable()
    {
        pastPositionBuffer.Dispose();
        buffer.Dispose();   
    }

}

[BurstCompile]
public class CompSystem : ComponentSystem
{
    public StarStruct[] starStructs;
    protected override void OnUpdate()
    {
        //starStructs = ComputeShaderManager.outputData;
        //starStructs = ComputeShaderManager.data;
        //Entities.ForEach((ref Translation tr, ref StarIndex si) => {
            //tr.Value = new float3(starStructs[si.index].position);
        //});
    }
}



