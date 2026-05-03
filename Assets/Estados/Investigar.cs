using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;


public class Investigar : Estado
{
    public Vector3 posicion;
    private NavMeshAgent agent;
    public Oido oido;
    public float cooldown;
    public bool comunicando;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        oido = GetComponent<Oido>();
        cooldown = -100;
        comunicando = false;
    }

    public override bool TomarControl() 
    {
        // Debug.Log(comunicando);
        bool escuchado = oido.EscuchaAlLadron();
        if (escuchado)
        {
            cooldown = Time.time;
            posicion = comunicando ? posicion : oido.puntoInvestigacion;
        }
        return escuchado || Time.time - cooldown < 10f || comunicando;
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
