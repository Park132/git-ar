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

    private void Start()
    {
        this_hp_target = GameObject.Instantiate(PrefabManager.Instance.imgHP);
        this_hp_target.transform.parent = GameManager.Instance.hpUIObjects.transform;
        this_hp_txt = this_hp_target.GetComponentInChildren<TextMeshProUGUI>();
    }
    private void Update()
    {
        if (GameManager.Instance.gameState == GAMESTATE.START || GameManager.Instance.gameState == GAMESTATE.SKILLTIME)
            this_hp_target.SetActive(true);
        else this_hp_target.SetActive(false);

        this_hp_target.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 0.8f, 0));
        this_hp_target.transform.position = new Vector3(this_hp_target.transform.position.x, this_hp_target.transform.position.y,0);
        hp = sc.Health;
        this_hp_txt.text = "HP : " + hp.ToString();
    }
    public void DestroyThis()
    {
        Destroy(this_hp_target);
    }
}
