using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class Patrullar : Estado
{
    NavMeshAgent agent;
    [SerializeField] Transform[] puntosIniciales;
    private List<Vector3> puntos = new List<Vector3>();
    int indiceActual;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   
        foreach (Transform t in puntosIniciales)
        {
            puntos.Add(t.position);
        }
        agent = GetComponent<NavMeshAgent>();
        indiceActual = 0;
    }

    public override void Comportamiento()
    {
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
           int nuevoIndice = indiceActual;

        while (nuevoIndice == indiceActual)
        {
            nuevoIndice = Random.Range(0, puntos.Count);
        }

        indiceActual = nuevoIndice;
        agent.destination = puntos[indiceActual];
        }
    }

    public void AñadirPuntoPatrulla(Vector3 nuevoPunto)
    {
        float distanciaMinima = 8f;
        foreach (Vector3 punto in puntos)
        {
            if (Vector3.Distance(punto, nuevoPunto) < distanciaMinima)
            {
                return;
            }
        }

        puntos.Add(nuevoPunto);
       
        int maxPuntos = 5;
        if (puntos.Count > maxPuntos)
        {
            int indiceMasLejano = 0;
            float maxDist = 0f;
            for (int i = 0; i < puntos.Count; i++)
            {
                float d = Vector3.Distance(puntos[i], nuevoPunto);
                if (d > maxDist)
                {
                    maxDist = d;
                    indiceMasLejano = i;
                }
            }
            puntos.RemoveAt(indiceMasLejano);
        }
    }
}
