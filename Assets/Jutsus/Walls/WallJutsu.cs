using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct Wall
{
	public GameObject go;
	public float speed;
	public float decal_height;
}

public class WallJutsu : MonoBehaviour {
	private Vector2 SpeedRange = new Vector2(0.01f, 0.015f);
	private Vector2 DecalHeightRange = new Vector2(0.0f, 1.0f);
	private Vector2 DecalYRange = new Vector2(-0.05f, 0.05f);
	private Transform InitialTransform;
	public int NbBarrier = 5;
	private Vector3 SizeBarrier = new Vector3(0.4f, 1.8f, 1.0f);
	public float DelayBetweenBarrier = 1.0f;
	private UnityEngine.Object wallPrefab;
	private float GoToHeight = -1.0f;
	private List<Wall> walls = new List<Wall>();
	private GameObject wallsContainer;
	public AudioClip[] stoneGrinding;

	public void Start() {
	}

	public void Launch() {
		wallsContainer = new GameObject("wallsContainer");
		wallsContainer.transform.position = transform.position;
		
		wallPrefab = Resources.Load("wall");

		InitialTransform = wallsContainer.transform;
		
		Vector3 initialPosition = InitialTransform.position - new Vector3(0.0f, SizeBarrier.y * 3.0f, 0.0f);
		initialPosition += (InitialTransform.right * (NbBarrier / 2)) * SizeBarrier.y / 2.0f;

		for (int i = 0; i < NbBarrier; ++i) {
			var wall = new Wall();
			wall.go = (GameObject) Instantiate(wallPrefab);
			wall.go.transform.localScale = SizeBarrier - new Vector3(0, 0, 0.5f) * 2.5f;
			var decalHeight = Random.Range(DecalHeightRange.x, DecalHeightRange.y);
			wall.speed = Random.Range(SpeedRange.x, SpeedRange.y);
			wall.decal_height = decalHeight;
			var decalY = Random.Range(DecalYRange.x, DecalYRange.y);
			wall.go.transform.Rotate(10.0f, 0, 20f);
			wall.go.transform.position = initialPosition + (-InitialTransform.right * SizeBarrier.x) * i;
			wall.go.transform.SetParent(wallsContainer.transform);
			walls.Add(wall);
		}

		for (int i = 0; i < NbBarrier; ++i) {
			var wall = new Wall();
			wall.go = (GameObject) Instantiate(wallPrefab);
			wall.go.transform.localScale = SizeBarrier;
			var decalHeight = Random.Range(DecalHeightRange.x, DecalHeightRange.y) * 0.5f;
			wall.speed = Random.Range(SpeedRange.x, SpeedRange.y);
			wall.decal_height = decalHeight;
			wall.go.transform.Rotate(10.0f, 0, 0);
			var decalY = Random.Range(DecalYRange.x, DecalYRange.y);
			wall.go.transform.position = initialPosition + new Vector3(-SizeBarrier.y, 0, SizeBarrier.x * -0.1f + decalY) + (-InitialTransform.right * SizeBarrier.x) * i;
			wall.go.transform.SetParent(wallsContainer.transform);
			walls.Add(wall);
		}

		for (int i = 0; i < NbBarrier; ++i) {
			var wall = new Wall();
			wall.go = (GameObject) Instantiate(wallPrefab);
			wall.go.transform.localScale = SizeBarrier - new Vector3(0f, 0, 0.5f);
			var decalHeight = Random.Range(DecalHeightRange.x, DecalHeightRange.y);
			wall.speed = Random.Range(SpeedRange.x, SpeedRange.y);
			wall.decal_height = decalHeight;
			var decalY = Random.Range(DecalYRange.x, DecalYRange.y);
			wall.go.transform.Rotate(10.0f, 0, -20);
			wall.go.transform.position = initialPosition + new Vector3(-SizeBarrier.y * 2, 0, 0 + decalY) + (-InitialTransform.right * SizeBarrier.x) * i;
			wall.go.transform.SetParent(wallsContainer.transform);
			walls.Add(wall);
		}

		wallsContainer.transform.Rotate(new Vector3(0, 90.0f, 0));

		GetComponent<AudioSource>().PlayOneShot(stoneGrinding[0]);
		GetComponent<AudioSource>().PlayOneShot(stoneGrinding[1]);
		GetComponent<AudioSource>().PlayOneShot(stoneGrinding[2]);
		GetComponent<AudioSource>().PlayOneShot(stoneGrinding[3]);
		GetComponent<AudioSource>().PlayOneShot(stoneGrinding[4]);
 	}

	public void FixedUpdate() {
		if (!walls.Any()) {
			Destroy(this);
			return;
		}

		var active_walls = new List<Wall>();
		foreach (var wall in walls) {
			wall.go.transform.position += wall.go.transform.up * wall.speed;

			if (wall.go.transform.position.y - InitialTransform.position.y > GoToHeight - wall.decal_height)
				continue;
			
			active_walls.Add(wall);
		}

		walls = active_walls;
	}
}
