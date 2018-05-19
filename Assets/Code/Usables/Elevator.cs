using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Elevator : NetworkBehaviour
{
	//public bool open = false;
	public bool enter = false;


	public float journeyDuration = 0.5f;
	public Vector3 displacedPosition;
	private Vector3 originalPosition;
	// 0 closed, 1 opened
	private float status = 0.0f;
	private int direction = 1;
	private Vector3 previousPos;

	void Start()
	{
		originalPosition = transform.position;
		displacedPosition += originalPosition;
	}

	// Update is called once per frame
	void FixedUpdate ()
	{
		if (isServer)
		{
			//Debug.Log("In server");
			status += direction * Time.deltaTime / journeyDuration;
			status = Mathf.Clamp01(status);

			previousPos = transform.position;

			transform.position = Vector3.Lerp(originalPosition, displacedPosition, status);

		}

		/*if (!isLocalPlayer)
        {
            Debug.Log("Not Local player");
            if (enter == true)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    Debug.Log("Key pressed");
                    // direction *= -1;
                    CmdSwitchDirection();
                }
            }
        }*/

		if (isLocalPlayer)
		{
			Debug.Log("In local player");
		}
	}


	void OnTriggerStay(Collider other){
        // Chequeo adicional específico para el ascensor
        Vector3 positionOffset = (transform.position - previousPos);
        positionOffset.x = 0;
        positionOffset.z = 0;
        other.transform.position += positionOffset;
		//Debug.Log ("estoy dentrp");
		
	}

	//Activate the Main function when player is near the door
	//[Command]  
	//[RPC]
	/*private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			Debug.Log("Trigger Enter");
			(enter) = true;
			other.GetComponent<PlayerController>().elevator = this;
		}
	}

	//Deactivate the Main function when player is go away from door
	//[Command]
	//[RPC]
	void OnTriggerExit (Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			Debug.Log("Trigger Exit");
			(enter) = false;
			other.GetComponent<PlayerController>().elevator = null;
		}
	}

	[Command]
	public void CmdSwitchDirection()
	{
		Debug.Log("Switching direction");
		direction *= -1;
	}*/

}
