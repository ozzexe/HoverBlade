using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("HoverBladeLevel"); // "pgame" sahnesine geç
        Debug.Log("PLASDGSAH");
    }

    public void QuitGame()
    {
        Debug.Log("QuitGame called!"); // Debug log, oyunu kapattýðýný görmen için
        Application.Quit(); // Oyunu kapat (Build alýndýðýnda çalýþýr)
    }
}
