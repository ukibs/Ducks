using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InmovilTrapItem : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	/*private void OnCollisionEnter(Collision collision)
	{
		var hit = collision.gameObject;
		if (hit.CompareTag("Player"))
		{
			hit.GetComponent<PlayerController> ().State = PlayerStates.Trapped;
			Destroy (gameObject);
		}
	}*/

	private void OnTriggerEnter(Collider collider)
	{
		var hit = collider.gameObject;
		if (hit.CompareTag("Player"))
		{
			hit.GetComponent<PlayerController> ().State = PlayerStates.Trapped;
			Destroy (gameObject);
		}
	}
}
