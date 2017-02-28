using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// C# translation from http://answers.unity3d.com/questions/155907/basic-movement-walking-on-walls.html
/// Author: UA @aldonaletto 
/// </summary>

// Prequisites: create an empty GameObject, attach to it a Rigidbody w/ UseGravity unchecked
// To empty GO also add BoxCollider and this script. Makes this the parent of the Player
// Size BoxCollider to fit around Player model.

public class CharController : MonoBehaviour
{
    public AudioClip soundActivationJutsu;
    public AudioClip[] soundsJutsu;
    public AudioClip soundKunaiThrown;
    public AudioClip soundJump;

    private float moveSpeed = 6; // move speed
    private float turnSpeed = 30; // turning speed (degrees/second)
    private float lerpSpeed = 10; // smoothing speed
    private float gravity = 10; // gravity acceleration
    public bool isGrounded;
    private float deltaGround = 0.2f; // character is grounded up to this distance
    private float jumpSpeed = 5; // vertical jump initial speed
    private float jumpRange = 7; // range to detect target wall
    private Vector3 surfaceNormal; // current surface normal
    public Vector3 myNormal; // character normal
    private float distGround; // distance from character position to ground
    private bool jumping = false; // flag &quot;I'm jumping to wall&quot;
    private bool jumpToWall = false;
    private float vertSpeed = 0; // vertical jump current speed
    private Rigidbody rigidbody;
    private UnityEngine.Object kunaiPrefab;
    private UnityEngine.Object kunaiExplosivePrefab;
    private UnityEngine.Object wallsPrefab;

    private Transform myTransform;
    public BoxCollider boxCollider; // drag BoxCollider ref in editor

    public TextAsset pathsToDraw;
    public bool InterceptInput = true;
    public bool ForceMoveForward = false;
    public bool ForceJump = false;
    public bool Animated = false;
    public bool Puppet = false;

    public enum Seal
    {
        Horse,
        Attack,
        Dog,
        Boar,
        Empty
    }

    private List<Seal> sealsCombination = new List<Seal>();

    private void Start()
    {
        // Cursor.lockState = CursorLockMode.Locked;
        Application.targetFrameRate = 60;

        kunaiPrefab = Resources.Load("Kunai");
        kunaiExplosivePrefab = Resources.Load("KunaiExplosive");
        wallsPrefab = Resources.Load("WallJutsu");

        boxCollider = GetComponent<BoxCollider>();
        rigidbody = GetComponent<Rigidbody>();
        myNormal = transform.up; // normal starts as character up direction
        myTransform = transform;
        rigidbody.freezeRotation = true; // disable physics rotation
                                         
        distGround = boxCollider.extents.y - boxCollider.center.y; // distance from transform.position to ground
    }

    private void FixedUpdate()
    {
        if (Puppet)
            return;

        // apply constant weight force according to character normal:
        rigidbody.AddForce(-gravity * rigidbody.mass * myNormal);
    }

    private void Update()
    {
        if (Puppet)
            return;

        // jump code - jump to wall or simple jump
        // if (jumpToWall) return; // abort Update while jumping to a wall

        Ray ray;
        RaycastHit hit;

        ManageEvents();


        if (Input.GetButtonDown("Jump") || ForceJump)
        { // jump pressed:
            ForceJump = false;
            GetComponent<AudioSource>().PlayOneShot(soundJump);
            jumping = true;
            surfaceNormal = Vector3.up;

            GetComponentInChildren<Animator>().SetTrigger("Jump");
            ray = new Ray(myTransform.position, myTransform.forward);
            if (Physics.Raycast(ray, out hit, jumpRange))
            { // wall ahead?
                rigidbody.velocity += jumpSpeed * myNormal;
                JumpToWall(hit.point, hit.normal); // yes: jump to the wall
            }
            else if (isGrounded)
            { // no: if grounded, jump up
                rigidbody.velocity += jumpSpeed * myNormal;
            }
        }

        // movement code - turn left/right with Horizontal axis:
        if (!Input.GetKeyDown(KeyCode.LeftControl) && !Animated)
            myTransform.Rotate(0, Input.GetAxis("Mouse X") * turnSpeed * Time.deltaTime, 0);
        if (!jumping)
        {
            // update surface normal and isGrounded:
            ray = new Ray(myTransform.position, -myNormal); // cast ray downwards
            if (Physics.Raycast(ray, out hit))
            { // use it to update myNormal and isGrounded
                isGrounded = hit.distance <= distGround + deltaGround;
                surfaceNormal = hit.normal;
            }
            else
            {
                isGrounded = false;
                // assume usual ground normal to avoid "falling forever"
                surfaceNormal = Vector3.up;
            }
        }
        myNormal = Vector3.Lerp(myNormal, surfaceNormal, lerpSpeed * Time.deltaTime);
        // find forward direction with new myNormal:
        Vector3 myForward = Vector3.Cross(myTransform.right, myNormal);
        // align character to the new myNormal while keeping the forward direction:
        if (!Animated) {
            Quaternion targetRot = Quaternion.LookRotation(myForward, myNormal);
            myTransform.rotation = Quaternion.Lerp(myTransform.rotation, targetRot, lerpSpeed * Time.deltaTime);
        }

        if (!Input.GetKeyDown(KeyCode.LeftControl))
            myTransform.Rotate(0, Input.GetAxis("Horizontal") * 100.0f * Time.deltaTime, 0);
        // move the character forth/back with Vertical axis:
        var moveForward = (ForceMoveForward ? 1.0f : Input.GetAxis("Vertical")) * moveSpeed * Time.deltaTime;
        GetComponentInChildren<Animator>().SetBool("Running", moveForward > 0);
        GetComponentInChildren<Animator>().SetBool("FightingPose", moveForward == 0);
        myTransform.Translate(0, 0, moveForward);
    }

