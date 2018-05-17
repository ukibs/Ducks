using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class VehicleController : NetworkBehaviour {

    public Transform driverPlace;
    public Transform turretGuyPlace;

    private PlayerController driver;
    private PlayerController turretGuy;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(driver != null)
        {
        }
	}

    [Command]
    public void CmdMove(Vector2 controlAxis)
    {
        transform.Translate(Vector3.forward * controlAxis.y * 20.0f * Time.deltaTime);
        transform.Rotate(transform.up * controlAxis.x * 90.0f * Time.deltaTime);
    }

    [Command]
    public void CmdUse(GameObject player)
    {
        // Check that there isn't currently a driver
        if (driver == null)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            driver = playerController;
            playerController.transform.position = driverPlace.position;
            playerController.State = PlayerStates.InVehicleDriving;
            playerController.CurrentVehicle = this;
            playerController.transform.SetParent(gameObject.transform);
        }
        else if(turretGuy == null)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            turretGuy = playerController;
            playerController.transform.position = turretGuyPlace.position;
            playerController.State = PlayerStates.InVehicleTurret;
            playerController.CurrentVehicle = this;
            playerController.transform.SetParent(gameObject.transform);
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
                playerController.transform.position = turretGuyPlace.position;
                playerController.State = PlayerStates.InVehicleTurret;
                driver = null;
                break;
            case PlayerStates.InVehicleTurret:
                //
                if (driver != null) return;
                playerController.transform.position = driverPlace.position;
                playerController.State = PlayerStates.InVehicleDriving;
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
        playerController.State = PlayerStates.Normal;
        playerController.CurrentVehicle = null;
        playerController.transform.position += Vector3.up;
        playerController.transform.SetParent(null);
    }
}
