using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Basics : MonoBehaviour
{
	public Transform cubeA, cubeB;

	void Start()
	{
		// Initialize DOTween (needs to be done only once).
		// If you don't initialize DOTween yourself,
		// it will be automatically initialized with default values.
		DOTween.Init(false, true, LogBehaviour.ErrorsOnly);

		// Create two identical infinitely looping tweens,
		// one with the shortcuts way and the other with the generic way.
		// Both will be set to "relative" so the given movement will be calculated
		// relative to each target's position.

		// cubeA > SHORTCUTS WAY
		cubeA.DOMove(new Vector3(-2, 2, 0), 3).SetRelative().SetDelay(1).SetEase(Ease.InOutQuad);
		cubeA.DOMove(new Vector3(2, -2, 0), 3).SetRelative().SetDelay(4).SetEase(Ease.InOutQuad);

		// cubeB > GENERIC WAY
		DOTween.To(()=> cubeB.position, x=> cubeB.position = x, new Vector3(-2, 2, 0), 3).SetRelative().SetLoops(-1, LoopType.Yoyo).SetDelay(1);

		// Voilà.
		// To see all available shortcuts check out DOTween's online documentation.
	}
}