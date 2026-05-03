using UnityEngine;

public class Puerta : MonoBehaviour
{
    public Vector3 openOffset = new Vector3(-7f, 0, 0); 
    public float speed = 2f;
    public float opendelay = 10f;
    private Vector3 closedPosition;
    private Vector3 targetPosition;
    private float t2;
    private bool open = false;

    void Start()
    {
        closedPosition = transform.position;
        targetPosition = closedPosition;
        t2 = Time.time;
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * speed);
        if (Time.time - t2 > opendelay)
        {
            t2 = Time.time;
            ToggleDoor();
        }
    }

    public void ToggleDoor()
    {
        open = !open;
        targetPosition = open ? closedPosition + openOffset : closedPosition;
    }
}