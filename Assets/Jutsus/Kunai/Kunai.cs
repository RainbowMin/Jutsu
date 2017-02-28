using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Kunai : MonoBehaviour
{
	private GameObject goParent;
    public AudioClip soundKunaiStuck;
    public AudioClip soundKunaiClash;
    private Vector3 throwTo;
	
    // Use this for initialization
    void Start()
    {
		name = "Kunai";
    }

    // Update is called once per frame
    void Update()
    {

    }

	public void ThrowFrom(GameObject goParent, Vector3 throwTo = default(Vector3))
	{
        this.throwTo = throwTo;
        transform.localPosition = Vector3.zero;
		Physics.IgnoreCollision(goParent.GetComponent<Collider>(), GetComponent<Collider>());
		this.goParent = goParent;

		var rightHandBone = goParent.GetComponentsInChildren<Transform>().Where(t => t.gameObject.name == "arm_right_finger_3b").First();
		transform.SetParent(rightHandBone, false);
		Invoke("ThrowKunai", 0.25f);
	}

    private void ThrowKunai()
    {
        transform.parent = null;
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        transform.LookAt(throwTo == Vector3.zero ? goParent.transform.forward * 50000.0f : throwTo);
        transform.Rotate(new Vector3(0.0f, 90.0f, 0.0f));
        GetComponent<Rigidbody>().AddForce(throwTo == Vector3.zero ? GameObject.Find("Main Camera").transform.forward * 1000.0f :  Vector3.Normalize(throwTo - goParent.transform.position) * 1000.0f);
    }

    void OnCollisionEnter(Collision collision)
    {
		GetComponent<Rigidbody>().useGravity = true;

        var dir = collision.contacts[0].point - transform.position;
        var meshFilter = collision.gameObject.GetComponent<MeshFilter>();
        if (meshFilter && meshFilter.sharedMesh.name == "Kunai_low") {
            GetComponent<Rigidbody>().AddExplosionForce(1400.0f, collision.contacts[0].point - dir * 10.0f, 50.0f);
		    GetComponent<AudioSource>().PlayOneShot(soundKunaiClash);
            return;
        }

		transform.position += dir * 0.5f; // Stuck it in
        GetComponent<Rigidbody>().velocity = Vector3.zero;
		GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
		GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Collider>().enabled = false;

		GetComponent<AudioSource>().PlayOneShot(soundKunaiStuck);
    }
}
