using System.Collections.Generic;
using UnityEngine;

public class Pajaro : MonoBehaviour, InterfazAgenteComunicativo
{

    private Dictionary<GameObject, float> propuestas = new Dictionary<GameObject, float>();
    private int ID_actual = 0;
    private bool esperandoRespuestas = false;
    private Dictionary<int, List<Mensaje>> historial_conversaciones = new Dictionary<int, List<Mensaje>>();
    private Mensaje nuevo_mensaje;    // para cuando escribamos un mensaje nuevo podamos almacenarlo


    void Start()
    {
        AgenteComunicativo.RegistrarAgente(this);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void VeAlLadron(Vector3 posicion)
    {
        // Debug.Log("Pájaro ha visto al ladrón");

        propuestas.Clear();
        ID_actual++;
        esperandoRespuestas = true;

        nuevo_mensaje = new Mensaje
            {
                Emisor = gameObject,
                intencion = Intencion.Cfp,
                Contenido = "Ladrón visto" + "|" + posicion.x + ";" + posicion.y + ";" + posicion.z,
                IDConversacion = ID_actual
            };

        RegistrarEnHistorial(nuevo_mensaje);

        AgenteComunicativo.EnviarBroadcast(nuevo_mensaje);

        CancelInvoke("SeleccionarGanador");     // cancelamos selecciones pasadas para que no acepte propuestas antiguas
        Invoke("SeleccionarGanador", 1.0f);
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

                nuevo_mensaje = new Mensaje
                    {
                        Emisor = gameObject,
                        Receptor = propuesta.Key,
                        intencion = Intencion.AcceptProposal,
                        IDConversacion = ID_actual
                    };

                RegistrarEnHistorial(nuevo_mensaje);

                AgenteComunicativo.EnviarDirecto(propuesta.Key, nuevo_mensaje);

                
            }
            else
            {
                nuevo_mensaje = new Mensaje
                    {
                        Emisor = gameObject,
                        Receptor = propuesta.Key,
                        intencion = Intencion.RejectProposal,
                        IDConversacion = ID_actual
                    };

                RegistrarEnHistorial(nuevo_mensaje);

                AgenteComunicativo.EnviarDirecto(propuesta.Key, nuevo_mensaje);
            }
        }

        // Debug.Log("Ganador: " + mejorGuardian.name);
    }

    private void RegistrarEnHistorial(Mensaje mensaje)
    {
        if (!historial_conversaciones.ContainsKey(mensaje.IDConversacion))
        {
            historial_conversaciones[mensaje.IDConversacion] = new List<Mensaje>();
        }
        historial_conversaciones[mensaje.IDConversacion].Add(mensaje);
    }


    public void RecibirMensaje(Mensaje mensaje)
    {
        if (mensaje.intencion == Intencion.Propose &&
            mensaje.IDConversacion == ID_actual)
        {
            float valor = float.Parse(mensaje.Contenido);

            propuestas[mensaje.Emisor] = valor;
            RegistrarEnHistorial(mensaje);

            // Debug.Log("Propuesta de " + mensaje.Emisor.name + ": " + valor);
        }
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