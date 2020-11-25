using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Burst;
using Unity.Collections;

public class OctreeOptimization : MonoBehaviour
{
    public GameObject testObj;
    public GameObject debugCube;
    public Octree RootOctree;

    private void Awake()
    {
        RootOctree = new Octree();
        RootOctree.Size = 500;
        RootOctree.particle = new List<StarStruct>();
        RootOctree.position = transform.position;
    }

    // Start is called before the first frame update
    void Start()
    {

        //RootOctree.debugCube = debugCube;

        //for (int i = 0; i < 1000; i++)
        //{
            //Vector3 tempPos = new Vector3(Random.Range(-300,300), Random.Range(-300, 300), Random.Range(-300, 300));
            //var temp = new Particle
            //{
            //    mass = 1000,
            //    position = new Vector3(Random.Range(-300, 300), Random.Range(-300, 300), Random.Range(-300, 300))
            //};
            //var tempOBJ = Instantiate(testObj, temp.position, Quaternion.identity);
            //RootOctree.Insert(temp);
        //}

    }

    // Update is called once per frame
    void Update()
    {

    }
}

public class Octree
{
    public float Size;
    public Vector3 position;
    public int capacity = 1;
    public bool divided = false;

    public ArrayList ArrayList;
    public List<StarStruct> particle;

    public Vector3 centerOfMass;
    public float Mass;

    //public GameObject debugCube;

    #region subdivisions
    Octree UpperLeftFront;
    Octree UpperRightFront;
    Octree UpperRightBack;
    Octree UpperLeftBack;
    Octree BottomLeftFront;
    Octree BottomRightFront;
    Octree BottomRightBack;
    Octree BottomLeftBack;
    #endregion

