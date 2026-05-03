using System.Collections.Generic;
using UnityEngine;

public class ComunicacionGuardian : MonoBehaviour, InterfazAgenteComunicativo
{
    private Cerebro cerebro;
    private Dictionary<GameObject, float> propuestas = new Dictionary<GameObject, float>();
    private int ID_actual = 0;
    private bool esperandoRespuestas = false;
    private Dictionary<int, List<Mensaje>> historial_conversaciones = new Dictionary<int, List<Mensaje>>();
    private Mensaje nuevo_mensaje;


    void Start()
    {
        AgenteComunicativo.RegistrarAgente(this);
        cerebro = GetComponent<Cerebro>();
    }


    public void CFP_LadronVisto(Vector3 posicion)
    {
        IniciarCFP("Ladrón visto", posicion);
    }


    public void CFP_LadronEscuchado(Vector3 posicion)
    {
        IniciarCFP("Ladrón escuchado", posicion);
    }


    public void CFP_TesoroRobado(Vector3 posicion)
    {
        IniciarCFP("Tesoro robado", posicion);
    }


    void IniciarCFP(string accion, Vector3 posicion)
    {
        Debug.Log(name + " inicia CFP por: " + accion);

        propuestas.Clear();
        ID_actual++;
        esperandoRespuestas = true;

        nuevo_mensaje = new Mensaje
        {
            Emisor = gameObject,
            intencion = Intencion.Cfp,
            Contenido = accion + "|" + posicion.x + ";" + posicion.y + ";" + posicion.z,
            IDConversacion = ID_actual
        };

        RegistrarEnHistorial(nuevo_mensaje);

        AgenteComunicativo.EnviarBroadcast(nuevo_mensaje);

        CancelInvoke("SeleccionarGanador");     // cancelamos selecciones pasadas para que no acepte propuestas antiguas
        Invoke("SeleccionarGanador", 1.0f);
    }


    public void RecibirMensaje(Mensaje mensaje)
    {
        RegistrarEnHistorial(mensaje);

        switch (mensaje.intencion)
        {
            case Intencion.Cfp:
                ProcesarCFP(mensaje);
                break;

            case Intencion.Propose:
                ProcesarPropose(mensaje);
                break;

            case Intencion.AcceptProposal:
                ProcesarAcceptProposal(mensaje);
                break;

            case Intencion.RejectProposal:
                Debug.Log(name + " rechazado");
                break;
        }
    }


    void ProcesarCFP(Mensaje mensaje)
    {
        if (cerebro.estado is Perseguir)
        {
            nuevo_mensaje = new Mensaje
            {
                Emisor = gameObject,
                Receptor = mensaje.Emisor,
                intencion = Intencion.Refuse,
                IDConversacion = mensaje.IDConversacion
            };

            RegistrarEnHistorial(nuevo_mensaje);
            AgenteComunicativo.EnviarDirecto(mensaje.Emisor, nuevo_mensaje);
            return;
        }

        var (_, posicion) = LeerContenido(mensaje.Contenido);
        float distancia = Vector3.Distance(transform.position, posicion);

        nuevo_mensaje = new Mensaje
        {
            Emisor = gameObject,
            Receptor = mensaje.Emisor,
            intencion = Intencion.Propose,
            Contenido = distancia.ToString(),
            IDConversacion = mensaje.IDConversacion
        };

        RegistrarEnHistorial(nuevo_mensaje);
        AgenteComunicativo.EnviarDirecto(mensaje.Emisor, nuevo_mensaje);
    }


    void ProcesarPropose(Mensaje mensaje)
    {
        if (!esperandoRespuestas) return;
        if (mensaje.IDConversacion != ID_actual) return;

        float valor = float.Parse(mensaje.Contenido);
        propuestas[mensaje.Emisor] = valor;
        Debug.Log("Propuesta de " + mensaje.Emisor.name + ": " + valor);
    }


    void ProcesarAcceptProposal(Mensaje mensaje)
    {
        if (mensaje.IDConversacion != ID_actual) return;

        var (accion, posicion) =
            LeerContenido(historial_conversaciones[ID_actual][0].Contenido);

        if (accion == "Ladrón visto" || accion == "Tesoro robado")
        {
            cerebro.IrAPerseguir(posicion);
        }
        else if (accion == "Ladrón escuchado")
        {
            cerebro.IrAInvestigar(posicion);
        }
    }





    void SeleccionarGanador()
    {
        if (!esperandoRespuestas) return;

        esperandoRespuestas = false;

        if (propuestas.Count == 0) return;

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
            nuevo_mensaje = new Mensaje
            {
                Emisor = gameObject,
                Receptor = propuesta.Key,
                intencion = (propuesta.Key == mejorGuardian)
                    ? Intencion.AcceptProposal
                    : Intencion.RejectProposal,
                IDConversacion = ID_actual
            };

            RegistrarEnHistorial(nuevo_mensaje);
            AgenteComunicativo.EnviarDirecto(propuesta.Key, nuevo_mensaje);
        }
    }


    (string, Vector3) LeerContenido(string contenido)
    {
        string[] partes = contenido.Split("|");
        string accion = partes[0];
        string[] coords = partes[1].Split(';');

        return (accion, new Vector3(float.Parse(coords[0]), float.Parse(coords[1]), float.Parse(coords[2])));
    }


    void RegistrarEnHistorial(Mensaje mensaje)
    {
        if (!historial_conversaciones.ContainsKey(mensaje.IDConversacion))
            historial_conversaciones[mensaje.IDConversacion] = new List<Mensaje>();

        historial_conversaciones[mensaje.IDConversacion].Add(mensaje);
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