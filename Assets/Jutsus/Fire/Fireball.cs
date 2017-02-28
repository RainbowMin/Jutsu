using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour {
	public static UnityEngine.Object prefabFirecollision = null;
	public float Velocity = 1000.0f;
	public Vector3 Target;

	void Start () {
		if (prefabFirecollision == null)
			prefabFirecollision = Resources.Load("FireCollision");
	}

	public void Launch(Vector3 target) {
		Target = target;
		transform.LookAt(Target);

		enabled = true;
		GetComponent<Rigidbody>().AddForce(transform.forward * Velocity, ForceMode.Acceleration);

		Destroy(gameObject, 5.0f);
	}
	
	void Update () {
		
	}

	void OnCollisionEnter(Collision collision) {
		GetComponent<ParticleSystem>().Stop();
		GetComponent<Rigidbody>().velocity *= 0.1f;

		Vector3 direction = collision.transform.position - transform.position;
		direction = direction.normalized;

		var firesplash = (GameObject) Instantiate(prefabFirecollision);
		firesplash.transform.rotation = transform.rotation;
		firesplash.transform.position = transform.position - (firesplash.transform.forward * 0.05f);
		Destroy(firesplash, 3.0f);

		// Use BoxCollider instead of the particle system colliders
		GetComponent<ParticleSystem>().Stop();
		GetComponent<BoxCollider>().enabled = false;
		GetComponent<Rigidbody>().AddForce(transform.forward * 100.0f, ForceMode.Acceleration);
		GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

		Destroy(gameObject);
    }
}
