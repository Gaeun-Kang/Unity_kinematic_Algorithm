using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidUnit : MonoBehaviour
{

    [SerializeField] float speed;
    [SerializeField] float neighbourDistance;
    [SerializeField] private Transform TargetObj;
    private Color boidcolor;
    bool isreach = false;

    Vector3 targetVec;
    Vector3 obstacleVec;

    
    Boids boids;

    List<BoidUnit> neighbours = new List<BoidUnit>();

    [Header("Layer")]
    [SerializeField] LayerMask neigboursLayers;
    [SerializeField] LayerMask boidUnitLayer;

    public void InitializeUnit(Boids _boids, float _speed)
    {
        boids = _boids;
        speed = _speed;
    }
  
    void Update()
    {
        if (boids == null) return;

        FindNeighbour();

        Vector3 cohesionVec = CohesionVector() * boids.cohesionWeight;
        Vector3 alignmenVec = AlignmentVec() * boids.alignmentWeight;
        Vector3 seperationVec = SeperationVec() * boids.seperationVecWeight;
          
        targetVec = cohesionVec + alignmenVec + seperationVec;

        targetVec = Vector3.Lerp(this.transform.forward,targetVec, speed * Time.deltaTime);
        this.transform.rotation = Quaternion.LookRotation(targetVec);
        this.transform.position += targetVec * speed * Time.deltaTime;

        DestroyBoidunit();
    }

    private void FindNeighbour()
    {
        neighbours.Clear();

        Collider[] colls = Physics.OverlapSphere(transform.position, neighbourDistance, boidUnitLayer);
        //Debug.Log($"[{name}] 발견된 콜라이더 수: {colls.Length}");

        for (int i = 0; i < colls.Length; i++)
        { 
                neighbours.Add(colls[i].GetComponent<BoidUnit>());
        }
    }

    //응집 벡터(이웃들 기준 중간점으로 가는 벡터) 계산 
    private Vector3 CohesionVector()
    {
     
        Vector3 cohesionVec = Vector3.zero;

        if (neighbours.Count > 0)
        {
            for (int i = 0; i < neighbours.Count; i++)
            {
                cohesionVec += neighbours[i].transform.position;
            }
        }
        else
        {
            return cohesionVec;
        }


        //중심 위치 벡터 
        cohesionVec /= neighbours.Count;
        return (cohesionVec - transform.position).normalized;

    }

    //주변 무리가 향하는 방향 : TargetObj(나무)  
    private Vector3 AlignmentVec()
    {
        Vector3 alignmenVec = TargetObj.transform.position;

        if (neighbours.Count > 0)
        {
            for(int i = 0; i < neighbours.Count; i++)
            {
                alignmenVec += neighbours[i].transform.forward;
            }
        }
        else
        {
            alignmenVec = transform.forward;
            return alignmenVec;

        }

        alignmenVec /= neighbours.Count;
        return (alignmenVec.normalized);
    }

    //주변 무리가 향하는 방향 피하기 
    private Vector3 SeperationVec()
    {
        Vector3 seperationVec = Vector3.zero;

        if(neighbours.Count > 0)
        {
            for(int i = 0;i < neighbours.Count;i++)
            {
                seperationVec += (transform.position - neighbours[i].transform.position);
            }
        }
        else
        {
            return seperationVec;
        }

            seperationVec /= neighbours.Count;
            return seperationVec;
    }

    private void DestroyBoidunit()
    {
        if(this.transform.position.x > TargetObj.transform.position.x)
        {
            isreach = true; 
            Debug.Log(this + "stop");

            if (isreach)
            {
                Destroy(gameObject);
            }
        }
    }
}


