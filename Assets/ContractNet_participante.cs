using UnityEngine;

public class ContractNet_participante : MonoBehaviour, InterfazAgenteComunicativo {
    void Start() => AgenteComunicativo.RegistrarAgente(this);


    private void Responder(GameObject receptor, Intencion intencion, string contenido, int id) {
        Mensaje respuesta = new Mensaje {
            Emisor = gameObject,
            Receptor = receptor,
            intencion = intencion,
            Contenido = contenido,
            IDConversacion = id
        };
        AgenteComunicativo.EnviarDirecto(receptor, respuesta);
    }


    public void RecibirMensaje(Mensaje mensaje) {
        if (mensaje.intencion == Intencion.Cfp) {
            EvaluarPropuesta(mensaje);
        } 
        else if (mensaje.intencion == Intencion.AcceptProposal) {
            Debug.Log($"{gameObject.name}: Acepto tu propuesta de: {mensaje.Contenido}");
            Responder(mensaje.Emisor, Intencion.Inform, "He completado la tarea", mensaje.IDConversacion);
        } 
        else if (mensaje.intencion == Intencion.RejectProposal) {
            Debug.Log($"{gameObject.name}: Rechazo tu propuesta");
        }
    }


    private void EvaluarPropuesta(Mensaje mensaje) {
        // implementar condición para aceptar o cfp
        if (true) {
            Responder(mensaje.Emisor, Intencion.Propose, "Acepto tu cfp. Ofrezco x para la subasta", mensaje.IDConversacion);
        } else {
            Responder(mensaje.Emisor, Intencion.Refuse, "Rechazo tu cfp", mensaje.IDConversacion);
        }
    }


    public GameObject GetGameObject() => gameObject;
}