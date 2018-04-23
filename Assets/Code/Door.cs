using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Door : NetworkBehaviour
{
	public bool open = false;
	public bool enter = false;


	public float movementSpeed = 0.5f;
	public Vector3 displacedPosition;
	private Vector3 originalPosition;
	private float distCovered;
	private float fracJourney;
	private float journeyLength, startTime;
	private Vector3 nextPosition, previousPosition;

	void Start()
	{
		originalPosition = transform.position;
		displacedPosition += originalPosition;
		nextPosition = displacedPosition;
		previousPosition = originalPosition;
		//
		journeyLength = Vector3.Distance(originalPosition, displacedPosition);
	}

	// Update is called once per frame
	void Update ()
	{
			distCovered = (Time.time - startTime) * movementSpeed;
			fracJourney = distCovered / journeyLength;
			if (open == true) {
				transform.position = Vector3.Lerp (previousPosition, nextPosition, fracJourney);
			}

			if (open == false) {
				transform.position = Vector3.Lerp (nextPosition, previousPosition, fracJourney);
			}

		if(enter == true)
		{
			if(Input.GetKeyDown(KeyCode.E))
			{
				open = !open;
			}
		}
	}

	//Activate the Main function when player is near the door
	[RPC]  
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			//Debug.Log("Trigger Enter");
			(enter) = true;
		}
	}

	//Deactivate the Main function when player is go away from door
	[RPC]
	void OnTriggerExit (Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			//Debug.Log("Trigger Exit");
			(enter) = false;
		}
	}

}
