using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FollowPath : MonoBehaviour
{
    Transform goal;
    float speed = 20f;
    float accuracy = 1.0f;
    float rotSpeed = 2.0f;

    public GameObject wpManager;
    GameObject[] wps;
    GameObject currentNode;
    int currentWP = 0;
    Graph g;
    private Ray ray;
    private RaycastHit rayhit;
    private NavMeshAgent agent;
    private Camera cam;
    private static readonly int ground = 1 << 6;

    private void Start()
    {
        wps = wpManager.GetComponent<WPManager>().waypoints;
        g = wpManager.GetComponent<WPManager>().graph;
        currentNode = wps[0];
        agent = GetComponent<NavMeshAgent>();
        cam = Camera.main;
    }

    public void GoToHeli()
    {
        g.AStar(currentNode, wps[1]);
        currentWP = 0;
    }

    public void GoToRuin()
    {
        g.AStar(currentNode, wps[6]);
        currentWP = 0;
    }

    public void GoToUsine()
    {
        g.AStar(currentNode, wps[10]);
        currentWP = 0;
    }

    private void LateUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ray = cam.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out rayhit, 1000f, ground))
            {
                agent.destination = rayhit.point;
            }
        }
        
        if (g.getPathLength() == 0 || currentWP == g.getPathLength())
            return;

        //O nó que estará mais próximo neste momento
        currentNode = g.getPathPoint(currentWP);

        //se estivermos mais próximo bastante do nó o tanque se moverá para o próximo
        if(Vector3.Distance(g.getPathPoint(currentWP).transform.position, transform.position)< accuracy)
        {
            currentWP++;
        }

        if(currentWP < g.getPathLength())
        {
            goal = g.getPathPoint(currentWP).transform;
            Vector3 lookAtGoal = new Vector3(goal.position.x, this.transform.position.y, goal.position.z);
            Vector3 direction = lookAtGoal - this.transform.position;

            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * rotSpeed);
            transform.position = Vector3.MoveTowards(transform.position, goal.position, speed * Time.deltaTime);   
        }
    }
}
