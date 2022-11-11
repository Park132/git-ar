using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PS_ShowHP : MonoBehaviour
{
    private int hp;
    public TextMeshProUGUI hp_txt;

    public GameObject hp_target;

    SlimeBaseSC sc;

    private void Awake()
    {
        //hp_txt = empty1.gameObject.GetComponent<TextMeshProUGUI>();
        sc = gameObject.GetComponentInParent<SlimeBaseSC>();
    }

    private void Update()
    {
        hp_target.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 0.8f, 0));
        hp = sc.Health;
        //Debug.Log("hp : " + hp);
        hp_txt.text = "HP : " + hp.ToString();
    }
}
