using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LS_TimerSC : MonoBehaviour
{
    //ΩÃ±€≈Ê
    private static LS_TimerSC instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }
    public static LS_TimerSC Instance
    {
        get { return instance; }
    }
    //

    public float timer = 0;
    public TextMeshProUGUI timerText;
    public int minute = 0, second = 0;

    private void Start()
    {
        StartCoroutine(Timer());
    }

    private void Update()
    {
        if (GameManager.Instance.gameState == GAMESTATE.START)
            timer += Time.deltaTime;
    }
    IEnumerator Timer()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            minute = (int)(timer / 60);
            second = (int)(timer % 60);
            timerText.text = string.Format("Timer {0:D2} : {1:D2}", minute, second);
        }
    }
}
