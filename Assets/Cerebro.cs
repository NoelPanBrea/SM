using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


// IMPLEMENTAR:
// Cono de visión (HECHO)
// Sensor escuchar, es decir, que se acerque a la zona donde ha oído al ladrón
// Que siga persiguiendo al ladrón 3-4 segundos después de perderlo de vista (HECHO)
// Actualizar zona patrulla a donde dejo de ve el ladrón
// Stamina/resistencia (no hace falta)

public class Cerebro : MonoBehaviour
{
    enum Estado {Patrullar, Investigar, Perseguir, ComprobarTesoro}
    NavMeshAgent agent;
    public List<Vector3> puntos = new List<Vector3>();
    [SerializeField] Transform[] puntosIniciales;
    public Transform PuntoComprobarTesoro;
    public Transform PuntoHuidaLadron;
    public Transform ladron;
    public Cerebro_ladrón LadronSeMueve;
    public Cerebro_ladrón LadrontieneTesoro;    
    public float distanciaVision = 10f;
    public float anguloVision = 90;
    public float distanciaAudicion = 20f;
    public float radioFalloAudicion = 15f;
    int indiceActual = 0;
    float tiempoUltimaVision = -Mathf.Infinity;
    Vector3 ultimaPosicionConocida;
    Vector3 puntoInvestigacion;
    Estado estadoActual = Estado.Patrullar;
    Estado estadoAnterior;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        foreach (Transform t in puntosIniciales)
        {
            puntos.Add(t.position);
        }

        if (puntos.Count == 0)
            return;

        agent.destination = puntos[indiceActual];
    }


    void Update()
    {

        if ((Time.time - tiempoUltimaVision) <= 4f)
        {
            estadoActual = Estado.Perseguir;
        }
        else
        {
            AñadirPuntoPatrulla(ultimaPosicionConocida);
            estadoActual = Estado.Patrullar;
        }

        EjecutarEstado();
    }

    void EjecutarEstado()
    {
        switch (estadoActual)
        {
            case Estado.Perseguir:
                agent.destination = ultimaPosicionConocida;
                break;

            case Estado.Investigar:
                agent.destination = puntoInvestigacion;
                break;

        case Estado.Patrullar:
            Patrullar();
            break;

        case Estado.ComprobarTesoro:
            ComprobarTesoro();
            break;
        }
    }


    void Patrullar()
    {
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            CambiarDestinoAleatorio();
        }
    }


    void CambiarDestinoAleatorio()
    {
        int nuevoIndice = indiceActual;

        while (nuevoIndice == indiceActual)
        {
            nuevoIndice = Random.Range(0, puntos.Count);
        }

        indiceActual = nuevoIndice;
        agent.destination = puntos[indiceActual];
    }

    void AñadirPuntoPatrulla(Vector3 nuevoPunto)   // NUEVA FUNCIÓN
    {
        // evitamos duplicados cercanos
        float distanciaMinima = 8f;
        foreach (Vector3 punto in puntos)
        {
            if (Vector3.Distance(punto, nuevoPunto) < distanciaMinima)
            {
                return;
            }
        }

        // añadimos punto
        puntos.Add(nuevoPunto);
       
        // eliminiamos el más alejado
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

    public void VeAlLadron()
    {
        estadoActual = Estado.Perseguir;
        ultimaPosicionConocida = ladron.position;
        tiempoUltimaVision = Time.time;
    }

    void ComprobarTesoro()
    {
        agent.destination = PuntoComprobarTesoro.position;
        if (LadrontieneTesoro.tieneTesoro)
        {
            agent.destination = PuntoHuidaLadron.position;
        }
    }

    public void EscuchaAlLadron()
    {
        Vector2 puntoAleatorio = Random.insideUnitCircle * radioFalloAudicion;
        puntoInvestigacion = ladron.position + new Vector3(puntoAleatorio.x, 0f, puntoAleatorio.y);
        if (estadoActual != Estado.Perseguir)
        {
            estadoActual = Estado.Investigar;
        }
        agent.destination = puntoInvestigacion;

    }

    public void TocaAlLadron()
    {
        Vector3 direccion = ladron.position - transform.position;
        float distancia = direccion.magnitude;
        if (distancia < 2f)
            {
                Restart();
            }
    }

    void Restart(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}