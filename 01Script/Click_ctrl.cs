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
	//				//////

	private void Start()
	{
		// outline Material�� ����
		outline = new Material(Shader.Find("Mingyu/Outline"));
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
					Debug.Log(hit_object.name);
					//������
					if (Input.GetMouseButtonDown(0))
					{
						SetClickPoint(hit_object);

						//AddOutline(hit_object);
						//Debug.Log(hit_object.name); hit_object.SendMessage("Damage_Start", 1, SendMessageOptions.DontRequireReceiver);
						//SetDestination(hit_object);
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
		if (!sbSC.Clicked)	// Ŭ���� ���� ���� ����
		{
			if (ReferenceEquals(des.SetP1, null))	// ��� ���� ����
			{
				// ��� ���� �� �÷��̾��� ������ ���۵Ǵ� ������ ������ �÷��̾�
				if (sbSC.state == TEAM.PLAYER)
				{ des.SetP1 = obj; sbSC.Clicked = true; }
			}
			else // ��� ������ ������ ��. �� ���� ���� 
			{
				if (!ReferenceEquals(des.SetP2, null)) // SetP2�� ������ ���� �ִ� ��� ����
				{ des.SetP2.GetComponent<SlimeBaseSC>().Clicked = false; }
				des.SetP2 = obj; sbSC.Clicked = true;
				OrderAttack();
				
			}
		}
		else
		{
			if (ReferenceEquals(obj, des.SetP1))
			{ des.SetP1 = null; }
			else if (ReferenceEquals(obj, des.SetP2))
			{ des.SetP2 = null; }
			sbSC.Clicked = false;
		}
	}

	// ���� ����. ü���� 1 �����鼭 ����
	private void OrderAttack()
	{
		if (ReferenceEquals(null, GameObject.Find(des.SetP1.name + "_bridge_" + des.SetP2.name)))
		{
			GameObject dummy = GameObject.Instantiate(bridge);
			dummy.name = des.SetP1.name + "_bridge_" + des.SetP2.name;
			
			dummy.transform.parent = stdPoint.transform;
			dummy.GetComponent<SlimeBridge>().SetSD(des, TEAM.PLAYER);
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
