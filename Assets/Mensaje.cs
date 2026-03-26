using System.Collections.Generic;
using UnityEngine;

public class Mensaje {
    public Intencion intencion;
    public GameObject Emisor;
    public GameObject Receptor;
    public string Contenido;
    public int IDConversacion;
}


public enum Intencion{
    Cfp, // Call for Proposal
    Propose,  
    AcceptProposal, 
    RejectProposal, 
    Inform,     
    Refuse
}