using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Boids : MonoBehaviour
{
    [SerializeField] private BoidUnit boidUnitPrefab;
    [SerializeField] private float spawnRange = 10;
    
    public int boidCount;

    BoidUnit boldunit;


    [Range(0, 10)] public float cohesionWeight=1;
    [Range (0, 10)]public float alignmentWeight=1;
    [Range(0, 10)] public float seperationVecWeight=1;
   


    void Start()
    {
        for(int i = 0; i < boidCount; i++)
        {
            Vector3 randomvec = Random.insideUnitSphere;
            randomvec *= spawnRange;
            Quaternion randomRot = Quaternion.Euler(0, Random.Range(0, 360f), 0);
            BoidUnit boidUnit = Instantiate(boidUnitPrefab, randomvec, randomRot);
            boidUnit.transform.SetParent(this.transform);
            boidUnit.InitializeUnit(this, Random.Range(10, 10));

        }

    }

}
