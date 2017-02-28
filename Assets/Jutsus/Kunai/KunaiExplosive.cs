using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class KunaiExplosive : MonoBehaviour
{
	private GameObject goParent;
    public AudioClip soundKunaiStuck;
    public AudioClip soundKunaiClash;
    public AudioClip soundKunaiExplode;
    private Vector3 throwTo;
    public GameObject explosion;
    public UnityEngine.Object clashKunai;
	
    // Use this for initialization
    void Start()
    {
        explosion.SetActive(false);
		name = "KunaiExplosive";
        // transform.DetachChildren();

        clashKunai = Resources.Load("ClashKunaiPS");
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
        GetComponent<Rigidbody>().AddForce(throwTo == Vector3.zero ? GameObject.Find("Main Camera").transform.forward * 10000.0f :  Vector3.Normalize(throwTo - goParent.transform.position) * 1000.0f);
    }

    void OnCollisionEnter(Collision collision)
    {
		GetComponent<Rigidbody>().useGravity = true;

        var dir = collision.contacts[0].point - transform.position;
        if (collision.collider.gameObject.name == "Kunai_block") {
            var clashKunaiGo =  (GameObject) Instantiate(clashKunai);
            clashKunaiGo.transform.position = transform.position;

            collision.collider.enabled = false;
		    Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
            GetComponent<Rigidbody>().AddForce(-GetComponent<Rigidbody>().velocity * 30.0f);
            
            Debug.Log(collision.contacts[0].point);
		    GetComponent<AudioSource>().PlayOneShot(soundKunaiClash);
            return;
        }

        transform.LookAt(dir);
		transform.position -= dir * 0.2f; // Stuck it in
        transform.Rotate(new Vector3(0, 280, 205));
        GetComponent<Rigidbody>().velocity = Vector3.zero;
		GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
		GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Collider>().enabled = false;
        Invoke("Explode", 2.0f);

		GetComponent<AudioSource>().PlayOneShot(soundKunaiStuck);
    }

    public void Explode() {
		GetComponent<AudioSource>().PlayOneShot(soundKunaiExplode);
        explosion.SetActive(true);
        explosion.GetComponent<ParticleSystem>().Play();
    }
}
