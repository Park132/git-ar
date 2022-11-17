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
	
	// 싱글톤 지정 //////
	private static Click_ctrl instance;

	private void Awake()
	{
		if (instance == null) { instance = this; }
		else { Destroy(this.gameObject); }
	}
	// 출처 https://glikmakesworld.tistory.com/2
	public static Click_ctrl Instance {
		get { 
			if (null == instance) { return null; } 
			return instance; 
		}
	}
	////////

	private void Start()
	{
		// outline Material을 생성
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
					//디버깅용
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

		// 기지의 클릭 횟수를 확인.
		// 이 후 오브젝트를 저장 혹은 삭제

		// 클릭이 되지 않은 기지
		if (sbSC.clickState == PLATESTATE.UNCLICKED || sbSC.clickState == PLATESTATE.CANCLICK)  
		{
			if (ReferenceEquals(des.SetP1, null))   // 출발 지점 적용
			{
				// 출발 지점 즉 플레이어의 공격이 시작되는 지점은 무조건 플레이어
				if (sbSC.state == TEAM.PLAYER)
				{
					des.SetP1 = obj; sbSC.ChangeState(PLATESTATE.CLICKED);
					BridgeManager.Instance.ConnectedPanChange(des.SetP1.transform.parent.gameObject, PLATESTATE.CANCLICK, true);
				}
			}
			else // 출발 지점이 선택이 됨. 즉 공격 지점 
			{
				if (!ReferenceEquals(des.SetP2, null)) // SetP2에 지정한 적이 있는 경우 삭제
				{ des.SetP2.GetComponent<SlimeBaseSC>().ChangeState(PLATESTATE.UNCLICKED); }
				des.SetP2 = obj; sbSC.ChangeState(PLATESTATE.CLICKED);
				OrderAttack();
			}
		}
		// 취소할 기지 선택
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
		// 혹시모를 오류 방지용
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

	// 지점 공격. 체력을 1 깍으면서 공격
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

	//일시적 오류 -> 다음에 수정
	// 출처 https://bloodstrawberry.tistory.com/707
	private void AddOutline(GameObject obj)
	{
		MeshRenderer renderer = obj.GetComponentInChildren<MeshRenderer>();

		matList.Clear();
		matList.AddRange(renderer.materials);
		matList.Add(outline);

		renderer.materials = matList.ToArray();
	}

}
