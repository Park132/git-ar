using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PS_SkillSystem : MonoBehaviour
{
    // Fucking Github ass
    public int[] skill_storage; // �ν��� ��ų ���� -> ��ųŸ�԰� �ܰ�, ��ư ǥ�ÿ� �̿�
    public int[] skill_type_storage;
    public int[] skill_level_storage;

    private int[] skill_database; // ��� ��ų ������ ������ �迭

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
    //0-����(����), 1-�����(����), 2-����(�̵��ӵ�), 3-�����(�̵��ӵ�)

    //�̱��� ///
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
            1, 2, 3, 4, // ����(���� / skill_type = 1
            5, 6, 7, 8,  // ����� (���� / skill_type = 2
            9, 10, 11, 12,  // ���� (�̵��ӵ� / skill_type = 3
            13, 14, 15, 16,  // ����� (�̵��ӵ� / skill_type = 4
            17, 18, // �������� / skill_type = 5
            19, 20, // ��ΰ��� / skill_type = 6
            21, 22, 23, 24 // �ٸ����
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

    // ��ų ���� �Լ�
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

                if(skill_storage[index] >= 17 && skill_storage[index] <= 20)
                {
                    skill_type = 5;
                    skill_level = skill_storage[index] % 4;
                    if (skill_level % 4 == 0)
                        skill_level = 4;
                    break;
                }
                /*
                if(skill_storage[index] >= 19 && skill_storage[index] <= 20) {
                    skill_type = 6;
                    skill_level = skill_storage[index] % 4;
                    if (skill_level % 4 == 0)
                        skill_level = 2;
                    break;
                }
                */
                if(skill_storage[index] >= 21 && skill_storage[index] <= 24)
                {
                    skill_type = 7;
                    skill_level = skill_storage[index] % 4;
                    if (skill_level % 4 == 0)
                        skill_level = 2;
                    break;
                }
            }
        }
    } // ��ų Ÿ�԰� ������ skilltype, skilllevel�� ����

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
                case 5:
                    btn1_txt.text = "Attack (Base)";
                    break;
                case 6:
                    btn1_txt.text = "Attack (Bridge)";
                    break;
                case 7:
                    btn1_txt.text = "Lock (Base)";
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
                case 5:
                    btn2_txt.text = "Attack (Base)";
                    break;
                case 6:
                    btn2_txt.text = "Attack (Bridge)";
                    break;
                case 7:
                    btn2_txt.text = "Lock (Base)";
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
                case 5:
                    btn3_txt.text = "Attack (Base)";
                    break;
                case 6:
                    btn3_txt.text = "Attack (Base)";
                    break;
                case 7:
                    btn3_txt.text = "Lock (Base)";
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
    } // ��ų �̸��� �ܰ踦 ��ư�� ǥ��

    public void SaveSkillKey(int index)
    {
        skill_type_storage[index] = skill_type;
        skill_level_storage[index] = skill_level;
        Debug.Log("skill_type_storage : " + skill_type_storage[index]);
        Debug.Log("skill_level_storage : " + skill_level_storage[index]);
    }

    //---------------------------��ų ��� �Լ�
    

    public void BuffSkillClick(int type, int level) // Ŭ�� 1��, ���� �����
    {
        currentClickLevel = level;
        currentClickType = type;

        // ��ų Ÿ�Կ� ���� ���̽� ����
        if(type == 1 || type == 3)
        {
            for (int i = 0; i < GameManager.Instance.arrPlayer.Count; i++)
            {
                SlimeBaseSC dummy_base = GameManager.Instance.arrPlayer[i].GetComponentInChildren<SlimeBaseSC>();
                dummy_base.ChangeState(PLATESTATE.CANUSESKILL);
            }
        } else if (type == 2 || type == 4 || type == 5 || type == 6 || type == 7)
        {
            for (int i = 0; i < GameManager.Instance.arrEnemy.Count; i++)
            {
                SlimeBaseSC dummy_base = GameManager.Instance.arrEnemy[i].GetComponentInChildren<SlimeBaseSC>();
                dummy_base.ChangeState(PLATESTATE.CANUSESKILL);
            }
        }
    }


    public void SkillUse() // Ŭ�� 2�� , ��ų ���
    {
        Ray ray;
        RaycastHit hit;
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if(Physics.Raycast(ray, out hit))
        {
            SlimeBaseSC dummy_base = hit.transform.gameObject.GetComponentInChildren<SlimeBaseSC>();
            
            if(currentClickType ==1) // ���� ����
            {
                switch (currentClickLevel)
                {
                    case 1:
                        StartCoroutine(ApplyingSkillProduce(dummy_base, 1.2f));
                        break;
                    case 2:
                        StartCoroutine(ApplyingSkillProduce(dummy_base, 1.0f));
                        break;
                    case 3:
                        StartCoroutine(ApplyingSkillProduce(dummy_base, 0.8f));
                        break;
                    case 4:
                        StartCoroutine(ApplyingSkillProduce(dummy_base, 0.6f));
                        break;
                }
                PlayerBaseColorInit();
            } else if(currentClickType == 2) // ���� �����
            {
                switch (currentClickLevel)
                {
                    case 1:
                        StartCoroutine(ApplyingSkillProduce(dummy_base, 1.8f));
                        break;
                    case 2:
                        StartCoroutine(ApplyingSkillProduce(dummy_base, 2.1f));
                        break;
                    case 3:
                        StartCoroutine(ApplyingSkillProduce(dummy_base, 2.4f));
                        break;
                    case 4:
                        StartCoroutine(ApplyingSkillProduce(dummy_base, 3.0f));
                        break;
                }
                EnemyBaseColorInit();
            } else if(currentClickType == 3) // ���� ������ �ð� �پ��
            {
                switch (currentClickLevel)
                {
                    case 1:
                        StartCoroutine(ApplyingSkillDelaySpeed(dummy_base, 0.8f));
                        break;
                    case 2:
                        StartCoroutine(ApplyingSkillDelaySpeed(dummy_base, 0.6f));
                        break;
                    case 3:
                        StartCoroutine(ApplyingSkillDelaySpeed(dummy_base, 0.4f));
                        break;
                    case 4:
                        StartCoroutine(ApplyingSkillDelaySpeed(dummy_base, 0.2f));
                        break;
                }
                PlayerBaseColorInit();
            } else if(currentClickType == 4) // ���� ������ �ð� �þ
            {
                switch (currentClickLevel)
                {
                    case 1:
                        StartCoroutine(ApplyingSkillDelaySpeed(dummy_base, 1.2f));
                        break;
                    case 2:
                        StartCoroutine(ApplyingSkillDelaySpeed(dummy_base, 1.4f));
                        break;
                    case 3:
                        StartCoroutine(ApplyingSkillDelaySpeed(dummy_base, 1.7f));
                        break;
                    case 4:
                        StartCoroutine(ApplyingSkillDelaySpeed(dummy_base, 2.0f));
                        break;
                }
                EnemyBaseColorInit();
            } else if(currentClickType == 5) // ���� ����
            {
                switch (currentClickLevel)
                {
                    case 1:
                        dummy_base.Health -= 20;
                        break;
                    case 2:
                        dummy_base.Health -= 30;
                        break;
                    case 3:
                        dummy_base.Health -= 35;
                        break;
                    case 4:
                        dummy_base.Health -= 40;
                        break;
                }
                EnemyBaseColorInit();
            } else if(currentClickType == 6)
            {
                switch(currentClickLevel) // ��ΰ���
                {
                    
                }
                EnemyBaseColorInit();
            } else if(currentClickType == 7) // ���� ���
            {
                switch(currentClickLevel)
                {
                    case 1:
                        StartCoroutine(ApplyingSkillLockBase(dummy_base, 5f));
                        break;
                    case 2:
                        StartCoroutine(ApplyingSkillLockBase(dummy_base, 7f));
                        break;
                    case 3:
                        StartCoroutine(ApplyingSkillLockBase(dummy_base, 10f));
                        break;
                    case 4:
                        StartCoroutine(ApplyingSkillLockBase(dummy_base, 15f));
                        break;
                }
                EnemyBaseColorInit();
            }

            

        } // ��ų ���

        switch (currentClickType)
        {
            case 1:// ����(����)
                GameObject dummy_effect1 = Instantiate(skillEffect_prefabs[0], hit.transform.position, hit.transform.rotation); 
                StartCoroutine(PassiveSkill(dummy_effect1)); // ����Ʈ ���� �� 5�� �� ����
                break;
            case 2: // �����(����
                GameObject dummy_effect2 = Instantiate(skillEffect_prefabs[1], hit.transform.position, hit.transform.rotation);
                StartCoroutine(PassiveSkill(dummy_effect2));
                break;
            case 3: // ���� (�̵� �ӵ�)
                GameObject dummy_effect3 = Instantiate(skillEffect_prefabs[2], hit.transform.position, hit.transform.rotation);
                StartCoroutine(PassiveSkill(dummy_effect3));
                break;
            case 4: // ����� (�̵��ӵ�)
                GameObject dummy_effect4 = Instantiate(skillEffect_prefabs[3], hit.transform.position, hit.transform.rotation);
                StartCoroutine(PassiveSkill(dummy_effect4));
                break;
            case 5: // �������� 
                GameObject dummy_effect5 = Instantiate(skillEffect_prefabs[4], hit.transform.position, hit.transform.rotation);
                StartCoroutine(PassiveSkill(dummy_effect5));
                break;
            case 6: // ��ΰ��� 
                GameObject dummy_effect6 = Instantiate(skillEffect_prefabs[5], hit.transform.position, hit.transform.rotation);
                StartCoroutine(PassiveSkill(dummy_effect6));
                break;
            case 7: // �ٸ����
                GameObject dummy_effect7 = Instantiate(skillEffect_prefabs[6], hit.transform.position, hit.transform.rotation);
                StartCoroutine(PassiveSkill(dummy_effect7));
                break;
        }// ��ų Ÿ�� ����Ʈ ����


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

    void EnemyBaseColorInit() // ��ų ���� ���̽� ���� �ٲٱ�
    {
        for (int i = 0; i < GameManager.Instance.arrEnemy.Count; i++)
        {
            SlimeBaseSC dummy_base1 = GameManager.Instance.arrEnemy[i].GetComponentInChildren<SlimeBaseSC>();
            dummy_base1.ChangeState(PLATESTATE.UNCLICKED);
        }
    }

    void PlayerBaseColorInit() // ��ų ���� ���̽� ���� �ٲٱ�
    {
        for (int i = 0; i < GameManager.Instance.arrPlayer.Count; i++)
        {
            SlimeBaseSC dummy_base1 = GameManager.Instance.arrPlayer[i].GetComponentInChildren<SlimeBaseSC>();
            dummy_base1.ChangeState(PLATESTATE.UNCLICKED);
        }
    }

    IEnumerator PassiveSkill(GameObject obj) // ����Ʈ
    {
        yield return new WaitForSecondsRealtime(10f);
        Destroy(obj.gameObject);
        StopCoroutine("PassiveSkill");
    }

    IEnumerator ApplyingSkillProduce(SlimeBaseSC sc, float val) // ���� ȿ��
    {
        sc.rechargeDelay = val;
        yield return new WaitForSecondsRealtime(10f);
        sc.rechargeDelay = StructorCollector.BASERECHARGEDELAY;
        StopCoroutine("ApplyingSkillProduce");
    }

    IEnumerator ApplyingSkillDelaySpeed(SlimeBaseSC sc, float val) // ������ ȿ��
    {
        sc.settingSkillSAD(1, 1, val);
        yield return new WaitForSecondsRealtime(10f);
        sc.settingSkillSAD(1, 1, 1);
        StopCoroutine("ApplyingSkillDelaySpeed");
    }

    IEnumerator ApplyingSkillLockBase(SlimeBaseSC sc, float val)
    {
        sc.settingSkillSAD(1, 1, 999);
        yield return new WaitForSecondsRealtime(val);
        sc.settingSkillSAD(1, 1, 1);
        StopCoroutine("ApplyingSkillLockBase");
    }
}