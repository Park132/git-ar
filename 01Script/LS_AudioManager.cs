using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class LS_AudioManager : MonoBehaviour
{
	/// //
    private static LS_AudioManager instance;
	private void Awake()
	{
		if (instance == null) { instance = this; DontDestroyOnLoad(this.gameObject); }
		else Destroy(this.gameObject);
		
	}
	public static LS_AudioManager Instance
	{get { return instance; }}
	///
	
	public GameObject bgmObj;
    public GameObject[] effects;
    public AudioMixer mixer;
	public AudioSource source;
	public AudioClip[] bgms;

	private void Start()
	{
		source = this.GetComponent<AudioSource>();
		LS_AudioManager.Instance.BGM_Setting(0);
	}

	public void BGM_Setting(int index)
	{
		source.clip = bgms[index];
		source.Play();
	}

	public IEnumerator BGM_FadeIn(float times)
	{
		float i = 0;
		while(i < 100){
			mixer.SetFloat("Background", -40+((40f/100)*i));
			i += times;
			yield return new WaitForSeconds(0.01f);
		}
	}

	public IEnumerator BGM_FadeOut(float times)
	{
		float i = 0;
		while (i < 100)
		{
			mixer.SetFloat("Background", ((-40f / 100) * i));
			i += times;
			yield return new WaitForSeconds(0.01f);
		}
		mixer.SetFloat("Background", 0);
	}

	public void SFX_spawn(int index,Vector3 position)
	{ StartCoroutine(SFX_instanciate(index, position)); }
	private IEnumerator SFX_instanciate(int index, Vector3 position)
	{
		GameObject dummy = GameObject.Instantiate(effects[index]);
		dummy.transform.position = position;
		yield return new WaitForSeconds(0.5f);
		Destroy(dummy);
	}


}