    public void ThrowKunai(Vector3 to = default(Vector3)) {
        GetComponentInChildren<Animator>().SetTrigger("Throw");
        var kunai = (GameObject) Instantiate(kunaiPrefab);
        kunai.GetComponent<Kunai>().ThrowFrom(gameObject, to);
        GetComponent<AudioSource>().PlayOneShot(soundKunaiThrown);
    }

    public void ThrowKunaiExplosive(Vector3 to = default(Vector3)) {
        GetComponentInChildren<Animator>().SetTrigger("Throw");
        var kunai = (GameObject) Instantiate(kunaiExplosivePrefab);
        kunai.GetComponent<KunaiExplosive>().ThrowFrom(gameObject, to);
        GetComponent<AudioSource>().PlayOneShot(soundKunaiThrown);
    }

    private void ManageEvents()
    {
        if (Input.GetMouseButtonDown(1))
            ThrowKunai();
        if (Input.GetMouseButtonDown(0))
            ActivateJutsu();

        if (Input.GetKeyDown(KeyCode.Q))
            DoJutsu(Seal.Attack);
        else if (Input.GetKeyDown(KeyCode.E))
            DoJutsu(Seal.Boar);
        else if (Input.GetKeyDown(KeyCode.X))
            DoJutsu(Seal.Dog);
        else if (Input.GetKeyDown(KeyCode.C))
            DoJutsu(Seal.Horse);
    }

    public void DoJutsu(Seal seal) {
        if (seal == Seal.Attack)
        {
            GetComponentInChildren<Animator>().SetTrigger("AttackJutsu");
            GetComponent<AudioSource>().PlayOneShot(soundsJutsu[Random.Range(0, 3)]);
            sealsCombination.Add(Seal.Attack);
        }
        else if (seal == Seal.Boar)
        {
            GetComponentInChildren<Animator>().SetTrigger("BoarJutsu");
            GetComponent<AudioSource>().PlayOneShot(soundsJutsu[Random.Range(0, 3)]);
            sealsCombination.Add(Seal.Boar);
        }
        else if (seal == Seal.Dog)
        {
            GetComponentInChildren<Animator>().SetTrigger("DogJutsu");
            GetComponent<AudioSource>().PlayOneShot(soundsJutsu[Random.Range(0, 3)]);
            sealsCombination.Add(Seal.Dog);
        }
        else if (seal == Seal.Horse)
        {
            GetComponentInChildren<Animator>().SetTrigger("HorseJutsu");
            GetComponent<AudioSource>().PlayOneShot(soundsJutsu[Random.Range(0, 3)]);
            sealsCombination.Add(Seal.Horse);
        }
    }

    public void ActivateJutsu()
    {
        GetComponentInChildren<Animator>().SetTrigger("ActivationJutsu");
        GetComponent<AudioSource>().PlayOneShot(soundActivationJutsu);

        if (sealsAre(Seal.Attack, Seal.Boar, Seal.Empty, Seal.Empty)) {
            gameObject.AddComponent<FlamethrowerJutsu>();
        }
        else if (sealsAre(Seal.Boar, Seal.Dog, Seal.Attack, Seal.Empty)) {
            StartCoroutine(UpdateBranchAttack(0.0f));
            StartCoroutine(UpdateBranchAttack(1.5f));
        }
        else if (sealsAre(Seal.Boar, Seal.Attack, Seal.Empty, Seal.Empty))
           ActivateJutsuFeet();
        else if (sealsAre(Seal.Dog, Seal.Attack, Seal.Empty, Seal.Empty))
           createWalls();
        else if (sealsAre(Seal.Dog, Seal.Attack, Seal.Boar, Seal.Horse))
            drawJutsu();

        sealsCombination.Clear();
    }

