using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;



public class Cerebro : MonoBehaviour, InterfazAgenteComunicativo
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
    private Dictionary<GameObject, float> propuestas = new Dictionary<GameObject, float>();
    private int ID_actual = 0;
    private bool esperandoRespuestas = false;

    void Start()
    {
        AgenteComunicativo.RegistrarAgente(this);
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

        Debug.Log(name + " ha visto al ladrón");
        propuestas.Clear();
        ID_actual++;
        esperandoRespuestas = true;

        AgenteComunicativo.EnviarBroadcast(
            new Mensaje
            {
                Emisor = gameObject,
                intencion = Intencion.Cfp,
                Contenido = posicion.x + "," + posicion.y + "," + posicion.z,
                IDConversacion = ID_actual
            });

        Invoke(nameof(SeleccionarGanador), 1.0f);

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

    public void RecibirMensaje(Mensaje mensaje)
    {
        switch (mensaje.intencion)
        {
            case Intencion.Cfp:

                // calculamos primero la distancia del guardián al ladrón (contenido del mensaje)
                ultimaPosicionConocida = LeerVector3(mensaje.Contenido);
                float distancia = Vector3.Distance(transform.position, ultimaPosicionConocida);

                AgenteComunicativo.EnviarDirecto(
                    mensaje.Emisor,
                    new Mensaje
                    {
                        Emisor = gameObject,
                        Receptor = mensaje.Emisor,
                        intencion = Intencion.Propose,
                        Contenido = distancia.ToString(),
                        IDConversacion = mensaje.IDConversacion
                    });

                break;

            case Intencion.AcceptProposal:
            
                estado = perseguir;
                perseguir.posicion = ultimaPosicionConocida;

                Debug.Log(name + " aceptado para perseguir");

                break;

            case Intencion.RejectProposal:

                Debug.Log(name + " rechazado");

                break;

            case Intencion.Propose:

                if (mensaje.IDConversacion == ID_actual)
                {
                    float valor = float.Parse(mensaje.Contenido);
                    propuestas[mensaje.Emisor] = valor;
                    Debug.Log("Propuesta de " + mensaje.Emisor.name + ": " + valor);
                }

                break;
        }
    }

    Vector3 LeerVector3(string texto)
    {
        string[] positions = texto.Split(',');

        return new Vector3(float.Parse(positions[0]), float.Parse(positions[1]), float.Parse(positions[2]));
    }

    void SeleccionarGanador()
    {
        if (!esperandoRespuestas) return;

        esperandoRespuestas = false;

        if (propuestas.Count == 0)
        {
            Debug.Log("Ningún guardián respondió");
            return;
        }

        GameObject mejorGuardian = null;
        float mejorValor = Mathf.Infinity;

        foreach (var propuesta in propuestas)
        {
            if (propuesta.Value < mejorValor)
            {
                mejorValor = propuesta.Value;
                mejorGuardian = propuesta.Key;
            }
        }

        foreach (var propuesta in propuestas)
        {
            if (propuesta.Key == mejorGuardian)
            {
                AgenteComunicativo.EnviarDirecto(
                    propuesta.Key,
                    new Mensaje
                    {
                        Emisor = gameObject,
                        Receptor = propuesta.Key,
                        intencion = Intencion.AcceptProposal,
                        IDConversacion = ID_actual
                    });
            }
            else
            {
                AgenteComunicativo.EnviarDirecto(
                    propuesta.Key,
                    new Mensaje
                    {
                        Emisor = gameObject,
                        Receptor = propuesta.Key,
                        intencion = Intencion.RejectProposal,
                        IDConversacion = ID_actual
                    });
            }
        }

        Debug.Log("Ganador: " + mejorGuardian.name);
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    void OnDestroy()
    {
        AgenteComunicativo.EliminarAgente(this);
    }
}