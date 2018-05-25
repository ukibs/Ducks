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
	public static int throwGrenadeIndex = 0;
	public static int throwExplosiveTrapIndex = 1;
	public static int throwInmovilTrapIndex = 2;
	public static int throwBlindGrenadeIndex = 3;
	public static int blindGrenadeIndex = 4;

	public float slowerSpeed = 0.0f;
    public float movementSpeed = 5.0f;
	public float runSpeed = 10.0f;
	public float crouchSpeed = 3.0f;
	public float jumpForce = 13.0f;

	//Camera
    public Camera cam;
	public Transform camFirstPerson;
	public Transform camThirdPerson;


	public GameObject lifePrefab;
	public GameObject explosiveTrap;
	public GameObject inmovilTrap;
    public GameObject bulletPrefab;
	public GameObject blindGrenadePrefab;
    public GameObject myPrefab;
    public List<GameObject> weaponPrefabs;
    public GameObject grenadePrefab;
	public Transform weaponPoint;
	public Vector3 gravity = new Vector3(0.0f, -9.81f, 0.0f);

	private float [] cooldown;
	private MovementStates movementState = MovementStates.Walking;
	private PlayerStates state = PlayerStates.Normal;
	private CharacterController controller;
	private WeaponController weaponController;
	private float verticalSpeed = 0.0f;
    private int currentWeaponIndex = 0;
    private List<GameObject> weapons;
    private VehicleController currentVehicle;
	private float stateTimer;
    private CustomNetworkManager networkManager;
    private int playerId;


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

    // Variable de prueba
    public Door door;

	private int score;

    #region Properties

    public BaseWeapon CurrentWeapon { get { return weapons[currentWeaponIndex].GetComponent<BaseWeapon>(); } }
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

    // Use this for initialization
    void Start () {
		cam = GetComponentInChildren<Camera> ();
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
			if (stateTimer > 0) {
				stateTimer -= dt;
			} else if(stateTimer < 0) {
				RpcChangeState(MovementStates.Walking);
				movementState = MovementStates.Walking;
				stateTimer = 0;
			}

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
				ChangeWeapon ();
			}
			if (mouseLeft)
				SimpleShoot (dt);
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

			UpdateMovement (dt);
        }
	}

	public float getCooldown(int i)
	{
		return cooldown [i];
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
		cKey = Input.GetKeyDown (KeyCode.C);
		key1 = Input.GetKeyDown(KeyCode.Alpha1);
		key2 = Input.GetKeyDown(KeyCode.Alpha2);
		key3 = Input.GetKeyDown (KeyCode.Alpha3);

		mouseX = Input.GetAxis ("Mouse X");
		mouseY = Input.GetAxis ("Mouse Y");
		mouseLeft = Input.GetMouseButtonDown (0);
		mouseRight = Input.GetMouseButtonDown (1);
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
		
    void ChangeWeapon()
    {
        weapons[currentWeaponIndex].SetActive(false);
        currentWeaponIndex++;
        if (currentWeaponIndex >= weapons.Count) currentWeaponIndex = 0;
        weapons[currentWeaponIndex].SetActive(true);

		CmdChange ();
    }

	[Command]
	void CmdChange()
	{
		weaponController.special = !weaponController.special;
	}

    void InitializeWeapons()
    {
        weapons = new List<GameObject>(weaponPrefabs.Count);
        for(int i = 0; i < weaponPrefabs.Count; i++)
        {
            GameObject newWeapon = Instantiate(weaponPrefabs[i], weaponPoint);
            newWeapon.transform.localPosition = Vector3.zero;
            if(i > 0) newWeapon.SetActive(false);
            weapons.Add(newWeapon);
        }
    }

	void InitializeCooldowns()
	{
		cooldown = new float[5];
		for (int i = 0; i < cooldown.Length; i++) 
		{
			cooldown [i] = 0;
		}
	}
		
	void SimpleShoot(float dt)
	{
        if (state == PlayerStates.Normal || state == PlayerStates.InVehicleTurret)
        {
           	CmdFire();
			//weaponController.wasteBullet ();
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
        //
        if (!SceneManager.GetActiveScene().name.Equals("SelectorOfMaps"))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
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

	#region Commands Throw

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
		newBullet.GetComponent<Bullet> ().owner = gameObject;

		weaponController.wasteBullet ();
        NetworkServer.Spawn(newBullet);

        Destroy(newBullet, 4.0f);
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

		CmdCooldown (throwGrenadeIndex, 5);
		//Destroy(newGrenade, 4);
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
		CmdCooldown (throwBlindGrenadeIndex, 5);
		//Destroy(newGrenade, 4);
	}

	[Command]
	void CmdThrowExplosiveTrap()
	{
		GameObject newExplosiveTrap = GameObject.Instantiate(explosiveTrap, CurrentWeapon.shootPoint.position + transform.forward*5, CurrentWeapon.shootPoint.rotation);

		NetworkServer.Spawn(newExplosiveTrap);

		newExplosiveTrap.GetComponent<ExplosionTrapItem> ().owner = gameObject;

		CmdCooldown (throwExplosiveTrapIndex, 5);
		//Destroy(newBullet, 4.0f);
	}

	[Command]
	void CmdThrowInmovilTrap()
	{
		GameObject newInmovilTrap = GameObject.Instantiate(inmovilTrap, CurrentWeapon.shootPoint.position + transform.forward*5, CurrentWeapon.shootPoint.rotation);

		NetworkServer.Spawn(newInmovilTrap);

		CmdCooldown (throwInmovilTrapIndex, 5);
		//Destroy(newBullet, 4.0f);
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
        MeshRenderer meshRenderer = this.gameObject.GetComponent<MeshRenderer>();
        meshRenderer.material.color = DecideColor(colorIndex);
    }

    #endregion
}
