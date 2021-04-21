 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class UI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI bodyText;
    [SerializeField] private DataSpawner entitySpawner;
    [SerializeField] private TMP_InputField xRange, yRange, zRange;

    // Start is called before the first frame update
    void Start()
    {
        bodyText.text = "20000 Bodies";
        //OffsetX(6.ToString());
        //OffsetY(1.ToString());
        //OffsetZ(6.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        else if (Input.GetKey(KeyCode.Period))
        {
            NBodyAttributes.Instance.TimeScale += 0.5f;
        }
        else if (Input.GetKey(KeyCode.Comma))
        {
            NBodyAttributes.Instance.TimeScale -= 0.5f;
            NBodyAttributes.Instance.TimeScale = Mathf.Clamp(NBodyAttributes.Instance.TimeScale, 0, NBodyAttributes.Instance.TimeScale);
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            if (Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift))
            {
                NBodyAttributes.Instance.Radius -= 1;
            }
            else
            {
                NBodyAttributes.Instance.Radius += 1;
            }
        }
        else if (Input.GetKey(KeyCode.L))
        {
            NBodyAttributes.Instance.DistanceScaler += 10;
        }
        else if (Input.GetKey(KeyCode.K))
        {
            NBodyAttributes.Instance.DistanceScaler -= 10;
            NBodyAttributes.Instance.DistanceScaler =  Mathf.Clamp(NBodyAttributes.Instance.DistanceScaler, 10, NBodyAttributes.Instance.DistanceScaler);
        }
    }

    private bool ChangedBeforeSpawn { get; set; }
    [System.Obsolete]
    public void UpdateBodyText(System.Single value)
    {
        if(ChangedBeforeSpawn == false)
        {
            //entitySpawner.OriginalAmount = entitySpawner.amount;
            ChangedBeforeSpawn = true;
        }
        entitySpawner.Amount = (int)value;
        bodyText.text = value.ToString() + " Bodies";
    }
    [System.Obsolete]
    public void SpawnMode(int value)
    {
        entitySpawner.SetSpawnSettings((DataSpawner.EspawnType)value);
        switch (value)
        {
            case 0:
                OffsetX(6.ToString());
                OffsetY(1.ToString());
                OffsetZ(6.ToString());
                break;
            case 1:
                OffsetX(6.ToString());
                OffsetY(6.ToString());
                OffsetZ(6.ToString());
                break;
            case 2:
                OffsetX(4.ToString());
                OffsetY(4.ToString());
                OffsetZ(4.ToString());
                break;
            case 3:
                OffsetX(6.ToString());
                OffsetY(1.ToString());
                OffsetZ(6.ToString());
                break;
        }

        Debug.Log("Mode changed to: " + value.ToString());
    }
    [System.Obsolete]
    public void RandomMass(int value)
    {
        if (value == 0)
        {
            entitySpawner.SetSpawnSettings(true);
        }
        else
        {
            entitySpawner.SetSpawnSettings(false);
        }
        Debug.Log(value);
    }
    [System.Obsolete]
    public void OffsetX(string value)
    {
        xRange.text = value;
        var v = entitySpawner.AxisRangeFactor;
        v.x = float.Parse(value);
        entitySpawner.AxisRangeFactor = v;
        Debug.Log(value);
    }
    [System.Obsolete]
    public void OffsetY(string value)
    {
        yRange.text = value;
        var v = entitySpawner.AxisRangeFactor;
        v.y = float.Parse(value);
        entitySpawner.AxisRangeFactor = v;
        Debug.Log(value);
    }
    [System.Obsolete]
    public void OffsetZ(string value)
    {
        zRange.text = value;
        var v = entitySpawner.AxisRangeFactor;
        v.z = float.Parse(value);
        entitySpawner.AxisRangeFactor = v;
        Debug.Log(value);
    }

    public void SetEntityAmount(System.Single value)
    {
        entitySpawner.Amount = (int)value;
        bodyText.text = value.ToString() + " Bodies";
    }
    public void SetSpawnMode(int value)
    {
        if(value == 0)
        {
            entitySpawner.SetSpawnType(DataSpawner.EspawnType.OrbitBlackHole);
        }
        else
        {
            entitySpawner.SetSpawnType(DataSpawner.EspawnType.Box);
        }
    }
    public void SetUniformMass(int value)
    {
        if (value == 0)
        {
            entitySpawner.UniformMass = true;
        }
        else
        {
            entitySpawner.UniformMass = false;
        }
    }
    public void Spawn()
    {
        ChangedBeforeSpawn = false;
        entitySpawner.DestroyAndSpawn();
        Debug.Log("Spawn");
    }

}
