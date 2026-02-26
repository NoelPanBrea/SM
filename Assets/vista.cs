using UnityEngine;
using UnityEngine.Events;

public class vista : MonoBehaviour
{
    public UnityEvent visto;
    public Transform posicionladron;
    public float distanciaVision = 10f;
    public float anguloVision = 90;
    private Vector3 origen;
    private Vector3 direccion;

    void Start()
    {
        visto ??= new UnityEvent();
        visto.AddListener(Cerebro.VeAlLadron);
    }

    // Update is called once per frame
    void Update()
    {
        origen = transform.position + Vector3.up * 1.5f; // altura de los "ojos" del guardián
        direccion = posicionladron.position - origen;
        if (EnRango() && EnAngulo() && SinObstaculos())
        {
            visto.Invoke();
        }
    }

    bool EnRango()
    {   
        if (direccion.magnitude > distanciaVision)
            return false; 
        return true;
    }

    bool EnAngulo()
    {
        float angulo = Vector3.Angle(transform.forward, direccion.normalized);
        if (angulo > anguloVision / 2f)
            return false;
        return true;
    }

    bool SinObstaculos()
    {
        RaycastHit hit;
        if (Physics.Raycast(origen, direccion.normalized, out hit, direccion.magnitude))
        {
            if (hit.transform == posicionladron)
                return true;
        }
        return false;
    }
}
