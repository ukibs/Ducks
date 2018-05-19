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

public enum PlayerStates
{
    Invalid = -1,
    Normal,
    InVehicleDriving,
    InVehicleTurret,
    Trapped,
    Count
}

// TODO: Trabajar cegado

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
	private PlayerStates state = PlayerStates.Normal;
	private CharacterController controller;
	private float verticalSpeed = 0.0f;
    // private BaseWeapon currentWeapon = null;
    private int currentWeaponIndex = 0;
    private List<GameObject> weapons;
    private VehicleController currentVehicle;


	private float vAxis;
	private float hAxis;
	private bool shiftKey;
	private bool ctrlKey;
	private bool spaceKey;
    private bool tabKey;
	private bool rKey;
    private bool eKey;

	private float mouseX;
	private float mouseY;
	// private bool mouseLeft;
	private bool mouseRight;

    // Variable de prueba
    public Door door;

    #region Properties

    public BaseWeapon CurrentWeapon { get { return weapons[currentWeaponIndex].GetComponent<BaseWeapon>(); } }
	public PlayerStates State {
        set { state = value; }
        get { return state; }
    }
    public VehicleController CurrentVehicle { set { currentVehicle = value; } }

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
			
			if (rKey)
				CurrentWeapon.Reload ();
            if (tabKey) ChangeWeapon();
			if (mouseRight) CmdThrowGrenade();
			SimpleShoot (dt);
			UpdateMovement (dt);
            
        }
	}

    private void OnGUI()
    {
		if (isLocalPlayer) {
            // Weapon info
			BaseWeapon weaponData = weapons [currentWeaponIndex].GetComponent<BaseWeapon> ();
			GUI.Label (new Rect (Screen.width * 9 / 10, Screen.height * 8.8f / 10, 100, 20), weaponData.CurrentWeaponAmmo + "/" + weaponData.maxWeaponAmmo);
			GUI.Label (new Rect (Screen.width * 9 / 10, Screen.height * 9 / 10, 100, 20), weaponData.CurrentReserveAmmo + "/" + weaponData.maxReserveAmmo);
            //
            GUI.Label(new Rect(10, 10, 350, 20), "State: " + state + ", movement state: " + movementState);

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
        eKey = Input.GetKeyDown(KeyCode.E);

		mouseX = Input.GetAxis ("Mouse X");
		mouseY = Input.GetAxis ("Mouse Y");
		//mouseLeft = Input.GetMouseButtonDown (0);
		mouseRight = Input.GetMouseButtonDown (1);
	}



	void Movement(float dt, float speed)
	{
        // TODO: Preguntar a Nestor que coño apsa con esto
        //Debug.Log("Applying movement");
        switch (state) {
            case PlayerStates.Normal:

                //
                //if (spaceKey) Debug.Log("Controller: " + controller.isGrounded);
                if (controller.isGrounded && spaceKey)
                {
                    Jump();
                }
                
                //
                Vector3 rightMovement = transform.right * hAxis * speed;
                Vector3 forwardMovement = transform.forward * vAxis * speed;
                Vector3 yMovement = transform.up * verticalSpeed;
                controller.Move((rightMovement + forwardMovement + yMovement) * dt);
                transform.Rotate(0.0f, mouseX * 90.0f * dt, 0.0f);
                cam.transform.Rotate(mouseY * -90.0f * dt, 0.0f, 0.0f);
                // 
                if (eKey)
                {
                    //CmdUseObject();
                    CmdCheckAndUse();
                }
                break;
            case PlayerStates.InVehicleDriving:
                
                CmdDriveVehicle(dt);
                
                break;
            case PlayerStates.InVehicleTurret:
                cam.transform.Rotate(mouseY * -90.0f * dt, 0.0f, 0.0f);
                transform.Rotate(0.0f, mouseX * 90.0f * dt, 0.0f);
                if (spaceKey) currentVehicle.CmdSwitchPlace(gameObject);
                if (eKey) currentVehicle.CmdQuitVehicle(gameObject);
                break;
            default:
                Debug.Log("Current state: " + state);
                break;
        }
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
        if (state == PlayerStates.Normal || state == PlayerStates.InVehicleTurret)
        {
            if (CurrentWeapon.OrderFire()) CmdFire();
        }
    }

	void ApplyGravity(float dt)
	{
        if (state == PlayerStates.Normal)
        {
            if (controller.isGrounded)
                verticalSpeed = 0.0f;
            else
                verticalSpeed += gravity.y * dt;
        }
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
    void CmdCheckAndUse()
    {
        //Debug.Log("Checking with ray");
        RaycastHit hit;
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 2.0f))
        {

            //Debug.Log("Checking object to use: " + hit.transform.name);
            BaseUsable usable = hit.transform.GetComponent<BaseUsable>();
            if (usable != null)
            {
                //TODO: Preguntar a Nestor sobre esto

                //usable.CmdUse();
                usable.SendMessage("CmdUse");
                //hit.transform.SendMessage("CmdUse");
                //CmdUseObject(usable);
                Debug.Log("Using object " + usable);
                //return;
            }

            VehicleController vehicleController = hit.transform.GetComponent<VehicleController>();
            //Debug.Log("Is it a vehicle? " + vehicleController != null);
            if (vehicleController != null)
            {
                
                vehicleController.CmdUse(gameObject);
            }
        }
    }

	[Command]
	public void CmdThrowItems()
	{
		GameObject itemLife = GameObject.Instantiate(lifePrefab, CurrentWeapon.shootPoint.position, CurrentWeapon.shootPoint.rotation);
		GameObject ammunitionItem = GameObject.Instantiate(weaponPrefabs[currentWeaponIndex], CurrentWeapon.shootPoint.position, CurrentWeapon.shootPoint.rotation);

        //ammunitionItem.GetComponent<ammunitionItem> ().Bullets = CurrentWeapon.CurrentWeaponAmmo;
        AmmunitionItem ammo = ammunitionItem.GetComponent<AmmunitionItem> ();
		ammo.IsItem = true;
		ammo.Bullets = CurrentWeapon.CurrentWeaponAmmo;

		//Debug.Log(ammo.IsItem);
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
    
    /*[Command]
    public void CmdUseObject(BaseUsable usable)
    {
        usable.CmdUse();
        //if (door != null)
        //    door.CmdSwitchDirection();
    }*/

	public void takeWeapon(BaseWeapon weapon, int bullets)
	{
		for (int i = 0; i < weapons.Count; i++) 
		{
			if (weapons [i].tag.Equals (weapon.tag)) 
			{
				weapons [i].GetComponent<BaseWeapon> ().addAmmo (bullets);
			}
		}
	}

    [ClientRpc]
    public void RpcEnterVehicle(GameObject vehicle, VehiclePlace vehiclePlace)
    {
        
        VehicleController vehicleController = vehicle.GetComponent<VehicleController>();
        //
        switch (vehiclePlace)
        {
            case VehiclePlace.Driver:
                transform.position = vehicleController.driverPlace.position;
                state = PlayerStates.InVehicleDriving;
                break;
            case VehiclePlace.Gunner:
                transform.position = vehicleController.turretGuyPlace.position;
                state = PlayerStates.InVehicleTurret;
                break;
        }
        //
        currentVehicle = vehicleController;
        transform.SetParent(vehicle.transform);
        //
        Debug.Log("Entering vehicle, state: " + state + ", vehicle: " + currentVehicle);
    }

    [ClientRpc]
    public void RpcSwitchVehiclePosition()
    {
        Debug.Log("Switching place");
        switch (state)
        {
            case PlayerStates.InVehicleDriving:
                transform.position = currentVehicle.turretGuyPlace.position;
                state = PlayerStates.InVehicleTurret;
                break;
            case PlayerStates.InVehicleTurret:
                transform.position = currentVehicle.driverPlace.position;
                state = PlayerStates.InVehicleDriving;
                break;
        }
    }

    [ClientRpc]
    public void RpcQuitVehicle()
    {
        Debug.Log("Quitting vehicle");
        state = PlayerStates.Normal;
        currentVehicle = null;
        transform.position += Vector3.up;
        transform.SetParent(null);
    }

    [Command]
    void CmdDriveVehicle(float dt)
    {
        Debug.Log("In vehicle driving by command");
        currentVehicle.CmdMove(new Vector2(hAxis, vAxis));
        cam.transform.Rotate(mouseY * -90.0f * dt, 0.0f, 0.0f);
        transform.Rotate(0.0f, mouseX * 90.0f * dt, 0.0f);
        if (spaceKey)
        {
            Debug.Log("Trying to switch place");
            currentVehicle.CmdSwitchPlace(gameObject);
        }
        if (eKey)
        {
            Debug.Log("Trying to quit vehicle");
            currentVehicle.CmdQuitVehicle(gameObject);
        }
    }

}
