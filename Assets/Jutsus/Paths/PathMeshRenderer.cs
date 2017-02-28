using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathMeshRenderer : IPathRenderer {
	private PathFollower pathFollower;
	// GameObjects for mesh rendering and chunk creation
	private GameObject goRing;
	private GameObject goMesh;
	// Properties rendering
	public float ChunkRadius = 0.12f;
	public float ChunkRadiusBeforeFirstPoint = 0.05f;
	public float ChunkRadiusIncreaseSize = 0.0045f;
	public int RadialSegmentCount = 10;
	public int UvHeightSegmentCount = 10;
	private float currentChunkRadius;
	private int meshNbChunk = 0;
	private MeshBuilder meshBuilder = new MeshBuilder();

	public void Init(PathFollower pathFollower) {
		this.pathFollower = pathFollower;
		currentChunkRadius = ChunkRadiusBeforeFirstPoint; 

		goRing = new GameObject("goRing");

		goMesh = new GameObject("goMesh");
		goMesh.transform.position = pathFollower.goCenter.transform.position;
		goMesh.AddComponent<MeshRenderer>();
		goMesh.AddComponent<MeshFilter>();
		// Collision
		// goMesh.AddComponent<MeshCollider>();

		goMesh.GetComponent<Renderer>().material = Resources.Load("PathMaterial", typeof(Material)) as Material;
		// goMesh.GetComponent<Renderer>().material.color = Color.yellow;
		// goMesh.GetComponent<MeshRenderer>().material.mainTexture = Resources.Load("PathTexture", typeof(Texture)) as Texture;
	}

	public void PathMoved() {
		if (pathFollower.currentTargetIndex > 0 && currentChunkRadius < ChunkRadius)
			currentChunkRadius += ChunkRadiusIncreaseSize;


		var nbPointsInSegment = RadialSegmentCount + 1;
		if (meshNbChunk > 1) {
			meshBuilder.Vertices.RemoveRange(meshBuilder.Vertices.Count - nbPointsInSegment , nbPointsInSegment);
			meshBuilder.UVs.RemoveRange(meshBuilder.Vertices.Count - nbPointsInSegment , nbPointsInSegment);
			meshBuilder.Triangles.RemoveRange(meshBuilder.Triangles.Count - (nbPointsInSegment * 6 - 6) , nbPointsInSegment * 6 - 6);
		}

		BuildRing(meshBuilder, RadialSegmentCount, currentChunkRadius, meshNbChunk > 0);

		if (meshNbChunk > 0) {
			pathFollower.goCenter.transform.Translate(Vector3.forward * pathFollower.SizeStep * 4);
			BuildRing(meshBuilder, RadialSegmentCount, 0.0f, true);
			pathFollower.goCenter.transform.Translate(-Vector3.forward * pathFollower.SizeStep * 4);
		}

		Mesh mesh = meshBuilder.CreateMesh();
		goMesh.GetComponent<MeshFilter>().mesh = mesh;
		// For collision
		// goMesh.GetComponent<MeshCollider>().sharedMesh = mesh;

		++meshNbChunk;
	}


	float currentUvHeight = 0.0f;
	private void BuildRing(MeshBuilder meshBuilder, int segmentCount, float radius, bool buildTriangles)
	{
		if (meshNbChunk + 1 % UvHeightSegmentCount == 0)
			currentUvHeight = 0.0f;

		float angleInc = 360.0f / segmentCount;
		for (int i = 0; i <= segmentCount; i++)
		{
			float angle = angleInc * i;

			goRing.transform.position = pathFollower.goCenter.transform.position;
			goRing.transform.rotation = pathFollower.goCenter.transform.rotation;
			goRing.transform.Rotate(0.0f, 90.0f, 0.0f);
			goRing.transform.Rotate(angle, 0.0f, 0.0f);
			goRing.transform.Translate(Vector3.forward * radius);

			// Debug Vertices
			// GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			// sphere.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
			// sphere.GetComponent<Renderer>().material.color = Color.blue;
			// sphere.transform.position = goRing.transform.position;

			meshBuilder.Vertices.Add(goRing.transform.position - goMesh.transform.position);
			meshBuilder.UVs.Add(new Vector2((float) i / segmentCount, currentUvHeight));

			if (i > 0 && buildTriangles)
			{
				int baseIndex = meshBuilder.Vertices.Count - 1;

				int vertsPerRow = segmentCount + 1;

				int index0 = baseIndex;
				int index1 = baseIndex - 1;
				int index2 = baseIndex - vertsPerRow;
				int index3 = baseIndex - vertsPerRow - 1;

				meshBuilder.AddTriangle(index0, index2, index1);
				meshBuilder.AddTriangle(index2, index3, index1);
			}
		}

		currentUvHeight +=  1.0f / UvHeightSegmentCount;
	}

	public void PointTaken() {
	}

	public void PathComplete() {

	}
}
