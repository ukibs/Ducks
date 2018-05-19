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

    private PlayerController driver;
    private PlayerController turretGuy;

    private Vector2 controlAxis;

	// Use this for initialization
	void Start () {
        controlAxis = Vector2.zero;
	}
	
	// Update is called once per frame
	void Update () {
        /*if (isServer)
        {
            transform.Translate(Vector3.forward * controlAxis.y * 20.0f * Time.deltaTime);
            transform.Rotate(transform.up * controlAxis.x * 90.0f * Time.deltaTime);
        }*/
            //RpcMove();
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
        switch (vehiclePlace)
        {
            case VehiclePlace.Driver:
                driver = playerController;
                break;
            case VehiclePlace.Gunner:
                turretGuy = playerController;
                break;
        }
        
        playerController.RpcEnterVehicle(gameObject, vehiclePlace);
        
        
        //
        // NetworkIdentity playerIdentity = player.GetComponent<NetworkIdentity>();
        // playerIdentity.localPlayerAuthority = false;
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
}
