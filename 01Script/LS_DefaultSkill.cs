using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LS_DefaultSkill : MonoBehaviour
{
    public TextMeshProUGUI tmp;
    int healing;

    public void ActiveSkillObj()
    {
        healing = Mathf.RoundToInt((LS_TimerSC.Instance.timer / 10) * 2 + 3);
        tmp.text = "+ " + healing;
    }

	public void ClickDefaultSkill()
    {
        if (GameManager.Instance.gameState == GAMESTATE.SKILLTIME)
        {
            healing = Mathf.RoundToInt((LS_TimerSC.Instance.timer / 10) * 2 + 3);
            for (int i = 0; i < GameManager.Instance.arrPlayer.Count; i++)
            {
                SlimeBaseSC dummy_slsc = GameManager.Instance.arrPlayer[i].GetComponentInChildren<SlimeBaseSC>();
                dummy_slsc.Health += healing;
            }
            GameManager.Instance.SkillSelected();
        }
        this.gameObject.SetActive(false);
    }
}
