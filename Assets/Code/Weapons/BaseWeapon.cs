using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BaseWeapon : NetworkBehaviour {

    public GameObject bulletPrefab;
    public Transform shootPoint;

    private float fireRate = 0.5f;
    private float fireCooldown = 0.0f;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        float dt = Time.deltaTime;

        if (fireCooldown < fireRate) fireCooldown += dt;
    }

    //
    public bool OrderFire()
    {
        if (Input.GetAxis("Fire1") != 0.0f && fireCooldown >= fireRate)
        {

            // CmdFire();
            fireCooldown = 0.0f;
            return true;
        }
        return false;
    }

    
}
