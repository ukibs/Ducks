using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public enum MovementStates
{
	Invalid = -1,
	Walking,
	Running,
	Crouching,
	Inmovile,
	Count
}

public class PlayerController : NetworkBehaviour {

    public float movementSpeed = 5.0f;
	public float runSpeed = 10.0f;
	public float crouchSpeed = 3.0f;
	public float jumpForce = 13.0f;

    public Camera cam;
    public GameObject bulletPrefab;
    public GameObject myPrefab;
	public Transform shootPoint;
	public Vector3 gravity = new Vector3(0.0f, -9.81f, 0.0f);

    private float fireRate = 0.5f;
    private float fireCooldown = 0.0f;
	private MovementStates movementState = MovementStates.Walking;
	private CharacterController controller;
	private float verticalSpeed = 0.0f;


	private float vAxis;
	private float hAxis;
	private bool shiftKey;
	private bool ctrlKey;
	private bool spaceKey;

	private float mouseX;
	private float mouseY;
	private bool mouseLeft;
	private bool mouseRight;

	// Use this for initialization
	void Start () {
		cam = GetComponentInChildren<Camera> ();
		cam.enabled = true;
		controller = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () {
		// Delta time
		float dt = Time.deltaTime;

        if (!isLocalPlayer)
        {
            cam.enabled = false;
            gameObject.GetComponentInChildren<AudioListener>().enabled = false;
            return;
        }
        else
        {
			UpdateInput ();
			ChangeStates ();
			ApplyGravity (dt);
			if (controller.isGrounded && spaceKey) 
			{
				Jump ();
			}
			SimpleShoot (dt);
			UpdateMovement (dt);
        }
	}

	void ChangeStates()
	{
		if (shiftKey){
			if(movementState == MovementStates.Walking) {
			movementState = MovementStates.Running;
			} else
			movementState = MovementStates.Walking;
		}

		if (ctrlKey) {
			if (movementState != MovementStates.Crouching) {
				movementState = MovementStates.Crouching;
			} else
				movementState = MovementStates.Walking;
		}
			
	}

	void UpdateMovement(float dt)
	{
		switch (movementState) 
		{
		case MovementStates.Walking:
			Movement (dt, movementSpeed);
			controller.height = 2;
			break;
		case MovementStates.Running:
			Movement (dt, runSpeed);
			break;
		case MovementStates.Crouching:
			Movement (dt, crouchSpeed);
			controller.height = 1.5f;
			break;
		case MovementStates.Inmovile:
			break;
		default:
			break;
		}
	}

	void UpdateInput()
	{
		vAxis = Input.GetAxis("Vertical");
		hAxis = Input.GetAxis("Horizontal");
		shiftKey = Input.GetKeyDown (KeyCode.LeftShift);
		ctrlKey = Input.GetKeyDown (KeyCode.LeftControl);
		spaceKey = Input.GetKeyDown (KeyCode.Space);

		mouseX = Input.GetAxis ("Mouse X");
		mouseY = Input.GetAxis ("Mouse Y");
		mouseLeft = Input.GetMouseButtonDown (0);
		mouseRight = Input.GetMouseButtonDown (1);
	}



	void Movement(float dt, float speed)
	{
		Vector3 rightMovement = transform.right * hAxis * speed;
		Vector3 forwardMovement = transform.forward * vAxis * speed;
		Vector3 yMovement = transform.up * verticalSpeed;
		controller.Move((rightMovement + forwardMovement + yMovement)* dt);
		transform.Rotate(0.0f, mouseX * 90.0f * dt, 0.0f);
		cam.transform.Rotate(mouseY * -90.0f * dt, 0.0f, 0.0f);
	}
		
	void SimpleShoot(float dt)
	{
		// Shoot
		if (fireCooldown < fireRate) fireCooldown += dt;
		if(Input.GetAxis("Fire1") != 0.0f && fireCooldown >= fireRate)
		{
			CmdFire();
			fireCooldown = 0.0f;
		}
	}

	void ApplyGravity(float dt)
	{
		if (controller.isGrounded)
			verticalSpeed = 0.0f;
		else
			verticalSpeed += gravity.y * dt;
	}

	void Jump()
	{
		verticalSpeed = jumpForce;
	}

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        this.gameObject.GetComponent<MeshRenderer>().material.color = Color.cyan;
        GameObject enemySkin = Instantiate(myPrefab, transform.position, transform.rotation);
        enemySkin.transform.parent = gameObject.transform;
    }

    [Command]
    void CmdFire()
    {
		GameObject newBullet = GameObject.Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);
        newBullet.GetComponent<Rigidbody>().velocity = newBullet.transform.forward * 10f;

        NetworkServer.Spawn(newBullet);

        Destroy(newBullet, 4.0f);
    }
}
