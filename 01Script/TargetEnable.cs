using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class TargetEnable : MonoBehaviour
{
	private ObserverBehaviour track;
	private int markerNum;
	public SlimeBaseSC slSC;

	private void Awake()
	{
		markerNum = -1;
		slSC = GetComponentInChildren<SlimeBaseSC>();
	}
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
		if (targetStatus.Status == Status.TRACKED || targetStatus.Status == Status.EXTENDED_TRACKED)
		{
			//Debug.Log(this.gameObject.name + "enabled");
			if (markerNum == -1)
			{
				if (GameManager.Instance.gameState != GAMESTATE.START && GameManager.Instance.gameState != GAMESTATE.READYFORSTART)
					markerNum = GameManager.Instance.SettingMarkerList(this.gameObject, slSC.state);
				else
				{this.gameObject.GetComponentInChildren<PS_ShowHP>().DestroyThis(); Destroy(this.gameObject); }
			}
			else
				GameManager.Instance.marker.markerExist[markerNum] = true;
		}
		else
		{
			if (markerNum != -1)
				GameManager.Instance.marker.markerExist[markerNum] = false;
		}

	}
}
