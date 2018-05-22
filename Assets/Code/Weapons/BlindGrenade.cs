using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BlindGrenade : NetworkBehaviour {

	public int damage;
	public int range;
	public GameObject owner;


	private void OnCollisionEnter(Collision collision)
	{
		Debug.Log("Collision");
		RaycastHit[] hitInfo = Physics.SphereCastAll(transform.position, range, transform.forward);
		foreach(RaycastHit hit in hitInfo)
		{
			if (hit.transform.tag.Equals("Player"))
			{
				var player = hit.collider.gameObject;
				player.GetComponent<PlayerController> ().State = PlayerStates.Blind;
			}
		}
		Destroy(gameObject);
	}
}
