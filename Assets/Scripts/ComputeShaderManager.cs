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
    public float forceMag;
}

public class ComputeShaderManager : MonoBehaviour
{
    [SerializeField] private NBodyAttributes settings;
    public static StarStruct[] Data { get; set; }
    //[SerializeField] private static StarStruct[] outputData;
    [SerializeField] private RenderTexture TargetTexture;
    [SerializeField] private Material material;
    [SerializeField] private DataSpawner spawner;
    [SerializeField] private ComputeBuffer currentPositionBuffer;
    [SerializeField] private ComputeShader compute;
    //[SerializeField] private ComputeBuffer pastPositionBuffer;

    int kernal;
    new Camera camera;

    private void Awake()
    {
        camera = Camera.main;
        spawner = FindObjectOfType<DataSpawner>();
        Data = new StarStruct[spawner.Amount];
        InitializeComputeShader();
        //outputData = new StarStruct[spawner.amount];
    }

    private void Start()
    {
        compute.SetFloat("width", camera.pixelWidth);
        compute.SetFloat("height", camera.pixelHeight);
        material.SetFloat("_AlphaClip", 0);
    }

    private void OnDisable()
    {
        DestroyBuffer();
        material.SetFloat("_AlphaClip", 1);
    }

    public void InitializeComputeShader()
    {
        currentPositionBuffer = new ComputeBuffer(Data.Length, 32);
        currentPositionBuffer.SetData(Data);
        kernal = compute.FindKernel("CSMain");
        compute.SetBuffer(kernal, "starBuffer", currentPositionBuffer);
        compute.SetInt("lengthOfBuffer", Data.Length);
        compute.SetMatrix("cameraProjection", camera.projectionMatrix * camera.worldToCameraMatrix);
        compute.SetFloat("farPlane", camera.farClipPlane);
        compute.SetFloat("nearPlane", camera.nearClipPlane);

        TargetTexture = new RenderTexture(camera.pixelWidth, camera.pixelHeight, 24);
        TargetTexture.enableRandomWrite = true;
        TargetTexture.Create();

        compute.SetTexture(kernal, "Result", TargetTexture);
    }
    public void DestroyBuffer()
    {
        currentPositionBuffer.Dispose();
        //pastPositionBuffer.Dispose();
    }
    public void CreateBuffer(int size)
    {
        Data = new StarStruct[size];
        //outputData = new StarStruct[size];
    }

    private void Update()
    {
        compute.SetMatrix("cameraProjection", camera.projectionMatrix * camera.worldToCameraMatrix);
        compute.SetFloat("distanceScaler", settings.DistanceScaler);
        compute.SetVector("color", settings.Color);
        compute.SetVector("secondaryColor", settings.SecondaryColor);
        compute.SetFloat("radius", settings.Radius);
        compute.SetFloat("timeScale", settings.TimeScale);

        RenderTexture rt = RenderTexture.active;
        RenderTexture.active = TargetTexture;
        GL.Clear(true, true, Color.clear);
        RenderTexture.active = rt;

        compute.Dispatch(kernal, 256, 1, 1);

        compute.SetTexture(kernal, "Result", TargetTexture);

        material.SetTexture("_BaseMap", TargetTexture);
    }
}