using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PS_SkillSystem : MonoBehaviour
{
    // Fucking Github ass
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

    private int currentClickType = 0;
    private int currentClickLevel = 0;

    public GameObject[] skillEffect_prefabs;
    //0-버프(생산), 1-디버프(생산), 2-버프(이동속도), 3-디버프(이동속도)

    //싱글톤 ///
    private static PS_SkillSystem instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

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

    public static PS_SkillSystem Instance
    {
        get {
            if (instance != null)
                return instance;
            return null;
        }
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

    // 스킬 저장 함수
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

    //---------------------------스킬 사용 함수
    

    public void BuffSkillClick(int type, int level) // 클릭 1번, 버프 디버프
    {
        currentClickLevel = level;
        currentClickType = type;

        if(type == 1 || type == 3)
        {
            for (int i = 0; i < GameManager.Instance.arrPlayer.Count; i++)
            {
                SlimeBaseSC dummy_base = GameManager.Instance.arrPlayer[i].GetComponentInChildren<SlimeBaseSC>();
                dummy_base.ChangeState(PLATESTATE.CANUSESKILL);
            }
        } else if (type == 2 || type == 4)
        {
            for (int i = 0; i < GameManager.Instance.arrEnemy.Count; i++)
            {
                SlimeBaseSC dummy_base = GameManager.Instance.arrEnemy[i].GetComponentInChildren<SlimeBaseSC>();
                dummy_base.ChangeState(PLATESTATE.CANUSESKILL);
            }
        }
    }


    public void SkillUse() // 클릭 2번 , 스킬 사용
    {
        Ray ray;
        RaycastHit hit;
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if(Physics.Raycast(ray, out hit))
        {
            SlimeBaseSC dummy_base = hit.transform.gameObject.GetComponentInChildren<SlimeBaseSC>();
            // SlimeBridge dummy_test = hit.transform.gameObject.Getcomponent
            
            if(currentClickType ==1)
            {
                switch (currentClickLevel)
                {
                    case 1:
                        StartCoroutine(ApplyingSkillProduce(dummy_base, 1.2f));
                        for (int i = 0; i < GameManager.Instance.arrPlayer.Count; i++)
                        {
                            SlimeBaseSC dummy_base1 = GameManager.Instance.arrPlayer[i].GetComponentInChildren<SlimeBaseSC>();
                            dummy_base1.ChangeState(PLATESTATE.UNCLICKED);
                        }
                        break;
                    case 2:
                        StartCoroutine(ApplyingSkillProduce(dummy_base, 1.0f));
                        for (int i = 0; i < GameManager.Instance.arrPlayer.Count; i++)
                        {
                            SlimeBaseSC dummy_base1 = GameManager.Instance.arrPlayer[i].GetComponentInChildren<SlimeBaseSC>();
                            dummy_base1.ChangeState(PLATESTATE.UNCLICKED);
                        }
                        break;
                    case 3:
                        StartCoroutine(ApplyingSkillProduce(dummy_base, 0.8f));
                        for (int i = 0; i < GameManager.Instance.arrPlayer.Count; i++)
                        {
                            SlimeBaseSC dummy_base1 = GameManager.Instance.arrPlayer[i].GetComponentInChildren<SlimeBaseSC>();
                            dummy_base1.ChangeState(PLATESTATE.UNCLICKED);
                        }
                        break;
                    case 4:
                        StartCoroutine(ApplyingSkillProduce(dummy_base, 0.6f));
                        for (int i = 0; i < GameManager.Instance.arrPlayer.Count; i++)
                        {
                            SlimeBaseSC dummy_base1 = GameManager.Instance.arrPlayer[i].GetComponentInChildren<SlimeBaseSC>();
                            dummy_base1.ChangeState(PLATESTATE.UNCLICKED);
                        }
                        break;

                } 
            } else if(currentClickType == 2)
            {
                switch (currentClickLevel)
                {
                    case 1:
                        StartCoroutine(ApplyingSkillProduce(dummy_base, 1.8f));
                        for (int i = 0; i < GameManager.Instance.arrEnemy.Count; i++)
                        {
                            SlimeBaseSC dummy_base1 = GameManager.Instance.arrEnemy[i].GetComponentInChildren<SlimeBaseSC>();
                            dummy_base1.ChangeState(PLATESTATE.UNCLICKED);
                        }
                        break;
                    case 2:
                        StartCoroutine(ApplyingSkillProduce(dummy_base, 2.1f));
                        for (int i = 0; i < GameManager.Instance.arrEnemy.Count; i++)
                        {
                            SlimeBaseSC dummy_base1 = GameManager.Instance.arrEnemy[i].GetComponentInChildren<SlimeBaseSC>();
                            dummy_base1.ChangeState(PLATESTATE.UNCLICKED);
                        }
                        break;
                    case 3:
                        StartCoroutine(ApplyingSkillProduce(dummy_base, 2.4f));
                        for (int i = 0; i < GameManager.Instance.arrEnemy.Count; i++)
                        {
                            SlimeBaseSC dummy_base1 = GameManager.Instance.arrEnemy[i].GetComponentInChildren<SlimeBaseSC>();
                            dummy_base1.ChangeState(PLATESTATE.UNCLICKED);
                        }
                        break;
                    case 4:
                        StartCoroutine(ApplyingSkillProduce(dummy_base, 3.0f));
                        for (int i = 0; i < GameManager.Instance.arrEnemy.Count; i++)
                        {
                            SlimeBaseSC dummy_base1 = GameManager.Instance.arrEnemy[i].GetComponentInChildren<SlimeBaseSC>();
                            dummy_base1.ChangeState(PLATESTATE.UNCLICKED);
                        }
                        break;
                }
            } else if(currentClickType == 3)
            {
                switch (currentClickLevel)
                {
                    case 1:

                        break;
                }
            }

            

        } // 스킬 사용

        switch (currentClickType)
        {
            case 1:// 버프(생산)
                GameObject dummy_effect1 = Instantiate(skillEffect_prefabs[0], hit.transform.position, hit.transform.rotation); 
                StartCoroutine(PassiveSkill(dummy_effect1)); // 이펙트 생성 후 5초 뒤 삭제
                break;
            case 2: // 디버프(생산
                GameObject dummy_effect2 = Instantiate(skillEffect_prefabs[1], hit.transform.position, hit.transform.rotation);
                StartCoroutine(PassiveSkill(dummy_effect2));
                break;
            case 3: // 버프 (이동 속도)
                GameObject dummy_effect3 = Instantiate(skillEffect_prefabs[2], hit.transform.position, hit.transform.rotation);
                StartCoroutine(PassiveSkill(dummy_effect3));
                break;
            case 4: // 디버프 (이동속도)
                GameObject dummy_effect4 = Instantiate(skillEffect_prefabs[3], hit.transform.position, hit.transform.rotation);
                StartCoroutine(PassiveSkill(dummy_effect4));
                break;
            case 5: // 기지공격 
                GameObject dummy_effect5 = Instantiate(skillEffect_prefabs[4], hit.transform.position, hit.transform.rotation);
                StartCoroutine(PassiveSkill(dummy_effect5));
                break;
            case 6: // 경로공격 
                GameObject dummy_effect6 = Instantiate(skillEffect_prefabs[5], hit.transform.position, hit.transform.rotation);
                StartCoroutine(PassiveSkill(dummy_effect6));
                break;
            case 7: // 다리잠금
                GameObject dummy_effect7 = Instantiate(skillEffect_prefabs[6], hit.transform.position, hit.transform.rotation);
                StartCoroutine(PassiveSkill(dummy_effect7));
                break;
        }// 스킬 타입 이펙트 생성


    }

    public void OnSkButton1()
    {
        BuffSkillClick(skill_type_storage[0], skill_level_storage[0]);
        skill_type_storage[0] = 0;
        skill_level_storage[0] = 0;
        skill_storage[0] = 0;
    }

    public void OnSkButton2()
    {
        BuffSkillClick(skill_type_storage[1], skill_level_storage[1]);
        skill_type_storage[1] = 0;
        skill_level_storage[1] = 0;
        skill_storage[1] = 0;
    }

    public void OnSkButton3()
    {
        BuffSkillClick(skill_type_storage[2], skill_level_storage[2]);
        skill_type_storage[2] = 0;
        skill_level_storage[2] = 0;
        skill_storage[2] = 0;
    }

    IEnumerator PassiveSkill(GameObject obj) // 이펙트
    {
        yield return new WaitForSecondsRealtime(10f);
        Destroy(obj.gameObject);
        StopCoroutine("PassiveSkill");
    }

    IEnumerator ApplyingSkillProduce(SlimeBaseSC sc, float val) // 효과
    {
        sc.rechargeDelay = val;
        yield return new WaitForSecondsRealtime(10f);
        sc.rechargeDelay = StructorCollector.BASERECHARGEDELAY;
        StopCoroutine("ApplyingSkillProduce");
    }

    IEnumerator ApplyingSkillSpeed(SlimeBaseSC sc, float val)
    {
        yield return new WaitForSecondsRealtime(10f);
    }
}
