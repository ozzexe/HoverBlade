using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("HoverBladeLevel"); // "pgame" sahnesine ge�
        Debug.Log("PLASDGSAH");
    }

    public void QuitGame()
    {
        Debug.Log("QuitGame called!"); // Debug log, oyunu kapatt���n� g�rmen i�in
        Application.Quit(); // Oyunu kapat (Build al�nd���nda �al���r)
    }
}
