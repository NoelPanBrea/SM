using UnityEngine;
using UnityEngine.Events;

public class Oido : MonoBehaviour
{
    public float radioFalloAudicion = 15f;
    private UnityEvent<Vector3> escuchado;
    private Cerebro cerebro;
    public Vector3 puntoInvestigacion;
    private bool escuchadovar;

    void Start()
    {   
        escuchado ??= new UnityEvent<Vector3>();
        escuchadovar = false;
        cerebro = GetComponent<Cerebro>();
        if (cerebro != null) escuchado.AddListener(cerebro.EscuchaAlLadron);
    }

    void Update()
    {
        escuchadovar = false;
    }

    public bool EscuchaAlLadron()
    {
        return escuchadovar;
    }

    void OnTriggerStay(Collider other)
    {

        if (other.CompareTag("Sonido") && other.GetComponent<Cerebro_ladrón>().seMueve)
        {
            Vector2 puntoAleatorio = Random.insideUnitCircle * radioFalloAudicion;
            puntoInvestigacion = other.transform.position + new Vector3(puntoAleatorio.x, 0f, puntoAleatorio.y);
            escuchado.Invoke(puntoInvestigacion);
            escuchadovar = true;
        }
    }

}

