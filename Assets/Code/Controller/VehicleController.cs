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

public class VehicleController : NetworkBehaviour {

    public Transform driverPlace;
    public Transform turretGuyPlace;
    public GameObject turretWeapon;
    public GameObject turretBulletPrefab;

    private PlayerController driver;
    private PlayerController turretGuy;

    private Vector2 controlAxis;
    private Rigidbody rb;
    private Effects effectManager;
    private float turretWeaponFireCount;
    private Transform turretFirePoint;

	// Use this for initialization
	void Start () {
        controlAxis = Vector2.zero;
        rb = GetComponent<Rigidbody>();
        effectManager = FindObjectOfType<Effects>();
        turretFirePoint = turretWeapon.transform.GetChild(0);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (isServer)
        {
            //transform.Translate(Vector3.forward * controlAxis.y * 20.0f * Time.deltaTime);
            rb.velocity = transform.forward * controlAxis.y * 20.0f * Time.deltaTime;
            transform.Rotate(transform.up * controlAxis.x * 90.0f * Time.deltaTime);
            rb.angularVelocity = Vector3.zero;
            if(turretGuy != null && turretWeaponFireCount < Constants.bulletCooldown)
            {
                turretWeaponFireCount += Time.deltaTime;
            }
        }
            //RpcMove();
    }

    private void OnGUI()
    {
        //
        //if(driver != null)
           // GUI.Label(new Rect(10, 30, 350, 20), "Driver: " + driver + ", axis: " + controlAxis);
    }

    void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Vehicle colliding with " + collision.gameObject.name);
        //
        float vehicleVelocity = rb.velocity.magnitude;
        PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
        //
        //if (/*vehicleVelocity <= 3 || */playerController == driver || playerController == turretGuy)
        //    return;
        //
        HealthController healthController = collision.gameObject.GetComponent<HealthController>();
        //Debug.Log("Collided has HealthController " + healthController != null);
        if (healthController != null)
        {
            Debug.Log("Hitting " + collision.gameObject + " with " + rb.mass + " mass & " + vehicleVelocity + " speed");
            healthController.TakeDamage((int)(rb.mass * vehicleVelocity), driver.gameObject);
        }
    }

    [ClientRpc]
    public void RpcMove(Vector2 controlAxis)
    {
        transform.Translate(Vector3.forward * controlAxis.y * 20.0f * Time.deltaTime);
        transform.Rotate(transform.up * controlAxis.x * 90.0f * Time.deltaTime);
    }

    [Command]
    public void CmdMove(Vector2 controlAxis)
    {
        this.controlAxis = controlAxis;
        Debug.Log("Receiving axis");
        RpcMove(controlAxis);
        //transform.Translate(Vector3.forward * controlAxis.y * 20.0f * Time.deltaTime);
        //transform.Rotate(transform.up * controlAxis.x * 90.0f * Time.deltaTime);
    }

    [Command]
    public void CmdUse(GameObject player)
    {
        //Debug.Log("Trying to use");
        // Check that there isn't currently a driver
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

    [Command]
    public void CmdUseTurret(Quaternion weaponPointRotation, bool leftMouse)
    {
        turretWeapon.transform.rotation = weaponPointRotation * Quaternion.AngleAxis(90, Vector3.right);
        if (leftMouse && turretWeaponFireCount >= Constants.bulletCooldown)
        {
            CmdFire();
            turretWeaponFireCount = 0;
        }
    }

    [Command]
    void CmdFire()
    {
        GameObject newBullet = Instantiate(turretBulletPrefab, 
            turretFirePoint.position + turretFirePoint.forward, 
            turretFirePoint.rotation);
        Rigidbody bulletRB = newBullet.GetComponent<Rigidbody>();
        bulletRB.velocity = newBullet.transform.forward * 20;
        Debug.Log("Created " + newBullet);

        effectManager.playEffect(0);
        RpcShootSound();
    }

    [ClientRpc]
    void RpcShootSound()
    {
        effectManager.playEffect(0);
    }
}
