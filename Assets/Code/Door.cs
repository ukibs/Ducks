using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Door : NetworkBehaviour
{
	//public bool open = false;
	public bool enter = false;

    
    public float journeyDuration = 0.5f;
	public Vector3 displacedPosition;
	private Vector3 originalPosition;
    // 0 closed, 1 opened
    private float status = 0.0f;
    private int direction = -1;

	void Start()
	{
		originalPosition = transform.position;
		displacedPosition += originalPosition;
	}

	// Update is called once per frame
	void Update ()
	{
        if (isServer)
        {
            status += direction * Time.deltaTime / journeyDuration;
            status = Mathf.Clamp01(status);

            transform.position = Vector3.Lerp(originalPosition, displacedPosition, status);

            if (enter == true)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    direction *= -1;
                }
            }
        }
	}

	//Activate the Main function when player is near the door
	//[Command]  
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			//Debug.Log("Trigger Enter");
			(enter) = true;
		}
	}

	//Deactivate the Main function when player is go away from door
	//[Command]
	void OnTriggerExit (Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			//Debug.Log("Trigger Exit");
			(enter) = false;
		}
	}

}
