using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PS_SkillSystem : MonoBehaviour
{
    public int[] skill_storage; // 인식한 스킬 정보 -> 스킬타입과 단계, 버튼 표시에 이용
    public int[] skill_type_storage;
    public int[] skill_level_storage;

    private int[] skill_database; // 모든 스킬 정보를 저장할 배열

    public int key = 0;

    public GameObject s_btn1;
    public GameObject s_btn2;
    public GameObject s_btn3;

    public TextMeshProUGUI btn1_txt;
    public TextMeshProUGUI btn2_txt;
    public TextMeshProUGUI btn3_txt;

    public int skill_type = 0;
    public int skill_level = 0;

    private void Awake()
    {
        skill_storage = new int[] { 0, 0, 0 };
        skill_type_storage = new int[] { 0, 0, 0 };
        skill_level_storage = new int[] { 0, 0, 0 };

        skill_database = new int[]
        {
            1, 2, 3, 4, // 버프(생산 / skill_type = 1
            5, 6, 7, 8,  // 디버프 (생산 / skill_type = 2
            9, 10, 11, 12,  // 버프 (이동속도 / skill_type = 3
            13, 14, 15, 16,  // 디버프 (이동속도 / skill_type = 4
            17, 18, // 기지공격 / skill_type = 5
            19, 20, // 경로공격 / skill_type = 6
            21, 22, 23, 24 // 다리잠금
        };
    }

    void Update()
    {
        if (skill_storage[0] != 0)
            s_btn1.SetActive(true);
        else
            s_btn1.SetActive(false);

        if (skill_storage[1] != 0)
            s_btn2.SetActive(true);
        else
            s_btn2.SetActive(false);

        if (skill_storage[2] != 0)
            s_btn3.SetActive(true);
        else
            s_btn3.SetActive(false);
    }

    public void ReadSkillDB(int index)
    {
        for(int i = 0; i<24; i++)
        {
            if(skill_database[i] == skill_storage[index])
            {
                if(skill_storage[index] >= 1 && skill_storage[index] <= 4)
                {
                    skill_type = 1;
                    skill_level = skill_storage[index] % 4;
                    if (skill_level % 4 == 0)
                        skill_level = 4;
                    break;
                }

                if (skill_storage[index] >= 5 && skill_storage[index] <= 8)
                {
                    skill_type = 2;
                    skill_level = skill_storage[index] % 4;
                    if (skill_level % 4 == 0)
                        skill_level = 4;
                    break;
                }

                if (skill_storage[index] >= 9 && skill_storage[index] <= 12)
                {
                    skill_type = 3;
                    skill_level = skill_storage[index] % 4;
                    if (skill_level % 4 == 0)
                        skill_level = 4;
                    break;
                }

                if (skill_storage[index] >= 13 && skill_storage[index] <= 16)
                {
                    skill_type = 4;
                    skill_level = skill_storage[index] % 4;
                    if (skill_level % 4 == 0)
                        skill_level = 4;
                    break;
                }
            }
        }
    } // 스킬 타입과 레벨을 skilltype, skilllevel에 저장

    public void SkillNameDetermine(int key1, int key2, int button)
    {
        if(button == 0)
        {
            switch (key1) {
                case 1:
                    btn1_txt.text = "Buff (Produce)";
                    break;
                case 2:
                    btn1_txt.text = "Debuff (Produce)";
                    break;
                case 3:
                    btn1_txt.text = "Buff (Speed)";
                    break;
                case 4:
                    btn1_txt.text = "Debuff (Speed)";
                    break;
            }

            switch(key2)
            {
                case 1:
                    btn1_txt.text += " Level 1";
                    break;
                case 2:
                    btn1_txt.text += " Level 2";
                    break;
                case 3:
                    btn1_txt.text += " Level 3";
                    break;
                case 4:
                    btn1_txt.text += " Level 4";
                    break;
            }
        } else if(button == 1)
        {
            switch (key1)
            {
                case 1:
                    btn2_txt.text = "Buff (Produce)";
                    break;
                case 2:
                    btn2_txt.text = "Debuff (Produce)";
                    break;
                case 3:
                    btn2_txt.text = "Buff (Speed)";
                    break;
                case 4:
                    btn2_txt.text = "Debuff (Speed)";
                    break;
            }
            switch (key2)
            {
                case 1:
                    btn2_txt.text += " Level 1";
                    break;
                case 2:
                    btn2_txt.text += " Level 2";
                    break;
                case 3:
                    btn2_txt.text += " Level 3";
                    break;
                case 4:
                    btn2_txt.text += " Level 4";
                    break;
            }


        } else
        {
            switch (key1)
            {
                case 1:
                    btn3_txt.text = "Buff (Produce)";
                    break;
                case 2:
                    btn3_txt.text = "Debuff (Produce)";
                    break;
                case 3:
                    btn3_txt.text = "Buff (Speed)";
                    break;
                case 4:
                    btn3_txt.text = "Debuff (Speed)";
                    break;
            }

            switch (key2)
            {
                case 1:
                    btn3_txt.text += " Level 1";
                    break;
                case 2:
                    btn3_txt.text += " Level 2";
                    break;
                case 3:
                    btn3_txt.text += " Level 3";
                    break;
                case 4:
                    btn3_txt.text += " Level 4";
                    break;
            }
        }
    } // 스킬 이름과 단계를 버튼에 표시

    public void SaveSkillKey(int index)
    {
        skill_type_storage[index] = skill_type;
        skill_level_storage[index] = skill_level;
        Debug.Log("skill_type_storage : " + skill_type_storage[index]);
        Debug.Log("skill_level_storage : " + skill_level_storage[index]);
    }
    
    public void OnSkButton1()
    {
        
    }

    public void OnSkButton2()
    {

    }

    public void OnSkButton3()
    {

    }


}