    public void drawJutsu(Vector3 offsetPosition = default(Vector3)) {
        var drawing = new PathsDrawing();
        drawing.InitDataPoints(pathsToDraw, offsetPosition + transform.position + transform.forward * 2.5f + new Vector3(-10, 2.0f, 0.0f), transform.eulerAngles.y, true);
        drawing.Launch();
    }

    public void createWalls() {
        var wallsGo = (GameObject) Instantiate(wallsPrefab, transform.position + transform.GetChild(0).forward * 3.0f, transform.GetChild(0).rotation);
        wallsGo.GetComponent<WallJutsu>().Launch();
    }

    public void ActivateJutsuFeet()
    {
        Transform[] trs = transform.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in trs){
            if(t.name == "FeetAura"){
                t.gameObject.SetActive(true);
            }
        }
    }

    private IEnumerator UpdateBranchAttack(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        var prefabPath = Resources.Load("Path");
        var branch = GameObject.Instantiate(prefabPath) as GameObject;
        var cameraCharacter = GameObject.Find("Main Camera");
        branch.GetComponent<PathFollower>().SizeStep *= 2.0f;
        var meshRenderer = new PathMeshRenderer();
        meshRenderer.ChunkRadius = 0.2f;
        meshRenderer.ChunkRadiusBeforeFirstPoint = 0.2f;
        branch.GetComponent<PathFollower>().PathRenderer = meshRenderer;
        branch.GetComponent<PathFollower>().InitialPosition = transform.position - GetComponent<BoxCollider>().size / 2;
        branch.GetComponent<PathFollower>().InitialRotation = Quaternion.Euler(-90.0f, 0.0f, 0.0f);

        float nbSecondsToWait = 3.0f;
        for (float secondsWaited = 0.0f; secondsWaited < nbSecondsToWait; secondsWaited += 0.1f)
        {
            Ray ray;
            RaycastHit hit;
            var target = cameraCharacter.transform.position + cameraCharacter.transform.forward * 125.0f;
            ray = new Ray(cameraCharacter.transform.position, cameraCharacter.transform.forward);
            if (Physics.Raycast(ray, out hit, 125.0f))
                target = hit.point;

            branch.GetComponent<PathFollower>().Target = target;
            yield return new WaitForSeconds(0.1f);
        } 

        branch.GetComponent<PathFollower>().Finished = true;
    }

    private bool sealsAre(Seal firstSeal, Seal secondSeal, Seal thirdSeal, Seal fourthSeal)
    {
        while (sealsCombination.Count() < 4)
            sealsCombination.Add(Seal.Empty);
        
        var seals = sealsCombination.Skip(sealsCombination.Count() - 4).Take(4).ToList();

        return firstSeal == seals[0] && secondSeal == seals[1] && thirdSeal == seals[2] && fourthSeal == seals[3];
    }

    private void JumpToWall(Vector3 point, Vector3 normal)
    {
        // jump to wall
        jumpToWall = true; // signal it's jumping to wall
                           //  rigidbody.isKinematic = true; // disable physics while jumping
        Vector3 orgPos = myTransform.position;
        Quaternion orgRot = myTransform.rotation;
        Vector3 dstPos = point + normal * (distGround + 0.5f); // will jump to 0.5 above wall
        Vector3 myForward = Vector3.Cross(myTransform.right, normal);
        Quaternion dstRot = Quaternion.LookRotation(myForward, normal);

        StartCoroutine(jumpTime(orgPos, orgRot, dstPos, dstRot, normal));
        //jumptime
    }

    private IEnumerator jumpTime(Vector3 orgPos, Quaternion orgRot, Vector3 dstPos, Quaternion dstRot, Vector3 normal)
    {
        for (float t = 0.0f; t < 1.0f;)
        {
            t += Time.deltaTime * 2.0f;
            //  myTransform.position = Vector3.Lerp(myTransform.position, dstPos, 0.1f);
            myTransform.rotation = Quaternion.Slerp(orgRot, dstRot, t);
            yield return null; // return here next frame
        }
        myNormal = normal; // update myNormal
        rigidbody.isKinematic = false; // enable physics
        jumping = false; // jumping to wall finished
        jumpToWall = false;
    }

}