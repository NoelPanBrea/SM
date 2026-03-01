using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;


// IMPLEMENTAR:
// Cono de visión (HECHO)
// Sensor escuchar, es decir, que se acerque a la zona donde ha oído al ladrón
// Que siga persiguiendo al ladrón 3-4 segundos después de perderlo de vista (HECHO)
// Actualizar zona patrulla a donde dejo de ve el ladrón
// Stamina/resistencia (no hace falta)

public class Cerebro : MonoBehaviour
{
    public Estado estado;
    public Patrullar patrullar;
    public Investigar investigar;
    public Perseguir perseguir;
    public Comprobar comprobar;
    public Transform PuntoComprobarTesoro;
    public Transform PuntoHuidaLadron;
    public Transform ladron;
    public Cerebro_ladrón LadrontieneTesoro;
    private float tiempoUltimaVision = -Mathf.Infinity;
    private float tiempoUltimaInvestigación = -Mathf.Infinity;
    private float tiempoUltimaComprobacion;
    private Vector3 ultimaPosicionConocida;
    private Vector3 puntoInvestigacion;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        patrullar = GetComponent<Patrullar>();
        perseguir = GetComponent<Perseguir>();
        investigar = GetComponent<Investigar>();
        comprobar = GetComponent<Comprobar>();
        ultimaPosicionConocida = transform.position;
        tiempoUltimaComprobacion = Time.time;
        estado = patrullar;
    }


    void Update()
    {
        if (Time.time - tiempoUltimaVision > 16f && Time.time - tiempoUltimaInvestigación > 16f
         && Time.time - tiempoUltimaComprobacion > 20f && estado is not Perseguir && estado is not Investigar)
        {
            estado = comprobar;
            tiempoUltimaComprobacion = (comprobar.comprobado & !comprobar.ladron.tieneTesoro) ? Time.time : tiempoUltimaComprobacion;
            comprobar.comprobado = false;
        }
        else if (Time.time - tiempoUltimaVision > 8f && Time.time - tiempoUltimaInvestigación > 8f)
        {
            estado = patrullar;
            patrullar.AñadirPuntoPatrulla(ultimaPosicionConocida);
        }
        else if (estado is not Investigar)
        {
            estado = investigar;
            investigar.posicion = ultimaPosicionConocida;
            tiempoUltimaInvestigación = Time.time;
        }
        estado.Comportamiento();
    }

    public void VeAlLadron(Vector3 posicion)
    {
        estado = perseguir;
        perseguir.posicion = posicion;
        ultimaPosicionConocida = posicion;
        tiempoUltimaVision = Time.time;
    }

    public void EscuchaAlLadron(Vector3 puntoInvestigacion)
    {
        if (estado is not Perseguir)
        {
            estado = investigar;
            investigar.posicion = puntoInvestigacion;
            tiempoUltimaInvestigación = Time.time;
        }


    }

    public void TocaAlLadron()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}