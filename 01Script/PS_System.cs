using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PS_System : MonoBehaviour
{
    public Image fadeImg;

    // ΩÃ±€≈Ê ///
    private static PS_System instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    public static PS_System Instance
    {
        get
        {
            if (instance != null)
                return instance;
            return null;
        }
    }
    ///////////////
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

    public void FadeScene(bool key) // key 1 -> ∆‰¿ÃµÂ ¿Œ / key 2 -> ∆‰¿ÃµÂ æ∆øÙ
    {
        if (key)
            StartCoroutine(FadeInCoroutine());
        else
            StartCoroutine(FadeOutCoroutine());
    }

    public IEnumerator FadeOutCoroutine()
    {
        float fadeCount = 0;
        while (fadeCount <1.0f)
        {
            fadeCount += 0.01f;
            yield return new WaitForSecondsRealtime(0.01f);
            fadeImg.color = new Color(0, 0, 0, fadeCount);
        }
    }

    public IEnumerator FadeInCoroutine()
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
