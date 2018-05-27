using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowerFloor : MonoBehaviour {
	public float slow = 2.0f;

	private void OnTriggerEnter(Collider collider)
	{
		var hit = collider.gameObject;
		if (hit.CompareTag("Player") )
		{
			var player = hit.GetComponent<PlayerController> ();
			player.CmdSlower (slow);
		}
	}

	private void OnTriggerExit(Collider collider)
	{
		var hit = collider.gameObject;
		if (hit.CompareTag("Player") )
		{
			var player = hit.GetComponent<PlayerController> ();
			player.CmdSlower (0.0f);
		}
	}
}
