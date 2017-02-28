using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SceneController : MonoBehaviour {
	public bool ViewMovieSequence = true;
	public bool OnlyDrawJutsu = false;
	private GameObject yugao;
	private GameObject yamato;
	private CharController yamatoController;
	private GameObject sasori;
	private CharController sasoriController;
	private GameObject camera;
	private bool yamatoLookAtSasori = false;
	public AudioClip epic_music;
	public AudioClip draw_sword;
	public AudioClip stab_wound;
	public AudioClip flap_cloth;
    public TextAsset pathsToDraw1;
    public TextAsset pathsToDraw2;
	public bool TurnHeadSasori;
	private GameObject SasoriHead;

	void Start () {
		SasoriHead = GameObject.Find("SasoriContainer/sasori/Armature/root_ground/pelvis/spine_lower/spine_upper/head_neck_lower");
		yugao = GameObject.Find("yugao");

		yamato = GameObject.Find("YamatoContainer");
		yamatoController = yamato.GetComponent<CharController>();
		camera = GameObject.Find("CameraEffect");
		camera.GetComponent<Camera>().tag = ViewMovieSequence ? "MainCamera" : "Untagged";

		sasori = GameObject.Find("SasoriContainer");
		sasoriController = sasori.GetComponent<CharController>();

		GameObject.Find("Main Camera").GetComponent<Camera>().tag = !ViewMovieSequence ? "MainCamera" : "Untagged";
		GameObject.Find("Main Camera").GetComponent<AudioListener>().enabled = !ViewMovieSequence;
		camera.GetComponent<AudioListener>().enabled = ViewMovieSequence;

		if (!ViewMovieSequence)
			return;

		yamatoController.Animated = true;
		sasoriController.Animated = true;
		yamatoController.Puppet = false;
		sasoriController.Puppet = true;
		yamatoController.InterceptInput = false;
		sasoriController.InterceptInput = false;
		DOTween.Init();

		StartCoroutine(BeginMovieSequence());;
	}

	IEnumerator BeginMovieSequence() {
		if (OnlyDrawJutsu) {
			yield return SequenceDrawJutsu();
			yield break;
		}

		// Disable unnecessary ragdoll colliders
		var colliders = sasori.GetComponentsInChildren<Collider>();
		foreach (Collider collider in colliders)
			collider.enabled = false;
		GameObject.Find("Kunai_block").GetComponent<Collider>().enabled = true;

		float delay = 0.0f;
		camera.transform.position = new Vector3(88, 767.6f, -286);
		yield return new WaitForSeconds(4.1f);

		camera.transform.DOMove(new Vector3(85.5f, 730.5f, -107), 8.0f - delay)
		.SetEase(Ease.InQuad)
		.OnComplete(() => {
			camera.transform.DOMove(new Vector3(86, 721, -24), 12.1f)
			.SetEase(Ease.OutQuad);
		});
		yield return new WaitForSeconds(17.2f - delay);

		camera.GetComponent<AudioSource>().PlayOneShot(epic_music);
		yield return new WaitForSeconds(5.5f);

		yamatoController.ThrowKunaiExplosive(sasori.transform.position + new Vector3(0, -0.1f, 	0.0f));

		// Sasori block kunai
		yield return new WaitForSeconds(0.35f);
		sasori.GetComponent<Collider>().enabled = false;
		sasoriController.GetComponentInChildren<Animator>().SetTrigger("FightingPose");
		StartCoroutine(AudioFadeOut.FadeOut(camera.GetComponent<AudioSource>(), 0.3f, 8.5f));

		// Display kunai explosion
		yield return new WaitForSeconds(1.0f);
		camera.transform.position = new Vector3(99.6f, 720.0f, -35.1f);
		camera.transform.rotation = Quaternion.Euler(-3.4f, -98.7f, 0);

		// Shake camera
		yield return new WaitForSeconds(2.25f);
		camera.GetComponent<CameraShake>().enabled = true;
		camera.GetComponent<CameraShake>().shakeDuration = 0.74f;
		camera.GetComponent<CameraShake>().shakeAmount = 0.03f;

		// Display Yugao
		yield return new WaitForSeconds(1.0f);
		camera.GetComponent<CameraShake>().enabled = false;
		camera.transform.DOMove(new Vector3(119.46f, 721.3f, -40.6f), 4.0f);
		camera.transform.DORotateQuaternion(Quaternion.Euler(-3.4f, -98.7f, 0), 4.0f);

		// Draw katana
		yield return new WaitForSeconds(5.0f);
		yugao.GetComponent<Animator>().SetTrigger("draw");
		yugao.GetComponent<AudioSource>().PlayOneShot(draw_sword, 4.0f);
		
		// Yamato go to tree
		yield return new WaitForSeconds(1.60f);
		yamato.transform.DORotate(new Vector3(-0.6f, -77f, 0.0f), 0.50f);
		yamatoController.ForceMoveForward = true;

		// Follow Yamato
		camera.transform.position = new Vector3(78.02925f, 720.803f, -32.61771f);
		camera.transform.rotation = Quaternion.Euler(-0.042f, -160.092f, 0f);
		camera.transform.DOMove(new Vector3(64.18915f, 720.8007f, -32.08497f), 2.7f).SetEase(Ease.Linear).OnComplete(() => {
			camera.transform.position = new Vector3(62.24498f, 725.2694f, -27.02685f);
			camera.transform.rotation = Quaternion.Euler(-0.989f, -175.217f, 0);
		});

		// Do jutsu feet wall climbing
		yield return new WaitForSeconds(0.8f);
		yamatoController.DoJutsu(CharController.Seal.Boar);

		yield return new WaitForSeconds(0.20f);
		yamatoController.DoJutsu(CharController.Seal.Attack);

		yield return new WaitForSeconds(0.25f);
		yamatoController.ActivateJutsu();
		sasori.GetComponent<AudioSource>().PlayOneShot(flap_cloth);

		// Jump to tree
		yield return new WaitForSeconds(0.95f);
		sasori.GetComponent<Collider>().enabled = true;
		yamatoController.ForceJump = true;

		// Look at Sasori from behind
		yield return new WaitForSeconds(2.05f);
		camera.transform.position = new Vector3(95.21884f, 720.8506f, -33.70605f);
		camera.transform.rotation = Quaternion.Euler(-4.598f, -91.816f, 0f);

		// Sasori jutsu shield wall
		yield return new WaitForSeconds(0.75f);
		sasoriController.DoJutsu(CharController.Seal.Dog);
		yield return new WaitForSeconds(0.20f);
		sasoriController.DoJutsu(CharController.Seal.Attack);
		yield return new WaitForSeconds(0.22f);
		sasoriController.ActivateJutsu();

		// Yamato jump from tree and face Sasori
		yamatoController.ForceJump = true;
		yield return new WaitForSeconds(0.1f);
		yamato.transform.rotation = Quaternion.Euler(0.6f, 77f, 0.0f);

		yield return new WaitForSeconds(0.35f);
		yamatoController.ForceMoveForward = false;
		yamato.GetComponentInChildren<Animator>().SetBool("Running", false); // Fix bug animator does not update out of camera

		// Look at shield wall
		yield return new WaitForSeconds(0.70f);
		camera.transform.position = new Vector3(93.35301f, 721.0123f, -34.67377f);
		camera.transform.rotation = Quaternion.Euler(-0.302f, 80.104f, 0f);
		camera.transform.DOMove(new Vector3(90.2216f, 721.0123f, -34.67373f), 2.5f).SetEase(Ease.InOutQuad);

		// Do jutsu flamethrower and wall shield
		yamatoController.DoJutsu(CharController.Seal.Attack);

		yield return new WaitForSeconds(0.20f);
		yamatoController.DoJutsu(CharController.Seal.Boar);

		yield return new WaitForSeconds(0.25f);
		yamato.transform.position = new Vector3(78.26944f, 720.52f, -32.13032f);
		yamato.transform.rotation = Quaternion.Euler(0f, 133f, 0.0f);
		camera.transform.position = new Vector3(78.18891f, 720.52f, -31.88908f);
		yamatoController.ActivateJutsu();

		yield return new WaitForSeconds(0.30f);		
		yamato.transform.rotation = Quaternion.Euler(0f, 133f, 0.0f);

		// Show yamato flamethrower and impact
		yield return new WaitForSeconds(4.05f);		
		camera.transform.position = new Vector3(78.118f, 721.0265f, -33.397f);
		camera.transform.rotation = Quaternion.Euler(2.793f, -324.828f, 0f);
		yield return new WaitForSeconds(0.75f);
		camera.transform.DOMove(new Vector3(77.7f, 720.6212f, -34.96f), 2.5f).SetEase(Ease.InOutQuad);
		camera.transform.DORotateQuaternion(Quaternion.Euler(-1.505f, -310.045f, 0f), 2.5f).SetEase(Ease.InOutQuad);

		// Display Sasori face
		yield return new WaitForSeconds(3.55f);
		camera.transform.position = new Vector3(93.46173f, 721.2806f, -33.73636f);
		camera.transform.rotation = Quaternion.Euler(-3.224f, 176.704f, 0f);

		// Yaguo impale Sasori
		yield return new WaitForSeconds(0.75f);
		yugao.GetComponent<Animator>().SetTrigger("charge");
		yugao.transform.position = new Vector3(95.366f, 719.93f, -34.75f);
		yugao.transform.rotation = Quaternion.Euler(0f, -73.428f, 0f);
		yugao.transform.DOMove(new Vector3(94.99f, 719.93f, -34.55f), 2.0f);
		yugao.GetComponent<AudioSource>().PlayOneShot(stab_wound, 4.0f);

		// Make Sasori face limp
		Rigidbody[] bodies = sasori.GetComponentsInChildren<Rigidbody>();
		foreach (Rigidbody rb in bodies)
			rb.isKinematic = true;
		sasori.GetComponentInChildren<Animator>().enabled = false;
		TurnHeadSasori = true;
		yield return new WaitForSeconds(0.45f);

		// Show backstabber
		yield return new WaitForSeconds(1.45f);
		camera.transform.DOMove(new Vector3(93.82977f, 721.1746f, -32.32128f), 2.5f).SetEase(Ease.InOutQuad);

		// Activate blood
		yield return new WaitForSeconds(0.25f);
		Transform[] trs = sasori.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in trs){
            if(t.name == "BloodStreamEffect"){
                t.gameObject.SetActive(true);
            }
        }
		yield return new WaitForSeconds(0.75f);
        foreach (Transform t in trs){
            if(t.name == "BloodSprayEffect"){
                t.gameObject.SetActive(true);
            }
        }

		// Fade out music
		StartCoroutine(AudioFadeOut.FadeOut(camera.GetComponent<AudioSource>(), 7.5f));

		yield return new WaitForSeconds(4.55f);
		yield return SequenceDrawJutsu();
	}

	IEnumerator SequenceDrawJutsu() {
		// Move yamato to proper place
		yamato.transform.position = new Vector3(93.96803f, 720.55f, -32.08865f);
		yamato.transform.rotation = Quaternion.Euler(0, 210, 0);
		camera.transform.position = new Vector3(93.96173f, 720.7806f, -35.43636f);
		camera.transform.rotation = Quaternion.Euler(-3.224f, 0.704f, 0f);

		// Disable unnecessary GOs
		yugao.SetActive(false); 
		sasori.SetActive(false);
		var walls = GameObject.Find("wallsContainer");
		if (walls) walls.SetActive(false);
		
		// Move Yamato and draw jutsu
		yield return new WaitForSeconds(1.55f);
		yamatoController.DoJutsu(CharController.Seal.Horse);
		yield return new WaitForSeconds(0.25f);
		yamatoController.DoJutsu(CharController.Seal.Attack);
		yield return new WaitForSeconds(0.23f);
		yamatoController.DoJutsu(CharController.Seal.Boar);
		yield return new WaitForSeconds(0.27f);
		yamatoController.DoJutsu(CharController.Seal.Dog);
		yield return new WaitForSeconds(0.35f);
		yamatoController.ActivateJutsu();
		// Use it programatically to change the drawing easily
		var drawing = new PathsDrawing();
        drawing.InitDataPoints(pathsToDraw1, yamato.transform.position + yamato.transform.forward * 2.5f + new Vector3(-3, 5.2f, 0.0f), yamato.transform.eulerAngles.y, true);
		drawing.NoiseZPos = new Vector2(2.0f, 0.0f);
        drawing.Launch();
		yield return new WaitForSeconds(0.55f);
		drawing = new PathsDrawing();
        drawing.InitDataPoints(pathsToDraw2, yamato.transform.position + yamato.transform.forward * 3.1f + new Vector3(-6.5f, 2.0f, 0.0f), yamato.transform.eulerAngles.y, true);
		drawing.NoiseZPos = new Vector2(2.0f, 0.0f);
        drawing.Launch();

		// Show jutsu drawing
		yield return new WaitForSeconds(0.50f);
		camera.transform.DOMove(new Vector3(93.46173f, 721.2806f, -38.73636f), 4.5f).SetEase(Ease.InQuad).OnComplete(() => {
			camera.transform.DOMove(new Vector3(93.46173f, 722.2806f, -43.73636f), 4.5f).SetEase(Ease.OutQuad);
		});
	}
	
	bool firstTurnedHead = true;
	Vector3 currentRotation;
	int nbUpdateRotated = 0;
	/// <summary>
    /// HUGE FIX to turn the head of sasori when he is hit, I've tried everything else to make it turn with physics but it just does not work.
	/// Also the frequent crash of Unity when I debug just make it to painful to search deeper
    /// </summary>
	void FixedUpdate () {
		if (TurnHeadSasori) {
			nbUpdateRotated++;

			if (firstTurnedHead) {
				currentRotation = SasoriHead.transform.eulerAngles;
				Debug.Log(currentRotation);
				firstTurnedHead = false;
			}

			currentRotation += new Vector3(25 / 20.0f, 20 / 20.0f, 4 / 20.0f);
			SasoriHead.transform.rotation = Quaternion.Euler(currentRotation);
			
			if (nbUpdateRotated == 20)
				TurnHeadSasori = false;
		}
	}
}
