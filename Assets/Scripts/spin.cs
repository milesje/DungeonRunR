using UnityEngine;

public class spin : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0f * Time.deltaTime, 30f * Time.deltaTime, 20f * Time.deltaTime, Space.Self);
    }
}
