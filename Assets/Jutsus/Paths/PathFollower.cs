using UnityEngine;
using System;
using System.Collections;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
	
public class PathFollower : MonoBehaviour
{
	[NonSerialized]
	public float SizeStep = 0.035f;
	public GameObject goCenter; // GameObject where the current path position is
	private int nbFrame = 0;
	public Vector3 Target = Vector3.zero;
	public List<Vector3> Targets;
	private bool onlyOneTarget;
	public int currentTargetIndex = 0;
	private Vector3 previousDistanceToTarget = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
	public bool Finished = false;
	public Vector3 InitialPosition = Vector3.zero;
	public Quaternion InitialRotation = Quaternion.identity;
	private float DistanceCloseEnoughFromPoint = 0.35f; // Must be superior to size step to avoid locking and possibly more to avoid going back and forth
	public IPathRenderer PathRenderer = null;
	private float LerpToTargetPoint = 50.0f;

	public void Start()
	{
		if (PathRenderer == null)
			PathRenderer = new PathMeshRenderer();

		goCenter = new GameObject("goCenter");
		goCenter.transform.position = InitialPosition;
		goCenter.transform.rotation = InitialRotation;

		onlyOneTarget = Targets.Count() == 0;
		
		PathRenderer.Init(this);
	}

	public void Update()
	{
		if (Finished)
			return;

		MovePointForward();
		++nbFrame;
	}
	
	public void MovePointForward()
	{	
		var target = onlyOneTarget ? Target : Targets[currentTargetIndex];
		var directionTarget = target - goCenter.transform.position;
		var TargetRotation = Quaternion.LookRotation(directionTarget);
		var str = Mathf.Min(LerpToTargetPoint * Time.deltaTime, 1);

		goCenter.transform.rotation = Quaternion.Lerp(goCenter.transform.rotation, TargetRotation, str);
		goCenter.transform.Translate(Vector3.forward * SizeStep);

		PathRenderer.PathMoved();

		if (onlyOneTarget)
			return;

		// if (directionTarget.IsGreaterThan(previousDistanceToTarget)) {
		if (directionTarget.SumAxis() < DistanceCloseEnoughFromPoint) {
			++currentTargetIndex;
			PathRenderer.PointTaken();

			if (currentTargetIndex >= Targets.Count()) {
				Finished = true;
				PathRenderer.PathComplete();				
			}
		}

		previousDistanceToTarget = directionTarget;
	}
}

public static class Extensions
{
	public static bool IsGreaterThan(this Vector3 v1, Vector3 v2)
	{
		return (v1.SumAxis() > v2.SumAxis());
	}

	public static float SumAxis(this Vector3 v1)
	{
		return (Mathf.Abs(v1.x) + Mathf.Abs(v1.y) + Mathf.Abs(v1.z));
	}
}  