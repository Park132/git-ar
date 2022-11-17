using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PS_System : MonoBehaviour
{
    public Image fadeImg;

    /*
    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
            FadeScene(1);
        if(Input.GetMouseButtonDown(1))
            FadeScene(2);
    }
    */

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

    public void FadeScene(int key) // key 1 -> 페이드 인 / key 2 -> 페이드 아웃
    {
        switch (key)
        {
            case 1:
                StartCoroutine(FadeInCoroutine());
                break;
            case 2:
                StartCoroutine(FadeOutCoroutine());
                break;
        }
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
        float fadeCount = 1.0f;
        while (fadeCount > 0.0f)
        {
            fadeCount -= 0.01f;
            yield return new WaitForSecondsRealtime(0.01f);
            fadeImg.color = new Color(0, 0, 0, fadeCount);
        }
    }
}
