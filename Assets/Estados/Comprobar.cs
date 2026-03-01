using UnityEngine;
using UnityEngine.AI;

public class Comprobar : Estado
{
    public Transform puntoCustodia;
    public Transform puntoCuarentena;
    public Cerebro_ladrón ladron;

    private NavMeshAgent agent;
    public bool trofeoausente = false;
    public bool comprobado = false;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public override void Comportamiento()
    {
        if (!trofeoausente)
        {
            agent.destination = puntoCustodia.position;
        }

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            comprobado = true;
            if ((comprobado && ladron.tieneTesoro) | trofeoausente)
            {
                trofeoausente = true;
                Vector2 puntoAleatorio = Random.insideUnitCircle * 4f;
                puntoCuarentena.position += new Vector3(puntoAleatorio.x, 0f, puntoAleatorio.y);
                agent.destination = puntoCuarentena.position;
            }
        }
    }
}