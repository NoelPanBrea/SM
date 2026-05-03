using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;


public class Investigar : Estado
{
    public Vector3 posicion;
    private NavMeshAgent agent;
    public Oido oido;
    public float cooldown;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        oido = GetComponent<Oido>();
        cooldown = -100;
    }

    public override bool TomarControl() 
    {
        bool escuchado = oido.EscuchaAlLadron();
        if (escuchado)
        {
            cooldown = Time.time;
            posicion = oido.puntoInvestigacion;
        }
        return escuchado || Time.time - cooldown < 10f;
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
