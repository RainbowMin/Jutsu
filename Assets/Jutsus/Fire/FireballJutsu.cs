using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballJutsu : MonoBehaviour {
    private UnityEngine.Object fireballPrefab;
	public int NbFireballToThrow = 30;
	public int NbFireballThrown = 0;
	public Vector2 DelayBetweenFireballs = new Vector2(0.05f, 0.1f);
	public Vector3 minOffsetFromThrower = new Vector3(1.0f, -1.5f, 1.0f);
	public Vector3 MaxOffsetFromPosition = new Vector3(0.5f, 1.5f, 0.5f);

	void Start () {
		fireballPrefab = Resources.Load("Fireball");

		LaunchFireball();
	}

	private void LaunchFireball()
	{
		var target = GameObject.Find("Main Camera").transform.forward * 10000.0f;
		var fireball = (GameObject) Instantiate(fireballPrefab);
		Physics.IgnoreCollision(GetComponent<Collider>(), fireball.GetComponent<Collider>());
		fireball.transform.position = GetComponent<BoxCollider>().transform.position + new Vector3(0.0f, GetComponent<BoxCollider>().size.y, 0.0f);
		var offsetX = Random.Range(0, 2) == 0 ? minOffsetFromThrower.x : -minOffsetFromThrower.x;
		var offsetZ = Random.Range(0, 2) == 0 ? minOffsetFromThrower.z : -minOffsetFromThrower.z;
		fireball.transform.position += new Vector3(offsetX, minOffsetFromThrower.y, offsetZ);
		fireball.transform.position += new Vector3(Random.Range(0, MaxOffsetFromPosition.x), Random.Range(0, MaxOffsetFromPosition.y), Random.Range(0, MaxOffsetFromPosition.z));
		fireball.GetComponent<Fireball>().Launch(target);

		++NbFireballThrown;
		if (NbFireballThrown >= NbFireballToThrow)
			Destroy(this);

		Invoke("LaunchFireball", Random.Range(DelayBetweenFireballs.x, DelayBetweenFireballs.y));
	}
}