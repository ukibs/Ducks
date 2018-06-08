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
	private float slowerSpeed = 0.0f;
    private float movementSpeed = 5.0f;
	private float runSpeed = 10.0f;
	private float crouchSpeed = 3.0f;
	private float jumpForce = 5.0f;
	private Vector3 gravity = new Vector3(0.0f, -9.81f, 0.0f);
	private float verticalSpeed = 0.0f;
	#endregion

	#region Camera Attributes
    private Camera cam;
	public Transform camFirstPerson;
	public Transform camThirdPerson;
	#endregion

	#region Prefabs
	private GameObject lifePrefab;
    private GameObject explosiveTrap;
    private GameObject inmovilTrap;
    private GameObject blindGrenadePrefab;
    private GameObject grenadePrefab;
	#endregion

	public Transform weaponPoint;
    public Transform rotationPoint;
    public GameObject body;

	private float [] cooldown;
	private MovementStates movementState = MovementStates.Walking;
	private PlayerStates state = PlayerStates.Normal;
    
	private float stateTimer;
    
    private Effects effectManager;

    private bool collObject = false;

    #region Controllers
    private CustomNetworkManager networkManager;
	private VehicleController currentVehicle;
	private CharacterController controller;
    private WeaponController2 weaponController;
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
    private bool quitKey;

	private float mouseX;
	private float mouseY;
	private bool mouseLeft;
	private bool mouseRight;
	#endregion

    #region Properties
	public PlayerStates State {
        set { state = value; }
        get { return state; }
    }

    public VehicleController CurrentVehicle
    {
        set { currentVehicle = value; }
        get { return currentVehicle; }
    }

    public bool CollisionObject
    {
        set { collObject = value; }
        get { return collObject; }
    }

    #endregion

    #region Unity Methods
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
        lifePrefab = Resources.Load("Prefabs/LifeItem") as GameObject;
        explosiveTrap = Resources.Load("Prefabs/Weapons/ExplosiveTrap") as GameObject;
        inmovilTrap = Resources.Load("Prefabs/Weapons/InmovilTrap") as GameObject;
        blindGrenadePrefab = Resources.Load("Prefabs/Weapons/blindGrenade") as GameObject;
        grenadePrefab = Resources.Load("Prefabs/Weapons/Grenade") as GameObject;

    cam = GetComponentInChildren<Camera> ();
        cam.transform.position = camFirstPerson.position;
		cam.enabled = true;
		controller = GetComponent<CharacterController>();
        weaponController = GetComponent<WeaponController2>();
		InitializeCooldowns ();
        //
        networkManager = FindObjectOfType<CustomNetworkManager>();
        //
        if (isServer)
        {
            networkManager.RegisterPlayer(gameObject);
            networkManager.SetColorToPlayers();
        }
        
        //
        effectManager = FindObjectOfType<Effects>();
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
            AudioListener listener = cam.GetComponent<AudioListener>();
            if (listener.enabled) listener.enabled = false;
            cam.enabled = false;
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
    #endregion

    #region Input Functions
    void UpdateInput()
	{
        if (isLocalPlayer)
        {
            vAxis = Input.GetAxis("Vertical");
            hAxis = Input.GetAxis("Horizontal");
            shiftKey = Input.GetKeyDown(KeyCode.LeftShift);
            ctrlKey = Input.GetKeyDown(KeyCode.LeftControl);
            spaceKey = Input.GetKeyDown(KeyCode.Space);
            tabKey = Input.GetKeyDown(KeyCode.Tab);
            rKey = Input.GetKeyDown(KeyCode.R);
            eKey = Input.GetKeyDown(KeyCode.E);
            cKey = Input.GetKeyDown(KeyCode.C);
            key1 = Input.GetKeyDown(KeyCode.Alpha1);
            key2 = Input.GetKeyDown(KeyCode.Alpha2);
            key3 = Input.GetKeyDown(KeyCode.Alpha3);
            quitKey = Input.GetKeyDown(KeyCode.P);

            mouseX = Input.GetAxis("Mouse X");
            mouseY = Input.GetAxis("Mouse Y");
            mouseLeft = Input.GetMouseButton(0);
            mouseRight = Input.GetMouseButtonDown(1);
        }
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
			weaponController.CmdChangeWeapon ();
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
        //
        if(quitKey && isServer)
        {
            SceneManager.LoadScene("SelectorOfMaps");
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
                
                //Movement
				Vector3 rightMovement = transform.right * hAxis * (speed - slowerSpeed);
				Vector3 forwardMovement = transform.forward * vAxis * (speed - slowerSpeed);
                Vector3 yMovement = transform.up * verticalSpeed;
                controller.Move((rightMovement + forwardMovement + yMovement) * dt);
                //Rotation
                Vector3 aux = rotationPoint.transform.localEulerAngles;
                float change = mouseY * -90.0f;
                transform.Rotate(0.0f, mouseX * 90.0f * dt, 0.0f);
                aux += new Vector3(change * dt, 0, 0);
                if (aux.x < 60 || aux.x > 300)
                {
                    rotationPoint.transform.Rotate(change * dt, 0.0f, 0.0f);
                    CmdRotateWeapon(rotationPoint.rotation);
                }
                // 
                if (eKey)
                {
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
                CmdUseTurret(cam.transform.rotation, mouseLeft);
                //
                cam.transform.Rotate(mouseY * -90.0f * dt, 0.0f, 0.0f);
                transform.Rotate(0.0f, mouseX * 90.0f * dt, 0.0f);
                break;
            default:
                break;
        }
	}

    [Command]
    private void CmdRotateWeapon(Quaternion rotation)
    {
        rotationPoint.rotation = rotation;
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

    [Command]
	private void CmdCheckAndUse()
	{
		RaycastHit hit;
		if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 2.0f))
		{
			BaseUsable usable = hit.transform.GetComponent<BaseUsable>();
			if (usable != null)
			{
				usable.SendMessage("CmdUse");
			}

			VehicleController vehicleController = hit.transform.GetComponent<VehicleController>();
			if (vehicleController != null)
			{

				vehicleController.CmdEnterVehicle(gameObject);
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
	public void CmdCooldown(int timer, float time)
	{
		cooldown [timer] = time;
		RpcCooldown (timer, time);
	}

	[ClientRpc]
	private void RpcCooldown(int timer, float time)
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
		GameObject itemLife = Instantiate(lifePrefab, weaponController.CurrentWeapon.position, weaponController.CurrentWeapon.rotation);
		GameObject ammunitionItem = Instantiate(weaponController.CurrentPrefab, weaponController.CurrentWeapon.position, weaponController.CurrentWeapon.rotation);

        AmmunitionItem ammo = ammunitionItem.GetComponent<AmmunitionItem> ();
		ammo.IsItem = true;
		ammo.Bullets = weaponController.CurrentAmmo;
        ammo.Special = weaponController.WeaponIndex;

		NetworkServer.Spawn(itemLife);
		NetworkServer.Spawn (ammunitionItem);

		Destroy (itemLife, Constants.lifeTimeDestroy);
		Destroy (ammunitionItem, Constants.weaponTimeDestroy);
	}

    [Command]
    private void CmdFire()
    {
		if (state == PlayerStates.Normal) 
		{
			if (weaponController.CurrentAmmo > 0) {
                // First, determine the point in front of the player size
                Ray ray = cam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
                RaycastHit hit;
                Vector3 shootPoint = Vector3.zero;
                if (cam.transform.position != camThirdPerson.position)
                {
                    if (Physics.Raycast(ray, out hit))
                    {
                        shootPoint = hit.point;
                    }
                }
                //
                weaponController.CmdShoot(shootPoint);
				CmdCooldown (throwBulletIndex, Constants.bulletCooldown);

                effectManager.playEffect(0);
                RpcShootSound();
			}
		}
    }

    [ClientRpc]
    void RpcShootSound()
    {
        effectManager.playEffect(0);
    }

	[Command]
    private void CmdThrowGrenade()
	{
		Vector3 grenadePosition = weaponPoint.position + weaponPoint.forward;
		Quaternion grenadeOrientation = weaponPoint.rotation;
		grenadeOrientation *= Quaternion.Euler(-30.0f, 0.0f, 0.0f);
		GameObject newGrenade = Instantiate(grenadePrefab, grenadePosition, grenadeOrientation);
		newGrenade.GetComponent<Rigidbody>().velocity = newGrenade.transform.forward * 20.0f;

		newGrenade.GetComponent<Grenade> ().owner = gameObject;

		NetworkServer.Spawn (newGrenade);

		CmdCooldown (throwGrenadeIndex, Constants.grenadeCooldown);
		Destroy(newGrenade, Constants.grenadeTimeDestroy);
	}

	[Command]
    private void CmdThrowBlindGrenade()
	{
		Vector3 grenadePosition = weaponPoint.position + weaponPoint.forward;
		Quaternion grenadeOrientation = weaponPoint.rotation;
		grenadeOrientation *= Quaternion.Euler(-30.0f, 0.0f, 0.0f);
		GameObject newGrenade = Instantiate(blindGrenadePrefab, grenadePosition, grenadeOrientation);
		newGrenade.GetComponent<Rigidbody>().velocity = newGrenade.transform.forward * 20.0f;

		NetworkServer.Spawn (newGrenade);
		CmdCooldown (throwBlindGrenadeIndex, Constants.blindGrenadeCooldown);
		Destroy(newGrenade, Constants.blindGrenadeDuration);
	}
		
	[Command]
    private void CmdThrowExplosiveTrap()
	{
        GameObject newExplosiveTrap = Instantiate(explosiveTrap, weaponController.CurrentWeapon.position + transform.forward*5, weaponController.CurrentWeapon.rotation);

		newExplosiveTrap.GetComponent<ExplosionTrapItem> ().owner = gameObject;

        NetworkServer.Spawn(newExplosiveTrap);

        CmdCooldown(throwExplosiveTrapIndex, 5);
		Destroy(newExplosiveTrap, Constants.trampExplosiveTimeDestroy);
	}

	[Command]
    private void CmdThrowInmovilTrap()
	{
		GameObject newInmovilTrap = Instantiate(inmovilTrap, weaponController.CurrentWeapon.position + transform.forward*5, weaponController.CurrentWeapon.rotation);

		NetworkServer.Spawn(newInmovilTrap);

		CmdCooldown (throwInmovilTrapIndex, Constants.trampInmovilCooldown);
		Destroy(newInmovilTrap, Constants.trampInmovilTimeDestroy);
	}

    #endregion

    #region Elevator and Door

    private void OnTriggerStay(Collider other)
    {
        // Chequeo adicional específico para el ascensor
        Elevator ele = other.gameObject.GetComponent<Elevator>();
        if (ele != null)
        {
            transform.position += ele.PositionOffset;
        }
    }

    [Command]
    public void CmdCollisionObject(bool state)
    {
        collObject = state;
        RpcCollisionObject(state);
    }

    [ClientRpc]
    public void RpcCollisionObject(bool state)
    {
        collObject = state;
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
    }

    [ClientRpc]
    public void RpcSwitchVehiclePosition()
    {
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
        state = PlayerStates.Normal;
        currentVehicle = null;
        transform.position += Vector3.up;
        transform.SetParent(null);
        GetComponent<CharacterController>().detectCollisions = true;
    }

    [Command]
    private void CmdDriveVehicle(float hAxis, float vAxis)
    {
        currentVehicle.CmdMove(new Vector2(hAxis, vAxis));
    }

    [Command]
    private void CmdVehicleOptions(bool spaceKey, bool eKey)
    {
        if (spaceKey)
        {
            currentVehicle.CmdSwitchPlace(gameObject);
        }
        if (eKey)
        {
            currentVehicle.CmdQuitVehicle(gameObject);
        }
    }

    [Command]
    private void CmdUseTurret(Quaternion weaponPointRotation, bool leftMouse)
    {
        currentVehicle.CmdUseTurret(weaponPointRotation, leftMouse);
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
