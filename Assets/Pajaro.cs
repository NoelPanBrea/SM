using System.Collections.Generic;
using UnityEngine;

public class Pajaro : MonoBehaviour, InterfazAgenteComunicativo
{

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
        AgenteComunicativo.EnviarBroadcast(
            new Mensaje{Emisor = gameObject, intencion = Intencion.Inform, Contenido = "Ladron visto"});
    }


    public void RecibirMensaje(Mensaje mensaje)
    {
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

