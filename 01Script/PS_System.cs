using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PS_System : MonoBehaviour
{
    public Image fadeImg;

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

    public void FadeScene()
    {

    }

    IEnumerator FadeInCoroutine()
    {
        float fadeCount = 0;
        while (fadeCount <1.0f)
        {
            fadeCount += 0.01f;
            yield return new WaitForSecondsRealtime(0.01f);
            fadeImg.color = new Color(0, 0, 0, fadeCount);
        }
    }

    IEnumerator FadeOutCoroutine()
    {
        float fadeCount = 0;
    }
}
