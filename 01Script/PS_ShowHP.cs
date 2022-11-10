using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PS_ShowHP : MonoBehaviour
{
    private int hp;
    TextMeshProUGUI hp_txt;

    SlimeBaseSC sc;

    private void Awake()
    {
        hp_txt = GetComponent<TextMeshProUGUI>();
        sc = gameObject.GetComponentInParent<SlimeBaseSC>();
    }

    private void Update()
    {
        Debug.Log(sc.Health);
        hp_txt.text = "HP : " + hp.ToString();
        Debug.Log("hp : " + hp);
    }
}
