using UnityEngine;
using UnityEngine.InputSystem;

public class Cerebro_ladrón : MonoBehaviour
{
    public float speed = 5f;

    public bool seMueve;

    void Start()
    {
        
    }

    void Update()
    {
        float x = 0f;
        float y = 0f;
        float z = 0f;

        if (Keyboard.current.aKey.isPressed) x = -1;
        if (Keyboard.current.dKey.isPressed) x = 1;
        if (Keyboard.current.wKey.isPressed) y = 1;
        if (Keyboard.current.sKey.isPressed) y = -1;

        if (Keyboard.current.jKey.isPressed) z = -1;
        if (Keyboard.current.lKey.isPressed) z = 1;

        Vector3 move = new Vector3(x, 0, y).normalized * speed * 1.41f * Time.deltaTime;
        transform.Translate(move);
        seMueve = move.magnitude > 0;
        Vector3 moveCamera = new Vector3(0, z, 0) * 100 * Time.deltaTime;
        transform.Rotate(moveCamera);


    }
}