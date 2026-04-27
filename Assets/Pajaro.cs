using System.Collections.Generic;
using UnityEngine;

public class Pajaro : MonoBehaviour, InterfazAgenteComunicativo
{

    private Dictionary<GameObject, float> propuestas = new Dictionary<GameObject, float>();
    private int ID_actual = 0;
    private bool esperandoRespuestas = false;


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
        Debug.Log("Pájaro ha visto al ladrón");

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

        CancelInvoke(nameof(SeleccionarGanador));     // cancelamos selecciones pasadas para que no acepte propuestas antiguas
        Invoke(nameof(SeleccionarGanador), 1.0f);
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

    public void RecibirMensaje(Mensaje mensaje)
    {
        if (mensaje.intencion == Intencion.Propose &&
            mensaje.IDConversacion == ID_actual)
        {
            float valor = float.Parse(mensaje.Contenido);

            propuestas[mensaje.Emisor] = valor;

            Debug.Log("Propuesta de " + mensaje.Emisor.name + ": " + valor);
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