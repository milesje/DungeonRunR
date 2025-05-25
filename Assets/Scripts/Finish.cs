using UnityEditor.Build.Content;
using UnityEngine;

public class Finish : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MazeGenerator.Instance.EndGame();
        }
    }
}
