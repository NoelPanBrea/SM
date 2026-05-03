using UnityEngine;
public class Estado : MonoBehaviour

{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    public virtual void Update()
    {
        
    }

    public virtual void Comportamiento() {}

    public virtual bool TomarControl() {return false;}
}