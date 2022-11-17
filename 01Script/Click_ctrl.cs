using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class Click_ctrl : MonoBehaviour
{


	private RaycastHit hit;
	Material outline;
	List<Material> matList = new List<Material> ();

	
	public StructorCollector.DestinationSet des;
	public GameObject bridge;
	public GameObject stdPoint;
	
	// �̱��� ���� //////
	private static Click_ctrl instance;

	private void Awake()
	{
		if (instance == null) { instance = this; }
		else { Destroy(this.gameObject); }
	}
	// ��ó https://glikmakesworld.tistory.com/2
	public static Click_ctrl Instance {
		get { 
			if (null == instance) { return null; } 
			return instance; 
		}
	}
	////////

	private void Start()
	{
		// outline Material�� ����
		//outline = new Material(Shader.Find("Mingyu/Outline"));
		stdPoint = GameObject.FindGameObjectWithTag("StandardPoint");

		des.SetP1 = null; des.SetP2 = null;
	}

	private void Update()
	{
#if UNITY_EDITOR
		if (Input.GetMouseButton(0) || Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			Debug.DrawRay(ray.origin, ray.direction * 100, Color.green, 3, false);

			if (Physics.Raycast(ray, out hit))
			{
				if (hit.transform.CompareTag("SlimeBase"))
				{

					GameObject hit_object = hit.transform.gameObject;
					//������
					if (Input.GetMouseButtonDown(0))
					{
						if (GameManager.Instance.gameState == GAMESTATE.START)
						{
							SetClickPoint(hit_object);

							//AddOutline(hit_object);
							//Debug.Log(hit_object.name); hit_object.SendMessage("Damage_Start", 1, SendMessageOptions.DontRequireReceiver);
							//SetDestination(hit_object);
						}
					}
				}
				else
				{
					if (!ReferenceEquals(des.SetP1, null))
					{
						des.SetP1.GetComponent<SlimeBaseSC>().ChangeState(PLATESTATE.UNCLICKED);
						BridgeManager.Instance.ConnectedPanChange(des.SetP1.transform.parent.gameObject, PLATESTATE.UNCLICKED);
						des.SetP1 = null;
					}
					if (!ReferenceEquals(des.SetP2, null))
					{
						des.SetP2.GetComponent<SlimeBaseSC>().ChangeState(PLATESTATE.UNCLICKED);
						des.SetP2 = null;
					}
				}
			}
		}
#endif
	}

	private void SetClickPoint(GameObject obj)
	{
		SlimeBaseSC sbSC = obj.GetComponent<SlimeBaseSC>();

		// ������ Ŭ�� Ƚ���� Ȯ��.
		// �� �� ������Ʈ�� ���� Ȥ�� ����

		// Ŭ���� ���� ���� ����
		if (sbSC.clickState == PLATESTATE.UNCLICKED || sbSC.clickState == PLATESTATE.CANCLICK)  
		{
			if (ReferenceEquals(des.SetP1, null))   // ��� ���� ����
			{
				// ��� ���� �� �÷��̾��� ������ ���۵Ǵ� ������ ������ �÷��̾�
				if (sbSC.state == TEAM.PLAYER)
				{
					des.SetP1 = obj; sbSC.ChangeState(PLATESTATE.CLICKED);
					BridgeManager.Instance.ConnectedPanChange(des.SetP1.transform.parent.gameObject, PLATESTATE.CANCLICK, true);
				}
			}
			else // ��� ������ ������ ��. �� ���� ���� 
			{
				if (!ReferenceEquals(des.SetP2, null)) // SetP2�� ������ ���� �ִ� ��� ����
				{ des.SetP2.GetComponent<SlimeBaseSC>().ChangeState(PLATESTATE.UNCLICKED); }
				des.SetP2 = obj; sbSC.ChangeState(PLATESTATE.CLICKED);
				OrderAttack();
			}
		}
		// ����� ���� ����
		else if (sbSC.clickState == PLATESTATE.CANCLE)
		{
			GameObject dummy = GameManager.Instance.attackObjs.transform.Find(des.SetP1.name + "_attack_" + obj.name).gameObject;
			des.SetP1.GetComponent<SlimeBaseSC>().atkObj.Remove(dummy);
			dummy.GetComponent<SlimeBridge>().CancleAttack();
			BridgeManager.Instance.ConnectedPanChange(des.SetP1.transform.parent.gameObject, PLATESTATE.UNCLICKED);
			des.SetP1.GetComponent<SlimeBaseSC>().ChangeState(PLATESTATE.UNCLICKED);
			
			des.SetP1 = null;
		}
		else if (sbSC.clickState == PLATESTATE.CANUSESKILL)
        {
			PS_SkillSystem.Instance.SkillUse();
        }
		// Ȥ�ø� ���� ������
		else
		{
			if (ReferenceEquals(obj, des.SetP1))
			{
				BridgeManager.Instance.ConnectedPanChange(des.SetP1.transform.parent.gameObject, PLATESTATE.UNCLICKED);
				des.SetP1.GetComponent<SlimeBaseSC>().ChangeState(PLATESTATE.UNCLICKED);
				des.SetP1 = null;
				if (!ReferenceEquals(null, des.SetP2))
				{
					des.SetP2.GetComponent<SlimeBaseSC>().ChangeState(PLATESTATE.UNCLICKED);
					des.SetP2 = null;
				}

			}
			else if (ReferenceEquals(obj, des.SetP2))
			{ des.SetP2 = null; }
			sbSC.ChangeState(PLATESTATE.UNCLICKED);
		}
	}

	// ���� ����. ü���� 1 �����鼭 ����
	private void OrderAttack()
	{
		BridgeManager.Instance.ConnectedPanChange(des.SetP1.transform.parent.gameObject, PLATESTATE.UNCLICKED);
		if (BridgeManager.Instance.CheckBridge(des.SetP1.transform.parent.gameObject, des.SetP2.transform.parent.gameObject))
		{
			bool exist = false;
			SlimeBaseSC p1_sc = des.SetP1.GetComponent<SlimeBaseSC>();
			for (int i = 0; i < p1_sc.atkObj.Count; i++)
			{ if (p1_sc.atkObj[i].name.Equals(des.SetP1.name + "_attack_" + des.SetP2.name))
				{ exist = true; break; }
			}
			if (!exist)
			{
				GameObject dummy = GameObject.Instantiate(bridge);
				dummy.name = des.SetP1.name + "_attack_" + des.SetP2.name;

				dummy.transform.parent = GameManager.Instance.attackObjs.transform;
				dummy.GetComponent<SlimeBridge>().SetSD(des, TEAM.PLAYER);
				des.SetP1.GetComponent<SlimeBaseSC>().atkObj.Add(dummy);
				des.SetP1.GetComponent<SlimeBaseSC>().ChangeSoldierPower(dummy);
			}
		}
		if (!ReferenceEquals(des.SetP1, null))
		{
			des.SetP1.GetComponent<SlimeBaseSC>().ChangeState(PLATESTATE.UNCLICKED);
			BridgeManager.Instance.ConnectedPanChange(des.SetP1.transform.parent.gameObject, PLATESTATE.UNCLICKED);
			des.SetP1 = null;
		}
		if (!ReferenceEquals(des.SetP2, null))
		{
			des.SetP2.GetComponent<SlimeBaseSC>().ChangeState(PLATESTATE.UNCLICKED);
			des.SetP2 = null;
		}
	}

	//�Ͻ��� ���� -> ������ ����
	// ��ó https://bloodstrawberry.tistory.com/707
	private void AddOutline(GameObject obj)
	{
		MeshRenderer renderer = obj.GetComponentInChildren<MeshRenderer>();

		matList.Clear();
		matList.AddRange(renderer.materials);
		matList.Add(outline);

		renderer.materials = matList.ToArray();
	}

}
