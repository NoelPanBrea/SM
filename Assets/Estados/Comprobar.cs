using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;


public class Comprobar : Estado
{
    public Transform puntoCustodia;
    public Transform puntoCuarentena;
    public Cerebro_ladrón ladron;
    private Cerebro cerebro;
    private UnityEvent<bool> trofeo_robado;
    private NavMeshAgent agent;
    public bool trofeoausente = false;
    public bool comprobado = false;


    void Start()
    {
        trofeo_robado ??= new UnityEvent<bool>();
        cerebro = GetComponent<Cerebro>();
        if (cerebro != null) trofeo_robado.AddListener(cerebro.TrofeoAusente);
        agent = GetComponent<NavMeshAgent>();
    }

    public override void Comportamiento()
    {
        if (!trofeoausente)
        {
            agent.destination = puntoCustodia.position;
        }

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            comprobado = true;
            if ((comprobado && ladron.tieneTesoro) | trofeoausente)
            {
                trofeoausente = true;
                trofeo_robado.Invoke(trofeoausente);
                Vector2 puntoAleatorio = Random.insideUnitCircle * 4f;
                puntoCuarentena.position += new Vector3(puntoAleatorio.x, 0f, puntoAleatorio.y);
                agent.destination = puntoCuarentena.position;
            }
        }
    }
}