using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PS_System : MonoBehaviour
{
    public void GameStart()
    {
        // SceneManager.LoadScene("");
    }

    public void ShowInformation()
    {
        SceneManager.LoadScene("psh_GameInfoScene");
    }

    public void GoMain()
    {
        SceneManager.LoadScene("psh_MainScene");
    }
}
