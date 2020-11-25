 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class UI : MonoBehaviour
{
    public TextMeshProUGUI bodyText;
    public EntitySpawner entitySpawner;
    public TMP_InputField xRange, yRange, zRange;

    // Start is called before the first frame update
    void Start()
    {
        bodyText.text = "20000 Bodies";
        OffsetX(6.ToString());
        OffsetY(1.ToString());
        OffsetZ(6.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public bool ChangedBeforeSpawn = false;
    public void UpdateBodyText(System.Single value)
    {
        if(ChangedBeforeSpawn == false)
        {
            entitySpawner.OriginalAmount = entitySpawner.amount;
            ChangedBeforeSpawn = true;
        }
        entitySpawner.amount = (int)value;
        bodyText.text = value.ToString() + " Bodies";
    }
    public void SpawnMode(int value)
    {
        entitySpawner.intSpawnMode = value;

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

        Debug.Log("Mode changed to: " + value);
    }
    public void RandomMass(int value)
    {
        if (value == 0)
        {
            entitySpawner.randomMass = true;
        }
        else
        {
            entitySpawner.randomMass = false;
        }
        Debug.Log(value);
    }

    public void OffsetX(string value)
    {
        xRange.text = value;
        entitySpawner.AxisRangeFactor.x = float.Parse(value);
        Debug.Log(value);
    }
    public void OffsetY(string value)
    {
        yRange.text = value;
        entitySpawner.AxisRangeFactor.y = float.Parse(value);
        Debug.Log(value);
    }
    public void OffsetZ(string value)
    {
        zRange.text = value;
        entitySpawner.AxisRangeFactor.z = float.Parse(value);
        Debug.Log(value);
    }

    public void Spawn()
    {
        ChangedBeforeSpawn = false;
        entitySpawner.DestroyAndSpawn();
        Debug.Log("Spawn");
    }

}
