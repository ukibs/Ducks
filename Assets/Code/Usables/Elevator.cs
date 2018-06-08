using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Elevator : BaseUsable
{
	public bool enter = false;


	public float journeyDuration = 0.5f;
	public Vector3 displacedPosition;
	private Vector3 originalPosition;
	// 0 closed, 1 opened
	private float status = 0.0f;
	private int direction = 1;
	private Vector3 previousPos;
	private Vector3 positionOffset;

	public Vector3 PositionOffset {get{return positionOffset;}}

	void Start()
	{
        if (isServer)
        {
            originalPosition = transform.position;
            displacedPosition += originalPosition;
        }
	}

	// Update is called once per frame
	void FixedUpdate ()
	{
		if (isServer)
		{
			status += direction * Time.deltaTime / journeyDuration;
			status = Mathf.Clamp01(status);
			transform.position = Vector3.Lerp(originalPosition, displacedPosition, status);
		}

		if (isLocalPlayer) {
			positionOffset = transform.position - previousPos;

		}
		previousPos = transform.position;
	}


	void OnTriggerStay(Collider other){
        // Chequeo adicional específico para el ascensor
		Elevator ele= other.gameObject.GetComponent<Elevator>();
		if (ele != null) {
			Vector3 positionOffset = (transform.position - previousPos);
			positionOffset.x = 0;
			positionOffset.z = 0;
			other.transform.position += positionOffset;
		}
	}

	[Command]
	public void CmdSwitchDirection()
	{
		direction *= -1;
	}

	[Command]
	public override void CmdUse()
	{
		base.CmdUse();
		direction *= -1;
	}


}
