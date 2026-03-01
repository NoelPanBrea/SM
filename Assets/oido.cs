using UnityEngine;
using UnityEngine.Events;

public class Oido : MonoBehaviour
{
    public float radioFalloAudicion = 15f;
    private UnityEvent<Vector3> escuchado;
    private Cerebro cerebro;
    private Vector3 puntoInvestigacion;


    void Start()
    {   
        escuchado ??= new UnityEvent<Vector3>();
        cerebro = GetComponent<Cerebro>();
        if (cerebro != null) escuchado.AddListener(cerebro.EscuchaAlLadron);
    }

    void Update()
    {
        
    }

    void OnTriggerStay(Collider other)
    {

        if (other.CompareTag("Sonido") && other.GetComponent<Cerebro_ladrón>().seMueve)
        {
            Vector2 puntoAleatorio = Random.insideUnitCircle * radioFalloAudicion;
            puntoInvestigacion = other.transform.position + new Vector3(puntoAleatorio.x, 0f, puntoAleatorio.y);
            escuchado.Invoke(puntoInvestigacion);
        }
    }
}

