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
			player.TakeDamage (100, owner);
			Destroy (gameObject);
		}
	}
}
