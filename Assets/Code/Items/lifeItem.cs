using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LifeItem : NetworkBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void OnCollisionEnter(Collision collision)
	{
		var hit = collision.gameObject;
		if (hit.CompareTag("Player"))
		{
			var health = hit.GetComponent<HealthController>();
			health.receiveLife (5);
			Debug.Log ("hola");
			Network.Destroy (gameObject);
		}
	}
}
