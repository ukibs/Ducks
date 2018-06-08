using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public enum VehiclePlace
{
    Driver,
    Gunner,

    Count
}

public class VehicleController : BaseUsable {

    #region Public Attributes

    public Transform driverPlace;
    public Transform turretGuyPlace;
    public GameObject turretWeapon;
    public GameObject turretBulletPrefab;

    #endregion

    #region Private Attributes

    private PlayerController driver;
    private PlayerController turretGuy;

    private Vector2 controlAxis;
    private Rigidbody rb;
    private Effects effectManager;
    private float turretWeaponFireCount;
    private Transform turretFirePoint;

    #endregion

    #region Monobehaviour Methods

    // Use this for initialization
    void Start () {
        if (isServer)
        {
            controlAxis = Vector2.zero;
            rb = GetComponent<Rigidbody>();
            effectManager = FindObjectOfType<Effects>();
            turretFirePoint = turretWeapon.transform.GetChild(0);
        }
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (isServer)
        {
            //transform.Translate(Vector3.forward * controlAxis.y * 20.0f * Time.deltaTime);
            rb.velocity = transform.forward * controlAxis.y * 500.0f * Time.deltaTime;
            transform.Rotate(transform.up * controlAxis.x * 90.0f * Time.deltaTime);
            rb.angularVelocity = Vector3.zero;
            if(turretGuy != null && turretWeaponFireCount < Constants.bulletCooldown)
            {
                turretWeaponFireCount += Time.deltaTime;
            }
        }
    }
    

    void OnCollisionEnter(Collision collision)
    {
        // Null reference control
        if (collision == null) return;
        //
        if (isServer)
        {
            float vehicleVelocity = rb.velocity.magnitude;
            //PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();

            HealthController healthController = collision.gameObject.GetComponent<HealthController>();

            if (healthController != null)
            {
                // Debug.Log("Hitting " + collision.gameObject + " with " + rb.mass + " mass & " + vehicleVelocity + " speed");
                healthController.TakeDamage((int)(rb.mass * vehicleVelocity), driver.gameObject);
            }
        }
    }

    #endregion

    #region Driver Methods
    [Command]
    public void CmdMove(Vector2 controlAxis)
    {
        this.controlAxis = controlAxis;
    }

    #endregion

    #region Place Management Methods

    [Command]
    public void CmdEnterVehicle(GameObject player)
    {
        // Check if there isn't currently a driver or a turret guy
        if (driver == null)
        {
            AssignPosition(player, VehiclePlace.Driver);
        }
        else if(turretGuy == null)
        {
            AssignPosition(player, VehiclePlace.Gunner);
        }
    }


    void AssignPosition(GameObject player, VehiclePlace vehiclePlace)
    {
        //
        PlayerController playerController = player.GetComponent<PlayerController>();

        playerController.RpcEnterVehicle(gameObject, vehiclePlace);

        switch (vehiclePlace)
        {
            case VehiclePlace.Driver:
                driver = playerController;
                break;
            case VehiclePlace.Gunner:
                turretGuy = playerController;
                break;
        }
        
    }

    [Command]
    public void CmdSwitchPlace(GameObject player)
    {
        PlayerController playerController = player.GetComponent<PlayerController>();
        
        switch (playerController.State)
        {
            case PlayerStates.InVehicleDriving:
                // First check the other place is free
                if (turretGuy != null) return;
                playerController.RpcSwitchVehiclePosition();
                turretGuy = playerController;
                driver = null;
                break;
            case PlayerStates.InVehicleTurret:
                //
                if (driver != null) return;
                playerController.RpcSwitchVehiclePosition();
                driver = playerController;
                turretGuy = null;
                break;
        }
    }

    [Command]
    public void CmdQuitVehicle(GameObject player)
    {
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController.State == PlayerStates.InVehicleDriving) driver = null;
        else if (playerController.State == PlayerStates.InVehicleTurret) turretGuy = null;
        playerController.RpcQuitVehicle();
        
    }

    #endregion

    #region Turret Methods

    [Command]
    public void CmdUseTurret(Quaternion weaponPointRotation, bool leftMouse)
    {
        turretWeapon.transform.rotation = weaponPointRotation * Quaternion.AngleAxis(90, Vector3.right);
        RpcMoveTurret(weaponPointRotation);
        if (leftMouse && turretWeaponFireCount >= Constants.bulletCooldown)
        {
            CmdFire();
            turretWeaponFireCount = 0;
            
        }
    }

    [ClientRpc]
    public void RpcMoveTurret(Quaternion weaponPointRotation)
    {
        turretWeapon.transform.rotation = weaponPointRotation * Quaternion.AngleAxis(90, Vector3.right);
    }

    [Command]
    void CmdFire()
    {
        GameObject newBullet = Instantiate(turretBulletPrefab, 
            turretFirePoint.position + turretFirePoint.forward, 
            turretFirePoint.rotation);
        Rigidbody bulletRB = newBullet.GetComponent<Rigidbody>();
        bulletRB.velocity = newBullet.transform.forward * 20;
        newBullet.GetComponent<Bullet>().owner = turretGuy.gameObject;
        NetworkServer.Spawn(newBullet);
        effectManager.playEffect(0);
        RpcShootSound();
    }

    [ClientRpc]
    void RpcShootSound()
    {
        effectManager.playEffect(0);
    }

    #endregion

    [Command]
    public override void CmdUse() { }
}
