using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlamethrowerJutsu : MonoBehaviour {
	private UnityEngine.Object flamethrowerPrefab;
	public Vector3 minOffsetFromThrower = new Vector3(1.0f, -1.5f, 1.0f);
	public Vector3 MaxOffsetFromPosition = new Vector3(0.5f, 1.5f, 0.5f);

	void Start () {
		flamethrowerPrefab = Resources.Load("FlameThrowerEffect");

		LaunchFlamethrower();
	}

	private void LaunchFlamethrower()
	{
		var flamethrower = (GameObject) Instantiate(flamethrowerPrefab, GameObject.Find("YamatoContainer/yamato/Armature/root_ground/pelvis/spine_lower/spine_upper/head_neck_lower/head_neck_upper/head_jaw/head_lip_lower_middle").transform);
		StartCoroutine(AudioFadeOut.FadeOut(flamethrower.GetComponent<AudioSource>(), 6.5f));
		flamethrower.GetComponent<AudioSource>().time = 0.5f;
		Destroy(flamethrower, 12.0f);
	}
}
