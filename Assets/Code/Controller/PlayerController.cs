using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

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
    Count
}


public class PlayerController : NetworkBehaviour {
	#region Index
	public static int throwGrenadeIndex = 0;
	public static int throwExplosiveTrapIndex = 1;
	public static int throwInmovilTrapIndex = 2;
	public static int throwBlindGrenadeIndex = 3;
	public static int blindGrenadeIndex = 4;
	public static int throwBulletIndex = 5;
	#endregion

	#region Movement Attributes
	public float slowerSpeed = 0.0f;
    public float movementSpeed = 5.0f;
	public float runSpeed = 10.0f;
	public float crouchSpeed = 3.0f;
	public float jumpForce = 13.0f;
	public Vector3 gravity = new Vector3(0.0f, -9.81f, 0.0f);
	private float verticalSpeed = 0.0f;
	#endregion

	#region Camera Attributes
    public Camera cam;
	public Transform camFirstPerson;
	public Transform camThirdPerson;
	#endregion

	#region Prefabs
	public GameObject lifePrefab;
	public GameObject explosiveTrap;
	public GameObject inmovilTrap;
    public GameObject bulletPrefab;
	public GameObject blindGrenadePrefab;
    public List<GameObject> weaponPrefabs;
    public GameObject grenadePrefab;
	#endregion

	public Transform weaponPoint;
    public GameObject body;

	private float [] cooldown;
	private MovementStates movementState = MovementStates.Walking;
	private PlayerStates state = PlayerStates.Normal;

    private int currentWeaponIndex = 0;
    private List<GameObject> weapons;
	private float stateTimer;

    private int score;
    public Door door;

    #region Controllers
    private CustomNetworkManager networkManager;
	private int playerId;
	private VehicleController currentVehicle;
	private CharacterController controller;
	private WeaponController weaponController;
	#endregion

	#region Input Attributes
	private float vAxis;
	private float hAxis;
	private bool shiftKey;
	private bool ctrlKey;
	private bool spaceKey;
    private bool tabKey;
	private bool rKey;
    private bool eKey;
	private bool cKey;
	private bool key1;
	private bool key2;
	private bool key3;

	private float mouseX;
	private float mouseY;
	private bool mouseLeft;
	private bool mouseRight;
	#endregion

    #region Properties

    public Transform CurrentWeapon { get { return weapons[currentWeaponIndex].GetComponent<BaseWeapon>().shootPoint; } }
	public PlayerStates State {
        set { state = value; }
        get { return state; }
    }

    public VehicleController CurrentVehicle
    {
        set { currentVehicle = value; }
        get { return currentVehicle; }
    }

	public int Score
	{
		set{ score = value; }
		get{ return score; }
	}
		
    #endregion

	public override void OnStartLocalPlayer()
	{
		base.OnStartLocalPlayer();
		//
		if (!SceneManager.GetActiveScene().name.Equals("SelectorOfMaps"))
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
	}

    // Use this for initialization
    void Start () {
		cam = GetComponentInChildren<Camera> ();
        cam.transform.position = camFirstPerson.position;
		cam.enabled = true;
		controller = GetComponent<CharacterController>();
		weaponController = GetComponent<WeaponController> ();
        InitializeWeapons();
		InitializeCooldowns ();
        //
        networkManager = FindObjectOfType<CustomNetworkManager>();
        //
        if (isServer)
        {
            networkManager.RegisterPlayer(gameObject);
            playerId = networkManager.GetId();
            networkManager.SetColorToPlayers();
        }
    }
	
