using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireCollision : MonoBehaviour {
	public AudioClip[] splashs;

	void Start () {
		GetComponent<AudioSource>().PlayOneShot(splashs[Random.Range(0, 3)]);
	}
}
