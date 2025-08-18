using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidUnit : MonoBehaviour
{

    [SerializeField] float speed;
    [SerializeField] float neighbourDistance;
    [SerializeField] private Transform TargetObj;

    bool isreach = false;

    Vector3 targetVec;
    Vector3 obstacleVec;
    
    Boids boids;

    [Header("Layer")]
    [SerializeField] LayerMask neigboursLayers;
    [SerializeField] LayerMask boidUnitLayer;

    List<BoidUnit> neighbours = new List<BoidUnit>();
    List<Color> colors = new List<Color>();

    //Tag설정용 딕셔너리
    Dictionary<int, string> colorTags = new Dictionary<int, string>()
    {{0, "Red"},{1, "Green"},{2, "Blue"}};

    private void Awake()
    {
        colors.Add(Color.red);
        colors.Add(Color.green);
        colors.Add(Color.blue);
  
    }

    public void InitializeUnit(Boids _boids, float _speed)
    {
        boids = _boids;
        speed = _speed;

        Renderer BoidUnitrenderer = GetComponent<Renderer>();

 
            int randomIndex = UnityEngine.Random.Range(0, 3);
            BoidUnitrenderer.material.color = colors[randomIndex];
            string ColorTags = colorTags[randomIndex];
              // Debug.Log("ColorTags : " + ColorTag);

        switch (randomIndex)
            {
                case 0:
                    gameObject.tag = "red";
                    break;
                case 1:
                    gameObject.tag = "green";
                    break;
                case 2:
                    gameObject.tag = "blue";
                    break;
            
           // Debug.Log($"Color: {colors[randomIndex]}, Tag: {gameObject.tag}");
        }

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
        this.transform.rotation = Quaternion.LookRotation(TargetObj.position);
        this.transform.position += targetVec * speed * Time.deltaTime;

        DestroyBoidunit();
    }

    private void FindNeighbour()
    {
        neighbours.Clear();

        Collider[] colls = Physics.OverlapSphere(transform.position, neighbourDistance, boidUnitLayer);

        for (int i = 0; i < colls.Length; i++)
        {
            if (colls[i].GetComponent<BoidUnit>() != null && colls[i].CompareTag(this.tag))
            {
                neighbours.Add(colls[i].GetComponent<BoidUnit>());
            }
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


