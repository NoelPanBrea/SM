using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Perseguir : Estado
{
    public Vector3 posicion;
    public Investigar investigar;
    public Patrullar patrullar;
    private NavMeshAgent agent;
    private Vista vista;
    private float cooldown;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        vista = GetComponent<Vista>();
        investigar = GetComponent<Investigar>();
        patrullar = GetComponent<Patrullar>();
        cooldown = -100;

    }

    public override void Comportamiento()
    {
        agent.destination = posicion;
    }

    public override bool TomarControl()
    {   
        bool visto = vista.VeAlLadron();
        if (!visto & Time.time - cooldown < 3) {
            investigar.cooldown = Time.time;
            patrullar.AñadirPuntoPatrulla(posicion);
        }
        else if (visto) {cooldown = Time.time;}
        posicion = vista.posicionladron.position;
        return visto;
    }
}
