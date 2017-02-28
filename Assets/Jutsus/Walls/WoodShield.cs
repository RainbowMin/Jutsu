using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodShield : MonoBehaviour {
	public int NbBarrier = 5;
	public float SizeBarrier = 1.0f;
	public float DelayBetweenBarrier = 1.0f;

	// Use this for initialization
	void Start () {
		Launch(transform);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void Launch(Transform initial) {
		for (int i = 0; i < NbBarrier; ++i) {
			
		}
	}
}
