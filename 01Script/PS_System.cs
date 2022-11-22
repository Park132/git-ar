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

    private void Start()
    {
        PS_System.Instance.FadeScene(true, 1.5f);
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
        PS_System.Instance.FadeScene(false, 1.5f);
        Time.timeScale = 1;
         SceneManager.LoadScene("psh_SampleScene_duplicated");
    }

    public void ShowInformation()
    {
        PS_System.Instance.FadeScene(false, 1.5f);
        SceneManager.LoadScene("psh_GameInfoScene");
    }

    public void GoMain()
    {
        PS_System.Instance.FadeScene(false, 1.5f);
        SceneManager.LoadScene("psh_MainScene");
    }

    public void FadeScene(bool key, float times) // key 1 -> ∆‰¿ÃµÂ ¿Œ / key 2 -> ∆‰¿ÃµÂ æ∆øÙ
    {
        if (key)
            StartCoroutine(FadeInCoroutine(times));
        else
            StartCoroutine(FadeOutCoroutine(times));
    }

    public IEnumerator FadeOutCoroutine(float times)
    {
        float fadeCount = 0;
        while (fadeCount <1.0f)
        {
            fadeCount += 0.01f*times;
            yield return new WaitForSecondsRealtime(0.01f);
            fadeImg.color = new Color(0, 0, 0, fadeCount);
        }
    }

    public IEnumerator FadeInCoroutine(float times)
    {
        float fadeCount = 1.0f;
        while (fadeCount > 0.0f)
        {
            fadeCount -= 0.01f * times;
            yield return new WaitForSecondsRealtime(0.01f);
            fadeImg.color = new Color(0, 0, 0, fadeCount);
        }
    }
}
