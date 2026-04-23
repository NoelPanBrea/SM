using UnityEngine;
using UnityEngine.Events;

public class Vista : MonoBehaviour
{
    public Transform posicionladron;
    public float distanciaVision = 20f;
    public float anguloVision = 210;
    private UnityEvent<Vector3> visto;
    private Cerebro cerebro;
    private Pajaro pajaro;
    private Vector3 origen;
    private Vector3 direccion;
    private Vector2 direccion2d;
    
    void Start()
    {
        visto ??= new UnityEvent<Vector3>();
        cerebro = GetComponent<Cerebro>();
        if (cerebro != null) visto.AddListener(cerebro.VeAlLadron);

        pajaro = GetComponent<Pajaro>();
        if (pajaro != null) visto.AddListener(pajaro.VeAlLadron);
    }

    // Update is called once per frame
    void Update()
    {
        origen = transform.position + Vector3.up * 1.5f;
        direccion = posicionladron.position - origen;
        direccion2d = new Vector2(direccion[0], direccion[2]);
        if (EnRango() && EnAngulo() && SinObstaculos())
        {
            visto.Invoke(posicionladron.position);

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
        Vector2 frente = new Vector2(transform.forward[0], transform.forward[2]);
        float angulo = Vector2.Angle(frente, direccion2d.normalized);
        if (angulo > anguloVision / 2f)
            return false;
        return true;
    }

    bool SinObstaculos()
    {
        RaycastHit hit;
        Debug.DrawRay(origen, direccion.normalized * direccion.magnitude, Color.red);
        if (Physics.Raycast(origen, direccion.normalized, out hit, direccion.magnitude))
        {
            if (hit.transform == posicionladron)
                return true;
        }
        return false;
    }
}
