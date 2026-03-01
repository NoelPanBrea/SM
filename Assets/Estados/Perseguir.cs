using UnityEngine;
using UnityEngine.AI;

public class Perseguir : Estado
{
    public Vector3 posicion;
    private NavMeshAgent agent;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public override void Comportamiento()
    {
        agent.destination = posicion;
    }
}
