using UnityEngine;
using UnityEngine.Events;

public class Oido : MonoBehaviour
{
    public UnityEvent escuchado;
    private Cerebro cerebro;
    private float espera_audicion = 0f;

    void Start()
    {
        escuchado ??= new UnityEvent();
        cerebro = GetComponent<Cerebro>();
        if (cerebro != null) escuchado.AddListener(cerebro.EscuchaAlLadron);
    }

    void Update()
    {
        
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Sonido") && Time.time > espera_audicion )
        {
            espera_audicion = Time.time + 1f;
            escuchado.Invoke();
        }
    }
}

