using System.Globalization;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathsDrawing {
	private TextAsset assetDataPoints;
	private List<List<Vector3>> paths_data_points = new List<List<Vector3>>();
	public int NbPointsPerPath = 200;
	public float Scale = 0.03f;
	public Vector3 InitialPosition;
	private bool ReverseYAxis = true;
	private float YRotation = 0.0f;
	public Vector2 NoiseXPos = new Vector2(-2, 2);
	public Vector2 NoiseYPos = new Vector2(-2, 0);
	public Vector2 NoiseZPos = new Vector2(-2, 2);
	public Vector2 NoiseXRot = new Vector2(-30, 30);
	public Vector2 NoiseZRot = new Vector2(-30, 30);
	public bool StartOnGround = true;

	/// <summary>
    /// Init datas from file
    /// </summary>
    /// <param name="assetDataPoints">Asset file containing path of points (see https://shinao.github.io/PathToPoints/)</param>
    /// <param name="initialPosition">Initial position of all the branches</param>
	public void InitDataPoints(TextAsset assetDataPoints, Vector3 initialPosition, float yRotation, bool reverseYAxis = true) {
		ReverseYAxis = reverseYAxis;
		InitialPosition = initialPosition;
		paths_data_points.Clear();

		Vector2 minValue = new Vector2(float.MaxValue, float.MaxValue);
		Vector2 maxValue = new Vector2(float.MinValue, float.MinValue);

		 var str_path_points = assetDataPoints.text;
		 var str_paths = str_path_points.Split(new char[] { '#' }, System.StringSplitOptions.RemoveEmptyEntries).Where(branch => branch.Count() > 2).ToArray();

		 foreach (var str_path in str_paths)
		 {
			 var data_points = new List<Vector3>();
			 var lines_points = str_path.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries).Where(line_point => line_point.Count() > 2).ToArray();;
			 foreach (var line_point in lines_points)
			 {
				var point = line_point.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
				var x = float.Parse(point[0], CultureInfo.InvariantCulture);
				var y = float.Parse(point[1], CultureInfo.InvariantCulture);

				data_points.Add(new Vector3(x, y, 0.0f));
			 }

			 minValue.y = Mathf.Min(minValue.y, data_points.Select(v => v.y).Min());
			 maxValue.y = Mathf.Max(maxValue.y, data_points.Select(v => v.y).Max());
			 minValue.x = Mathf.Min(minValue.x, data_points.Select(v => v.x).Min());
			 maxValue.x = Mathf.Max(maxValue.x, data_points.Select(v => v.x).Max());

			 paths_data_points.Add(data_points);
		 }

		ScaleAndReposition(minValue, maxValue);
	}

	/// <summary>
    /// Scale and reposition path depending on min/max and target value
    /// </summary>
    /// <param name="minValue">Min x,y value of all paths points</param>
    /// <param name="maxValue">Max x,y value of all paths points</param>
	public void ScaleAndReposition(Vector2 minValue, Vector2 maxValue)
	{
		foreach (var path_data_points in paths_data_points)
		{
			for (int iPoint = 0; iPoint < path_data_points.Count(); ++iPoint)
			{
				var newPoint = path_data_points[iPoint];
				if (ReverseYAxis)
				{
					newPoint = new Vector3(newPoint.x, newPoint.y * -1.0f, newPoint.z);
					newPoint = newPoint + new Vector3(0.0f, minValue.y + maxValue.y, 0.0f);
				}
				newPoint = newPoint - new Vector3(minValue.x, minValue.y, 0.0f);
				newPoint = newPoint * Scale;
				newPoint = newPoint + InitialPosition;
				
				path_data_points[iPoint] = newPoint;
			}
		}

		// YRotation
		// var nb_paths = paths_data_points.Count() - 1;
		// Vector3 center_drawing = paths_data_points[0][0] + ((paths_data_points[nb_paths][paths_data_points[nb_paths].Count() - 1] - paths_data_points[0][0]) / 2);
		// Debug.Log(center_drawing);
		// foreach (var path_data_points in paths_data_points)
		// {
		// 	for (int iPoint = 0; iPoint < path_data_points.Count(); ++iPoint)
		// 	{
		// 		var newPoint = path_data_points[iPoint];
		// 		var dist_from_center = newPoint - center_drawing;
		// 		newPoint = center_drawing + new Vector3(Mathf.Cos(YRotation) * dist_from_center.x, Mathf.Sin(YRotation) * dist_from_center.y, 0.0f);
				
		// 		path_data_points[iPoint] = newPoint;
		// 	}
		// }
	}
	
	/// <summary>
    /// Launch all paths after init 
    /// </summary>
	public void Launch() {
		foreach (var path_data_points in paths_data_points)
		{
			int nbPoints = path_data_points.Count();
			int nbOfBranches = nbPoints / NbPointsPerPath;
			if (nbPoints % NbPointsPerPath != 0)
				nbOfBranches += 1;

			for (int iBranch = 0; iBranch < nbOfBranches; ++iBranch)
			{
				var data_points_current_branch = path_data_points.GetRange(iBranch * NbPointsPerPath, Mathf.Min(NbPointsPerPath, nbPoints - iBranch * NbPointsPerPath));
				
				var branch = new GameObject();
				branch.AddComponent<PathFollower>();
				branch.GetComponent<PathFollower>().Targets = data_points_current_branch;

				var branch_start_position = data_points_current_branch[0];
				if (StartOnGround) {
					// Debug.DrawRay(branch_start_position, -Vector3.up, Color.red, 5000.0f);
					RaycastHit raycast;
					if (Physics.Raycast(branch_start_position, -Vector3.up, out raycast))
						branch_start_position = raycast.point - new Vector3(0.0f, 1.0f, 0.0f);
				}

				branch_start_position += new Vector3(Random.Range(NoiseXPos.x, NoiseXPos.y), Random.Range(NoiseYPos.x, NoiseYPos.y), Random.Range(NoiseZPos.x, NoiseZPos.y));

				branch.GetComponent<PathFollower>().InitialPosition = branch_start_position;
				branch.GetComponent<PathFollower>().InitialRotation = Quaternion.Euler(90.0f + Random.Range(NoiseXRot.x, NoiseXRot.y), 0.0f, Random.Range(NoiseZRot.x, NoiseZRot.y));
				// branch.GetComponent<PathFollower>().InitialRotation = Quaternion.Euler(-90.0f, 0.0f, 0.0f);
			}
		}
	}
}
