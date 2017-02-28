using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalEvent : MonoBehaviour {

	// Use this for initialization
	void Start () {
		// UnityEditor.SceneView.FocusWindowIfItsOpen(typeof(UnityEditor.SceneView));
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.P) && Input.GetKey(KeyCode.RightShift))
			Debug.Break();
	}
}
