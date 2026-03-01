using UnityEngine;
using UnityEngine.Events;

#pragma warning disable CA1050 // Declare types in namespaces
public class Tacto : MonoBehaviour
{
    public float distanciaToque = 1f;
    private Cerebro cerebro;
    private UnityEvent tocado;

    void Start()
    {
        tocado ??= new UnityEvent();
        cerebro = GetComponent<Cerebro>();
        if (cerebro != null) tocado.AddListener(cerebro.TocaAlLadron);
    }

    void Update()
    {
        
    }

    void OnTriggerStay(Collider other)
    {
        Vector3 direccion = other.transform.position - transform.position;
        float distancia = direccion.magnitude;
        if (other.CompareTag("Sonido") && distancia < distanciaToque)
        {   
            tocado.Invoke();
        }
    }
}

