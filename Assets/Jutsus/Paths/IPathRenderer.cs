using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPathRenderer {
	void Init(PathFollower pathFollower);
	void PathMoved();
	void PointTaken();
	void PathComplete();
}
