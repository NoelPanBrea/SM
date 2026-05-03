using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;



public class Cerebro : MonoBehaviour
{
    public Estado estado;
    public List<Estado> lista_estados;
    public Patrullar patrullar;
    public Investigar investigar;
    public Perseguir perseguir;
    public Comprobar comprobar;
    public Transform PuntoComprobarTesoro;
    public Transform PuntoHuidaLadron;
    public Transform ladron;
    public Cerebro_ladrón LadrontieneTesoro;
    private ComunicacionGuardian comunicacion;
    private float tiempoUltimaVision = -Mathf.Infinity;
    private float tiempoUltimaInvestigación = -Mathf.Infinity;
    private float tiempoUltimaComprobacion;
    private Vector3 ultimaPosicionConocida;
    private Vector3 puntoInvestigacion;
    private NavMeshAgent agent;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        comunicacion = GetComponent<ComunicacionGuardian>();
        patrullar = GetComponent<Patrullar>();
        perseguir = GetComponent<Perseguir>();
        investigar = GetComponent<Investigar>();
        comprobar = GetComponent<Comprobar>();
        lista_estados = new List<Estado> {perseguir, investigar, comprobar, patrullar};
        ultimaPosicionConocida = transform.position;
        tiempoUltimaComprobacion = Time.time;
        estado = patrullar;
    }


    void Update()
    {
        CambiarComportamiento();
    }

    public void CambiarComportamiento()
    {
        foreach (Estado estado in lista_estados)
        {
           if (estado.TomarControl())
           {
                Debug.Log(estado);
                estado.Comportamiento();
                break;
           }
        }
    }

    public void VeAlLadron(Vector3 posicion)
    {
        estado = perseguir;
        ultimaPosicionConocida = posicion;
        tiempoUltimaVision = Time.time;

        comunicacion.CFP_LadronVisto(posicion);
    }


    public void EscuchaAlLadron(Vector3 puntoInvestigacion)
    {
        if (estado is not Perseguir)
        {
            estado = investigar;
            tiempoUltimaInvestigación = Time.time;

            comunicacion.CFP_LadronEscuchado(puntoInvestigacion);
        }
    }


    public void TrofeoAusente(bool trofeo_robado)
    {
        comunicacion.CFP_TesoroRobado(PuntoHuidaLadron.position);
    }


    public void IrAPerseguir(Vector3 pos)
    {
        estado = perseguir;
        perseguir.posicion = pos;
    }


    public void IrAInvestigar(Vector3 pos)
    {
        estado = investigar;
        investigar.posicion = pos;
    }


    public void TocaAlLadron()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    public GameObject GetGameObject()
    {
        return gameObject;
    }
}