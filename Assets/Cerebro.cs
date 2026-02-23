using UnityEngine;
using UnityEngine.AI;


// IMPLEMENTAR:
// Cono de visión (HECHO)
// Sensor escuchar, es decir, que se acerque a la zona donde ha oído al ladrón (HECHO)
// Que siga persiguiendo al ladrón 3-4 segundos después de perderlo de vista (HECHO)
// Actualizar zona patrulla a donde dejo de ve el ladrón
// Stamina/resistencia (no hace falta)

public class Cerebro : MonoBehaviour
{
    enum Estado {Patrullar, Investigar, Perseguir}
    NavMeshAgent agent;
    public Transform[] puntos;
    public Transform ladron;
    public Cerebro_ladrón LadronSeMueve;     
    public float distanciaVision = 10f;
    public float anguloVision = 90;
    public float distanciaAudicion = 20f;
    public float radioFalloAudicion = 15f;
    int indiceActual = 0;
    float tiempoUltimaVision = -Mathf.Infinity;
    Vector3 ultimaPosicionConocida;
    Vector3 puntoInvestigacion;
    Estado estadoActual = Estado.Patrullar;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (puntos.Length == 0)
            return;

        agent.destination = puntos[indiceActual].position;
    }


    void Update()
    {
        bool ve = VeAlLadronConAngulo();
        bool oye = EscuchaAlLadron();

        if (ve)
        {
            estadoActual = Estado.Perseguir;
            ultimaPosicionConocida = ladron.position;
            tiempoUltimaVision = Time.time;
        }
        else if (oye)
        {
            Debug.LogError("A");
            estadoActual = Estado.Investigar;
        }
        else if ((Time.time - tiempoUltimaVision) <= 4f)
        {
            estadoActual = Estado.Perseguir;
        }
        else
        {
            estadoActual = Estado.Patrullar;
        }

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
            nuevoIndice = Random.Range(0, puntos.Length);
        }

        indiceActual = nuevoIndice;
        agent.destination = puntos[indiceActual].position;
    }


    bool VeAlLadronConAngulo()
    {
        Vector3 origen = transform.position + Vector3.up * 1.5f; // altura de los "ojos" del guardián
        Vector3 direccion = ladron.position - origen;

        float distancia = direccion.magnitude;

        if (distancia > distanciaVision)
            return false;

        Vector3 direccionNormalizada = direccion.normalized;
        float angulo = Vector3.Angle(transform.forward, direccionNormalizada);
        if (angulo > anguloVision / 2f)
            return false;

        RaycastHit hit;

        if (Physics.Raycast(origen, direccionNormalizada, out hit, distancia))
        {
            if (hit.transform == ladron)
                return true;
        }

        return false;
    }


    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Sonido"))
        {
            Vector2 puntoAleatorio = Random.insideUnitCircle * radioFalloAudicion;
            puntoInvestigacion = ladron.position + new Vector3(puntoAleatorio.x, 0f, puntoAleatorio.y);
            if (estadoActual != Estado.Perseguir)
            {
                estadoActual = Estado.Investigar;
            }
            agent.destination = puntoInvestigacion;

        }
    }
}