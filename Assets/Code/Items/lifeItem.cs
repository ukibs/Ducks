using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LifeItem : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	/*private void OnCollisionEnter(Collision collision)
	{
        Debug.Log("Collided: " + collision.transform.name);
		var hit = collision.gameObject;
		if (hit.CompareTag("Player"))
		{
			var health = hit.GetComponent<HealthController>();
			health.receiveLife (5);
			Debug.Log ("hola");
			Destroy (gameObject);
		}
	}*/

   private void OnTriggerEnter(Collider collider)
   {
        var hit = collider.gameObject;
		var health = hit.GetComponent<HealthController>();
		if (hit.CompareTag("Player") && health.Health != 100)
        {
            health.receiveLife(5);
            Destroy(gameObject);
        }
    }
}
