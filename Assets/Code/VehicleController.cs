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
            driver.transform.position = driverPlace.position;
        }
	}

    [Command]
    public void CmdMove(Vector2 controlAxis)
    {
        transform.Translate(transform.forward * controlAxis.y * 20.0f * Time.deltaTime);
        transform.Rotate(transform.forward * controlAxis.y * 90.0f * Time.deltaTime);
    }

    [Command]
    public void CmdUse(PlayerController playerController)
    {
        playerController.State = PlayerStates.InVehicleDriving;
    }
}
