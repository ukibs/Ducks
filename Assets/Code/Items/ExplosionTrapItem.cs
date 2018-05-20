using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionTrapItem : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	/*private void OnCollisionEnter(Collision collision)
	{
		var hit = collision.gameObject;
		if (hit.CompareTag("Player") )
		{
			var player = hit.GetComponent<HealthController> ();
			//TO DO: change 100 to the player max life
			player.TakeDamage (100);
			Destroy (gameObject);
		}
	}*/

	private void OnTriggerEnter(Collider collider)
	{
		var hit = collider.gameObject;
		if (hit.CompareTag("Player") )
		{
			var player = hit.GetComponent<HealthController> ();
			//TO DO: change 100 to the player max life
			player.TakeDamage (100);
			Destroy (gameObject);
		}
	}
}
