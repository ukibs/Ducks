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
	public GameObject lifePrefab;
	public GameObject ammunition;
    public GameObject bulletPrefab;
    public GameObject myPrefab;
    public List<GameObject> weaponPrefabs;
    public GameObject grenadePrefab;
	public Transform weaponPoint;
	public Vector3 gravity = new Vector3(0.0f, -9.81f, 0.0f);

    // private float fireRate = 0.5f;
    // private float fireCooldown = 0.0f;
	private MovementStates movementState = MovementStates.Walking;
	private CharacterController controller;
	private float verticalSpeed = 0.0f;
    // private BaseWeapon currentWeapon = null;
    private int currentWeaponIndex = 0;
    private List<GameObject> weapons;


	private float vAxis;
	private float hAxis;
	private bool shiftKey;
	private bool ctrlKey;
	private bool spaceKey;
    private bool tabKey;
	private bool rKey;

	private float mouseX;
	private float mouseY;
	// private bool mouseLeft;
	private bool mouseRight;

    // Variable de prueba
    public Door door;

    #region Properties

    public BaseWeapon CurrentWeapon { get { return weapons[currentWeaponIndex].GetComponent<BaseWeapon>(); } }

    #endregion

    // Use this for initialization
    void Start () {
		cam = GetComponentInChildren<Camera> ();
		cam.enabled = true;
		controller = GetComponent<CharacterController>();
        InitializeWeapons();
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
			if (rKey)
				CurrentWeapon.Reload ();
            if (tabKey) ChangeWeapon();
			if (mouseRight) CmdThrowGrenade();
			SimpleShoot (dt);
			UpdateMovement (dt);
            //
            if (Input.GetKeyDown(KeyCode.E))
            {
                CmdUseObject();
            }
        }
	}

    private void OnGUI()
    {
		if (isLocalPlayer) {
			BaseWeapon weaponData = weapons [currentWeaponIndex].GetComponent<BaseWeapon> ();
			GUI.Label (new Rect (20, 20, 100, 20), weaponData.CurrentWeaponAmmo + "/" + weaponData.CurrentReserveAmmo);
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
        tabKey = Input.GetKeyDown(KeyCode.Tab);
		rKey = Input.GetKeyDown (KeyCode.R);

		mouseX = Input.GetAxis ("Mouse X");
		mouseY = Input.GetAxis ("Mouse Y");
		//mouseLeft = Input.GetMouseButtonDown (0);
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

    void ChangeWeapon()
    {
        weapons[currentWeaponIndex].SetActive(false);
        currentWeaponIndex++;
        if (currentWeaponIndex >= weapons.Count) currentWeaponIndex = 0;
        // InitiateWeapon();
        weapons[currentWeaponIndex].SetActive(true);
    }

    void InitializeWeapons()
    {
        //Debug.Log(weapons[currentWeaponIndex]);
        /*GameObject newWeapon = Instantiate(weapons[currentWeaponIndex].gameObject, weaponPoint);
        newWeapon.transform.localPosition = Vector3.zero;
        currentWeapon = newWeapon.GetComponent<BaseWeapon>();*/
        weapons = new List<GameObject>(weaponPrefabs.Count);
        for(int i = 0; i < weaponPrefabs.Count; i++)
        {
            GameObject newWeapon = Instantiate(weaponPrefabs[i], weaponPoint);
            newWeapon.transform.localPosition = Vector3.zero;
            if(i > 0) newWeapon.SetActive(false);
            weapons.Add(newWeapon);
        }
    }

	[Command]
    void CmdThrowGrenade()
    {
        Vector3 grenadePosition = weaponPoint.position + weaponPoint.forward;
        Quaternion grenadeOrientation = weaponPoint.rotation;
        grenadeOrientation *= Quaternion.Euler(-45.0f, 0.0f, 0.0f);
        GameObject newGrenade = Instantiate(grenadePrefab, grenadePosition, grenadeOrientation);
        newGrenade.GetComponent<Rigidbody>().velocity = newGrenade.transform.forward * 20.0f;

		NetworkServer.Spawn (newGrenade);
        //Destroy(newGrenade, 4);
    }
		
	void SimpleShoot(float dt)
	{
		
        if(CurrentWeapon.OrderFire()) CmdFire();
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
        //GameObject enemySkin = Instantiate(myPrefab, transform.position, transform.rotation);
        //enemySkin.transform.parent = gameObject.transform;
    }

	[Command]
	public void CmdThrowItems()
	{
		GameObject itemLife = GameObject.Instantiate(lifePrefab, CurrentWeapon.shootPoint.position, CurrentWeapon.shootPoint.rotation);
		GameObject ammunitionItem = GameObject.Instantiate(weaponPrefabs[currentWeaponIndex], CurrentWeapon.shootPoint.position, CurrentWeapon.shootPoint.rotation);
		ammunitionItem.GetComponent<ammunitionItem> ().IsItem = true;
		NetworkServer.Spawn(itemLife);
		NetworkServer.Spawn (ammunitionItem);
	}

    [Command]
    void CmdFire()
    {
        //Debug.Log("Shootpoint: " + currentWeapon.shootPoint.position + ", " + currentWeapon.shootPoint.rotation);

		GameObject newBullet = GameObject.Instantiate(bulletPrefab, 
            CurrentWeapon.shootPoint.position, 
            CurrentWeapon.shootPoint.rotation);

        newBullet.GetComponent<Rigidbody>().velocity = newBullet.transform.forward * 10f;

        NetworkServer.Spawn(newBullet);

        Destroy(newBullet, 4.0f);
    }
    
    [Command]
    public void CmdUseObject()
    {
        if (door != null)
            door.CmdSwitchDirection();
    }
}
