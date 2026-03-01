using UnityEngine;
using UnityEngine.AI;

public class Investigar : Estado
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
        if (agent.remainingDistance <= agent.stoppingDistance)
            {
                Vector2 puntoAleatorio = Random.insideUnitCircle * 8f;
                posicion += new Vector3(puntoAleatorio.x, 0f, puntoAleatorio.y);
            }
    }
}
