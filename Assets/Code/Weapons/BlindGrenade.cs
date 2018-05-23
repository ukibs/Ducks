using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BlindGrenade : NetworkBehaviour {
	public int range;

	private void OnCollisionEnter(Collision collision)
	{
		RaycastHit[] hitInfo = Physics.SphereCastAll(transform.position, range, transform.forward);
		foreach(RaycastHit hit in hitInfo)
		{
			if (hit.transform.tag.Equals("Player"))
			{
				var player = hit.collider.gameObject;
				player.GetComponent<PlayerController> ().CmdChangeState (PlayerStates.Blind, 3);
			}
		}
		Destroy(gameObject);
	}
}
