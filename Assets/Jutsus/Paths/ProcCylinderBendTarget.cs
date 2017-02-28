using UnityEngine;
using System.Collections;

public class ProcCylinderBendTarget : MonoBehaviour
{
	public float m_Radius = 0.5f;
	public float m_Height = 2.0f;
	public float m_BendAngle = 90.0f;
	public int m_RadialSegmentCount = 10;
	public int m_HeightSegmentCount = 4;
	MeshBuilder meshBuilder = new MeshBuilder();
	public GameObject target;
	public TextAsset asset_data_points;

	//Build the mesh:
	public void Start()
	{
		// var drawingJutsu = new PathsDrawing();
		// drawingJutsu.InitDataPoints(asset_data_points, transform.position, true);
		// drawingJutsu.Launch();

		goRing = new GameObject("goRing");
		goCenter = new GameObject("goCenter");
		goCenter.transform.position = transform.position;
		GetComponent<Renderer>().material.color = Color.yellow;

		// Invoke("MovePointForward", 0.05f);
	}

	private Vector3 prev_angles = Vector3.zero;
	public Vector3 angles = Vector3.zero;
	public float size_step = 0.05f;
	public float speed  = 0.2f;
	public Vector3 position;
	public GameObject goCenter;
	public int nbFrame = 0;
	public void MovePointForward() {
		var targetRotation = Quaternion.LookRotation(target.transform.position - goCenter.transform.position);
		var str = Mathf.Min(1.0f * Time.deltaTime, 1);
		goCenter.transform.rotation = Quaternion.Lerp(goCenter.transform.rotation, targetRotation, str);

		goCenter.transform.Translate(Vector3.forward * size_step);
		prev_angles = angles;

		float v = (float)nbFrame / m_HeightSegmentCount;

		int nbPointsInSegment = m_RadialSegmentCount + 1;
		if (nbFrame > 5) {
			// meshBuilder.Vertices.RemoveRange(meshBuilder.Vertices.Count - nbPointsInSegment , nbPointsInSegment);
			// meshBuilder.Triangles.RemoveRange(meshBuilder.Triangles.Count - (nbPointsInSegment * 6 - 6) , nbPointsInSegment * 6 - 6);
		}

		BuildRing(meshBuilder, m_RadialSegmentCount, m_Radius, v, nbFrame > 0);

		if (nbFrame > 4) {
			// goCenter.transform.Translate(Vector3.forward * size_step * 4);
			// BuildRing(meshBuilder, m_RadialSegmentCount, 0.0f, v, true);
			// goCenter.transform.Translate(-Vector3.forward * size_step * 4);
		}
		

		Mesh mesh = meshBuilder.CreateMesh();
		GetComponent<MeshFilter>().mesh = mesh;

		Invoke("MovePointForward", speed);
		++nbFrame;
	}

	private GameObject goRing;
	public Vector2 UVs = Vector2.zero;
	public Vector3 Normals = Vector3.zero;
	public float v = 0.0f;
	/// <summary>
	/// Builds a ring as part of a cylinder.
	/// </summary>
	/// <param name="meshBuilder">The mesh builder currently being added to.</param>
	/// <param name="segmentCount">The number of segments in this ring.</param>
	/// <param name="centre">The position at the centre of the ring.</param>
	/// <param name="radius">The radius of the ring.</param>
	/// <param name="v">The V coordinate for this ring.</param>
	/// <param name="buildTriangles">Should triangles be built for this ring? This value should be false if this is the first ring in the cylinder.</param>
	private void BuildRing(MeshBuilder meshBuilder, int segmentCount, float radius, float v, bool buildTriangles)
	{
		float angleInc = 360.0f / segmentCount;
		for (int i = 0; i <= segmentCount; i++)
		{
			float angle = angleInc * i;

			goRing.transform.position = goCenter.transform.position;
			goRing.transform.rotation = goCenter.transform.rotation;
			goRing.transform.Rotate(0.0f, 90.0f, 0.0f);
			goRing.transform.Rotate(angle, 0.0f, 0.0f);
			goRing.transform.Translate(Vector3.forward * radius);

			// Debug Vertices
			// GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			// sphere.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
			// sphere.GetComponent<Renderer>().material.color = Color.blue;
			// sphere.transform.position = goRing.transform.position;

			meshBuilder.Vertices.Add(goRing.transform.position - transform.position);
			meshBuilder.UVs.Add(new Vector2(i / segmentCount, v));

			v += 0.2f;
			if (v > 1.0f) v = 0.0f;

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
	}
}