	// Update is called once per frame
	void Update () {
		// Delta time
		float dt = Time.deltaTime;
		if (isServer) {
			updateInmovil (dt);
			updateCooldown(dt);
		}
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
			checkInputs ();
			UpdateMovement (dt);
        }
	}

	#region Input Functions
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
		cKey = Input.GetKeyDown (KeyCode.C);
		key1 = Input.GetKeyDown(KeyCode.Alpha1);
		key2 = Input.GetKeyDown(KeyCode.Alpha2);
		key3 = Input.GetKeyDown (KeyCode.Alpha3);

		mouseX = Input.GetAxis ("Mouse X");
		mouseY = Input.GetAxis ("Mouse Y");
		mouseLeft = Input.GetMouseButtonDown (0);
		mouseRight = Input.GetMouseButtonDown (1);
	}

	void checkInputs()
	{
		if (rKey) {
			weaponController.CmdReload ();
		}
		if (cKey) {
			if (cam.transform.position == camFirstPerson.position) {
				cam.transform.position = camThirdPerson.position;
			} else {
				cam.transform.position = camFirstPerson.position;
			}

		}
		if (tabKey) {
			CmdChangeWeapon ();
		}
		if (mouseLeft && cooldown [throwBulletIndex] == 0) {
			CmdFire ();
		}
		if (mouseRight && cooldown [throwGrenadeIndex] == 0) { 
			CmdThrowGrenade ();
		}
		if (key1 && cooldown [throwExplosiveTrapIndex] == 0) {
			CmdThrowExplosiveTrap ();
		}
		if (key2 && cooldown [throwInmovilTrapIndex] == 0) {
			CmdThrowInmovilTrap ();
		}
		if (key3 && cooldown [blindGrenadeIndex] == 0) {
			CmdThrowBlindGrenade ();
		}
	}
	#endregion

	#region Movement Functions
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

	void Movement(float dt, float speed)
	{
        switch (state) 
		{
            case PlayerStates.Normal:

                //
                if (controller.isGrounded && spaceKey)
                {
                    Jump();
                }
                
                //
				Vector3 rightMovement = transform.right * hAxis * (speed - slowerSpeed);
				Vector3 forwardMovement = transform.forward * vAxis * (speed - slowerSpeed);
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
                
                CmdDriveVehicle(hAxis, vAxis);
                CmdVehicleOptions(spaceKey, eKey);

                cam.transform.Rotate(mouseY * -90.0f * dt, 0.0f, 0.0f);
                transform.Rotate(0.0f, mouseX * 90.0f * dt, 0.0f);

                break;
            case PlayerStates.InVehicleTurret:
                //
                CmdVehicleOptions(spaceKey, eKey);
                //
                cam.transform.Rotate(mouseY * -90.0f * dt, 0.0f, 0.0f);
                transform.Rotate(0.0f, mouseX * 90.0f * dt, 0.0f);
                break;
            default:
                Debug.Log("Current state: " + state);
                break;
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
    #endregion

    #region Weapon Functions
    private void InitializeWeapons()
    {
        weapons = new List<GameObject>(weaponPrefabs.Count);
        for (int i = 0; i < weaponPrefabs.Count; i++)
        {
            GameObject newWeapon = Instantiate(weaponPrefabs[i], weaponPoint);
            newWeapon.transform.localPosition = Vector3.zero;
            if (i > 0) newWeapon.SetActive(false);
            weapons.Add(newWeapon);
        }
    }

    [Command]
	void CmdChangeWeapon()
	{
        int lastIndex = currentWeaponIndex;
        weapons[currentWeaponIndex].SetActive(false);
        currentWeaponIndex++;
        if (currentWeaponIndex >= weapons.Count) currentWeaponIndex = 0;
        weapons[currentWeaponIndex].SetActive(true);

        weaponController.special = !weaponController.special;
        RpcChangeWeapon(lastIndex, currentWeaponIndex, weaponController.special);
	}

    [ClientRpc]
    private void RpcChangeWeapon(int lastIndex, int newIndex, bool specialState)
    {
        weapons[lastIndex].SetActive(false);
        currentWeaponIndex = newIndex;
        weapons[currentWeaponIndex].SetActive(true);

        weaponController.special = specialState;
    }
    #endregion

    [Command]
	private void CmdCheckAndUse()
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

	#region Punishments
	private void updateInmovil(float dt)
	{
		if (stateTimer > 0) {
			stateTimer -= dt;
		} else if(stateTimer < 0) {
			RpcChangeState(MovementStates.Walking);
			movementState = MovementStates.Walking;
			stateTimer = 0;
		}
	}

	[Command]
	public void CmdChangeState(MovementStates newState, int time)
	{
		movementState = newState;
		stateTimer = time;
		RpcChangeState (newState);
	}

	[ClientRpc]
	private void RpcChangeState(MovementStates newState)
	{
		movementState = newState;
	}

    private void InitializeCooldowns()
    {
        cooldown = new float[6];
        for (int i = 0; i < cooldown.Length; i++)
        {
            cooldown[i] = 0;
        }
    }

    private void updateCooldown(float dt)
	{
		for (int i = 0; i < cooldown.Length; i++) 
		{
			if (cooldown [i] > 0) 
			{
				cooldown [i] -= dt;
			} else if (cooldown [i] < 0) 
			{
				RpcCooldown (i, 0);
				cooldown [i] = 0;
			}
		}
	}
	public float getCooldown(int i)
	{
		return cooldown [i];
	}

	[Command]
	public void CmdCooldown(int timer, int time)
	{
		cooldown [timer] = time;
		RpcCooldown (timer, time);
	}

	[ClientRpc]
	private void RpcCooldown(int timer, int time)
	{
		cooldown [timer] = time;
	}

	[Command]
	public void CmdSlower(float amount)
	{
		slowerSpeed = amount;
		RpcSlower (amount);
	}

	[ClientRpc]
	private void RpcSlower(float amount)
	{
		slowerSpeed = amount;
	}
	#endregion

	#region Commands Throw
	[Command]
	public void CmdThrowItems()
	{
		GameObject itemLife = GameObject.Instantiate(lifePrefab, CurrentWeapon.position, CurrentWeapon.rotation);
		GameObject ammunitionItem = GameObject.Instantiate(weaponPrefabs[currentWeaponIndex], CurrentWeapon.position, CurrentWeapon.rotation);

        AmmunitionItem ammo = ammunitionItem.GetComponent<AmmunitionItem> ();
		ammo.IsItem = true;
		ammo.Bullets = weaponController.CurrentAmmo;
        ammo.Special = weaponController.special;

		NetworkServer.Spawn(itemLife);
		NetworkServer.Spawn (ammunitionItem);

		Destroy (itemLife, Constants.lifeTimeDestroy);
		Destroy (ammunitionItem, Constants.weaponTimeDestroy);
	}

    [Command]
    void CmdFire()
    {
		if (state == PlayerStates.Normal || state == PlayerStates.InVehicleTurret) 
		{
			if (weaponController.CurrentAmmo > 0) {
				GameObject newBullet = GameObject.Instantiate (bulletPrefab, CurrentWeapon.position, CurrentWeapon.rotation);
				newBullet.GetComponent<Rigidbody> ().velocity = newBullet.transform.forward * 10f;
				newBullet.GetComponent<Bullet> ().owner = gameObject;

				weaponController.wasteBullet ();
				NetworkServer.Spawn (newBullet);
				CmdCooldown (throwBulletIndex, Constants.bulletCooldown);
				Destroy (newBullet, Constants.bulletTimeDestroy);
			}
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

		newGrenade.GetComponent<Grenade> ().owner = gameObject;

		NetworkServer.Spawn (newGrenade);

		CmdCooldown (throwGrenadeIndex, Constants.grenadeCooldown);
		Destroy(newGrenade, Constants.grenadeTimeDestroy);
	}

	[Command]
	void CmdThrowBlindGrenade()
	{
		Vector3 grenadePosition = weaponPoint.position + weaponPoint.forward;
		Quaternion grenadeOrientation = weaponPoint.rotation;
		grenadeOrientation *= Quaternion.Euler(-45.0f, 0.0f, 0.0f);
		GameObject newGrenade = Instantiate(blindGrenadePrefab, grenadePosition, grenadeOrientation);
		newGrenade.GetComponent<Rigidbody>().velocity = newGrenade.transform.forward * 20.0f;

		NetworkServer.Spawn (newGrenade);
		CmdCooldown (throwBlindGrenadeIndex, Constants.blindGrenadeCooldown);
		Destroy(newGrenade, Constants.blindGrenadeDuration);
	}
		
	[Command]
	void CmdThrowExplosiveTrap()
	{
		GameObject newExplosiveTrap = GameObject.Instantiate(explosiveTrap, CurrentWeapon.position + transform.forward*5, CurrentWeapon.rotation);

		NetworkServer.Spawn(newExplosiveTrap);

		newExplosiveTrap.GetComponent<ExplosionTrapItem> ().owner = gameObject;

		CmdCooldown (throwExplosiveTrapIndex, 5);
		Destroy(newExplosiveTrap, Constants.trampExplosiveTimeDestroy);
	}

	[Command]
	void CmdThrowInmovilTrap()
	{
		GameObject newInmovilTrap = GameObject.Instantiate(inmovilTrap, CurrentWeapon.position + transform.forward*5, CurrentWeapon.rotation);

		NetworkServer.Spawn(newInmovilTrap);

		CmdCooldown (throwInmovilTrapIndex, Constants.trampInmovilCooldown);
		Destroy(newInmovilTrap, Constants.trampInmovilTimeDestroy);
	}

	#endregion

	#region Elevator

	void OnTriggerStay(Collider other)
	{
		// Chequeo adicional específico para el ascensor
		Elevator ele= other.gameObject.GetComponent<Elevator>();
		if (ele != null) {
			transform.position += ele.PositionOffset;
		}
	}

	#endregion

	#region Vehicle Functions
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
        GetComponent<CharacterController>().detectCollisions = false;
        // NetworkIdentity playerIdentity = player.GetComponent<NetworkIdentity>();
        // playerIdentity.localPlayerAuthority = false;
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
    void CmdDriveVehicle(float hAxis, float vAxis)
    {
        Debug.Log("In vehicle driving by command");
        
        currentVehicle.CmdMove(new Vector2(hAxis, vAxis));
        
    }

    [Command]
    void CmdVehicleOptions(bool spaceKey, bool eKey)
    {
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
	#endregion

    #region Color Functions
    public Color DecideColor(int colorIndex)
    {
        switch (colorIndex)
        {
            case 0:
                return Color.cyan;
            case 1:
                return Color.red;
            case 2:
                return Color.blue;
            case 3:
                return Color.green;
            case 4:
                return Color.yellow;
            case 5:
                return Color.magenta;
            case 6:
                return Color.white;
            case 7:
                return Color.grey;
            default:
                return Color.black;
        }
    }

    [ClientRpc]
    public void RpcChangeColor(int colorIndex)
    {
        MeshRenderer meshRenderer = body.GetComponent<MeshRenderer>();
        meshRenderer.material.color = DecideColor(colorIndex);
    }
    #endregion
}
