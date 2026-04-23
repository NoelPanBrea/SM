using UnityEngine;

public class Puerta : MonoBehaviour
{
    public Vector3 openOffset = new Vector3(-7f, 0, 0); 
    public float speed = 2f;
    public float opendelay;
    private Vector3 closedPosition;
    private Vector3 targetPosition;
    private bool open = false;

    void Start()
    {
        closedPosition = transform.position;
        targetPosition = closedPosition;
        opendelay = Time.time;
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * speed);
        if (Time.time - opendelay > 10f)
        {
            opendelay = Time.time;
            ToggleDoor();
        }
    }

    public void ToggleDoor()
    {
        open = !open;
        targetPosition = open ? closedPosition + openOffset : closedPosition;
    }
}