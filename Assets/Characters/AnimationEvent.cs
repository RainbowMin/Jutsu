using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvent : MonoBehaviour {
	public AudioClip[] footsteps;
	public AudioClip wallJumped;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Footstep() {
		GetComponent<AudioSource>().PlayOneShot(footsteps[Random.Range(0, footsteps.Length - 1)], 0.2f);
	}

	public void WallJumped() {
		GetComponent<AudioSource>().PlayOneShot(wallJumped, 1f);
	}
}
