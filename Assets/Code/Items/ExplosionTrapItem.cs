using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionTrapItem : MonoBehaviour {

	public GameObject owner;

	private void OnTriggerEnter(Collider collider)
	{
		var hit = collider.gameObject;
		if (hit.CompareTag("Player") || hit.CompareTag("Enemy"))
		{
			var player = hit.GetComponent<HealthController> ();
			//TO DO: change 100 to the player max life
			player.TakeDamage (100, owner);
			Destroy (gameObject);
		}
	}
}
