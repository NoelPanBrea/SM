// using System.Collections.Generic;
// using UnityEngine;


// public class AgenteComunicativo : MonoBehaviour {
//     // lista de agentes que pueden comunicarse
//     private static List<InterfazAgenteComunicativo> agentes = new List<InterfazAgenteComunicativo>();
//     public static void RegistrarAgente(InterfazAgenteComunicativo agente) => agentes.Add(agente);
//     public static void EliminarAgente(InterfazAgenteComunicativo agente) => agentes.Remove(agente);

//     public static void EnviarBroadcast(Mensaje mensaje) {
//         foreach (var agente in agentes) {
//             // no nos enviamos el mensaje a nosotros mismos
//             if (agente.GetGameObject() != mensjae.Emisor) {
//                 agente.RecibirMensaje(mensaje);
//             }
//         }
//     }

//     public static void EnviarDirecto(GameObject receptor, Mensaje mensaje) {
//         InterfazAgenteComunicativo agente = receptor.GetComponent<InterfazAgenteComunicativo>();
//         agente.RecibirMensaje(mensaje);
//     }
// }

// // interfaz para recibir mensajes
// public interface InterfazAgenteComunicativo {
//     void RecibirMensaje(Mensaje mensaje);
//     GameObject GetGameObject();
// }