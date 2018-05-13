using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bullet : NetworkBehaviour {

    public int damage;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        var hit = collision.gameObject;
		var health = hit.GetComponent<HealthController>();
		if (health != null)
        {
            health.TakeDamage(damage);
			Destroy (gameObject);
        }
    }
}
