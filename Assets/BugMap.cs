using UnityEngine;
using UnityEngine.SceneManagement;

public class BugMap : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            SceneManager.LoadScene("Arayüz");
        }
    }

    
}
