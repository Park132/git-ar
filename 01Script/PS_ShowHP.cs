using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PS_ShowHP : MonoBehaviour
{
    private int hp;
    public TextMeshProUGUI this_hp_txt;

    public GameObject this_hp_target;

    SlimeBaseSC sc;

    private void Awake()
    {
        sc = gameObject.GetComponentInParent<SlimeBaseSC>();
    }

    private void Update()
    {
        this_hp_target.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 0.8f, 0));
        hp = sc.Health;
        this_hp_txt.text = "HP : " + hp.ToString();
    }
}
