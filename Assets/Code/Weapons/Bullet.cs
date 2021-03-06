﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bullet : NetworkBehaviour {

    public int damage;
	public GameObject owner;
    public GameObject bloodPrefab;

    private Effects effectManager;

	// Use this for initialization
	void Start () {
        effectManager = FindObjectOfType<Effects>();
	}

    private void OnCollisionEnter(Collision collision)
    {
        var hit = collision.gameObject;
		var health = hit.GetComponent<HealthController>();
		if (health != null)
        {
			health.TakeDamage(damage, owner);
            GameObject blood = Instantiate(bloodPrefab, transform.position, Quaternion.identity);
            Destroy(blood, 1);
        }
        effectManager.playChoque();
        Destroy(gameObject);
    }
}
