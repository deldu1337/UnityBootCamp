using UnityEngine;

public class TestCollision : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision!!!!");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("TriggerEnter!!!!");
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("OnTriggerExit!!!!");
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("OnTriggerStay!!!!");
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