    public Vector3 CalculateForce(StarStruct point)
    {
        Vector3 force = Vector3.zero;
        Vector3 direction;
        if (particle.ToArray().Length == 1)
        {
            direction = particle[0].position - point.position;
            float dist = Vector3.Distance(point.position, particle[0].position);
            float denom = dist * dist;
            float g = point.mass * particle[0].mass / denom;
            force = direction.normalized * g;
        }
        else if(particle.ToArray().Length > 1)
        {
            float r = Vector3.Distance(point.position, centerOfMass);
            float d = Size;
            if(d/r < 0.5f)
            {
                direction = point.position - centerOfMass;
                float denom = r * r;
                float g = Mass * point.mass / denom;
                force = direction.normalized * g;
            }
            else
            {
                force += UpperLeftFront.CalculateForce(point);
                force += UpperRightFront.CalculateForce(point);
                force += UpperRightBack.CalculateForce(point);
                force += UpperLeftBack.CalculateForce(point);
                force += BottomLeftFront.CalculateForce(point);
                force += BottomRightFront.CalculateForce(point);
                force += BottomRightBack.CalculateForce(point);
                force += BottomLeftBack.CalculateForce(point);
            }

        }

        return force;
    }
    public void Insert(StarStruct point)
    {
        if (!InContainer(point))
        {
            return;
        }
        particle.Add(point);

        if (particle.ToArray().Length > capacity)
        {
            if (divided == false)
            {
                SubDivide(point);
                InsertToChild(point);
                divided = true;
            }
            else
            {
                InsertToChild(point);
            }
            particle.Remove(point);
            CalculateCenterOfMass();
        }
        else
        {
            //Debug.Log("dwa");
            centerOfMass = point.position;
            Mass = point.mass;
        }
    }
    public void CalculateCenterOfMass()
    {
        centerOfMass += UpperLeftFront.centerOfMass * UpperLeftFront.Mass;
        Mass += UpperLeftFront.Mass;
        centerOfMass += UpperLeftBack.centerOfMass * UpperLeftBack.Mass;
        Mass += UpperLeftBack.Mass;
        centerOfMass += UpperRightBack.centerOfMass * UpperRightBack.Mass;
        Mass += UpperLeftFront.Mass;
        centerOfMass += UpperRightFront.centerOfMass * UpperRightFront.Mass;
        Mass += UpperRightFront.Mass;

        centerOfMass += BottomLeftBack.centerOfMass * BottomLeftBack.Mass;
        Mass += BottomLeftBack.Mass;
        centerOfMass += BottomRightBack.centerOfMass * BottomRightBack.Mass;
        Mass += BottomRightBack.Mass;
        centerOfMass += BottomLeftFront.centerOfMass * BottomLeftFront.Mass;
        Mass += BottomLeftFront.Mass;
        centerOfMass += BottomRightFront.centerOfMass * BottomRightFront.Mass;
        Mass += BottomRightFront.Mass;

        centerOfMass /= Mass;
    }
    public bool InContainer(StarStruct point)
    {
        return (point.position.x <= position.x + Size && point.position.x >= position.x - Size &&
            point.position.y <= position.y + Size && point.position.y >= position.y - Size &&
            point.position.z <= position.z + Size && point.position.z >= position.z - Size);
    }
    public void SubDivide(StarStruct point)
    {
        #region create subdivisions
        UpperLeftFront = new Octree
        {
            particle = new List<StarStruct>(),
            //debugCube = this.debugCube,
            Size = this.Size / 2,
            capacity = 1,
            position = this.position + Vector3.forward * this.Size / 2 +
            Vector3.up * this.Size / 2 + Vector3.left * this.Size / 2,
        };
        UpperRightFront = new Octree
        {
            particle = new List<StarStruct>(),
            //debugCube = this.debugCube,
            Size = this.Size / 2,
            capacity = 1,
            position = this.position + Vector3.forward * this.Size / 2 +
            Vector3.up * this.Size / 2 + Vector3.right * this.Size / 2,
        };
        UpperRightBack = new Octree
        {
            particle = new List<StarStruct>(),
            //debugCube = this.debugCube,
            Size = this.Size / 2,
            capacity = 1,
            position = this.position + Vector3.back * this.Size / 2 +
            Vector3.up * this.Size / 2 + Vector3.right * this.Size / 2,
        };
        UpperLeftBack = new Octree
        {
            particle = new List<StarStruct>(),
            //debugCube = this.debugCube,
            Size = this.Size / 2,
            capacity = 1,
            position = this.position + Vector3.back * this.Size / 2 +
            Vector3.up * this.Size / 2 + Vector3.left * this.Size / 2,
        };
        BottomLeftFront = new Octree
        {
            particle = new List<StarStruct>(),
            //debugCube = this.debugCube,
            Size = this.Size / 2,
            capacity = 1,
            position = this.position + Vector3.forward * this.Size / 2 +
            Vector3.down * this.Size / 2 + Vector3.left * this.Size / 2,
        };
        BottomRightFront = new Octree
        {
            particle = new List<StarStruct>(),
            //debugCube = this.debugCube,
            Size = this.Size / 2,
            capacity = 1,
            position = this.position + Vector3.forward * this.Size / 2 +
            Vector3.down * this.Size / 2 + Vector3.right * this.Size / 2,
        };
        BottomRightBack = new Octree
        {
            particle = new List<StarStruct>(),
            //debugCube = this.debugCube,
            Size = this.Size / 2,
            capacity = 1,
            position = this.position + Vector3.back * this.Size / 2 +
            Vector3.down * this.Size / 2 + Vector3.right * this.Size / 2,
        };
        BottomLeftBack = new Octree
        {
            particle = new List<StarStruct>(),
            //debugCube = this.debugCube,
            Size = this.Size / 2,
            capacity = 1,
            position = this.position + Vector3.back * this.Size / 2 +
            Vector3.down * this.Size / 2 + Vector3.left * this.Size / 2,
        };
        #endregion
    }
    public void InsertToChild(StarStruct point)
    {
        UpperLeftFront.Insert(point);
        UpperRightFront.Insert(point);
        UpperRightBack.Insert(point);
        UpperLeftBack.Insert(point);

        BottomLeftFront.Insert(point);
        BottomRightFront.Insert(point);
        BottomRightBack.Insert(point);
        BottomLeftBack.Insert(point);
    }
}
