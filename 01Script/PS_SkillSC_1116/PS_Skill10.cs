using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class PS_Skill10 : MonoBehaviour
{
	private ObserverBehaviour track;

	public int skill_code = 10; // 이 스킬의 코드

	private int stored_skill_index; // 버튼에 할당된 인덱스

	private void Start()
	{
		track = this.GetComponent<ObserverBehaviour>();
		if (track)
		{
			track.OnTargetStatusChanged += OnObserverStatusChanged;
			OnObserverStatusChanged(track, track.TargetStatus);
		}
	}

	void OnObserverStatusChanged(ObserverBehaviour behavior, TargetStatus targetStatus)
	{
		if (targetStatus.Status == Status.TRACKED)
			Skill1();
	}

	void Skill1()
	{
		// 스킬 저장 배열에 skillex1의 스킬 코드를 저장
		PS_SkillSystem s = GameObject.FindGameObjectWithTag("Skill").GetComponent<PS_SkillSystem>();
		for (int i = 0; i < 3; i++)
		{
			if (s.skill_storage[i] == 0)
			{
				s.skill_storage[i] = skill_code;
				stored_skill_index = i;
				break;
			}
			else if (s.skill_storage[i] != 0)
			{
				if (s.skill_storage[i] == skill_code)
					break;
			}
			else
				continue;
		}

		s.ReadSkillDB(stored_skill_index);
		Debug.Log(s.skill_type + "/" + s.skill_level);
		s.SkillNameDetermine(s.skill_type, s.skill_level, stored_skill_index);
		s.SaveSkillKey(stored_skill_index);
	}

}
