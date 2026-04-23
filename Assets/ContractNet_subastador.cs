using UnityEngine;
using System.Collections.Generic;
using System.Net.Mime;

public class AgenteIniciador : MonoBehaviour, InterfazAgenteComunicativo {
    private int ID_actual = 0;
    private bool subastaAbierta = false;
    private List<Mensaje> propuestasRecibidas = new List<Mensaje>();

    void Start() => AgenteComunicativo.RegistrarAgente(this);


    public void IniciarProtocolo() {
        ID_actual++;
        propuestasRecibidas.Clear();
        subastaAbierta = true;

        Mensaje cfp = new Mensaje {
            Emisor = gameObject,
            intencion = Intencion.Cfp,
            Contenido = "Tarea: _____",  //nombre tarea
            IDConversacion = ID_actual
        };

        Debug.Log("Subastador: Enviando CFP a participantes...");
        AgenteComunicativo.EnviarBroadcast(cfp);

        // deberíamos añadir un time.sleep() o algo así
    }


    public void RecibirMensaje(Mensaje mensaje) {
        if ((!subastaAbierta) || (mensaje.IDConversacion != ID_actual)) return;

        if (mensaje.intencion == Intencion.Propose) {
            propuestasRecibidas.Add(mensaje);
            Debug.Log($"Subastador: Propuesta recibida de {mensaje.Emisor.name}");
        }
    }


    private void CerrarProtocolo() {
        subastaAbierta = false;
        if (propuestasRecibidas.Count > 0) {
            // hay que implementar la lógica para elegir la mejor subasta
            Mensaje propuesta_ganadora = propuestasRecibidas[0];
            
            foreach (var propuesta in propuestasRecibidas) {
                Intencion intencion = (propuesta == propuesta_ganadora) ? Intencion.AcceptProposal : Intencion.RejectProposal;
                Enviar_AcceptProposal_o_RejectProposal(propuesta.Emisor, intencion);
            }
            Debug.Log($"Subastador: Ganador elegido: {propuesta_ganadora.Emisor.name}");
        } else {
            Debug.Log("Subastador: No hubo propuestas.");
        }
    }


    private void Enviar_AcceptProposal_o_RejectProposal(GameObject receptor, Intencion intencion) {
        AgenteComunicativo.EnviarDirecto(receptor, new Mensaje {
            Emisor = gameObject,
            Receptor = receptor,
            intencion = intencion,
            Contenido = "Ve a por el ladrón",
            IDConversacion = ID_actual
        });
    }


    public GameObject GetGameObject() => gameObject;
}