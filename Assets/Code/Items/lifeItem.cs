using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LifeItem : MonoBehaviour {
   private void OnTriggerEnter(Collider collider)
   {
        var hit = collider.gameObject;
		var health = hit.GetComponent<HealthController>();
		if (hit.CompareTag("Player"))
        {
			if(health.Health != 100)
			{
            	health.receiveLife(5);
			}
            Destroy(gameObject);
        }
    }
}
