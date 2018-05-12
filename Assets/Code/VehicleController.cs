using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class VehicleController : NetworkBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    [Command]
    void CmdMove(Vector2 controlAxis)
    {
        transform.Translate(transform.forward * controlAxis.y * 20.0f * Time.deltaTime);
        transform.Rotate(transform.forward * controlAxis.y * 90.0f * Time.deltaTime);
    }
}
