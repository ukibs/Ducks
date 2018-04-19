using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {

    public float movementSpeed = 5.0f;
    public Camera cam;
    public GameObject bulletPrefab;
    public GameObject myPrefab;

    private float fireRate = 0.5f;
    private float fireCooldown = 0.0f;

	// Use this for initialization
	void Start () {
		//cam = GetComponentInChildren<>
	}
	
	// Update is called once per frame
	void Update () {

        if (!isLocalPlayer)
        {
            cam.enabled = false;
            gameObject.GetComponentInChildren<AudioListener>().enabled = false;
            return;
        }
        else
        {
            // Delta time
            float dt = Time.deltaTime;
            // Movement
            float vAxis = Input.GetAxis("Vertical");
            float hAxis = Input.GetAxis("Horizontal");
            transform.Translate(0.0f, 0.0f, vAxis * movementSpeed * dt);
            transform.Rotate(0.0f, hAxis * 90.0f * dt, 0.0f);
            // Shoot
            if (fireCooldown < fireRate) fireCooldown += dt;
            if(Input.GetAxis("Fire1") != 0.0f && fireCooldown >= fireRate)
            {
                CmdFire();
                fireCooldown = 0.0f;
            }
        }
	}

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        this.gameObject.GetComponent<MeshRenderer>().material.color = Color.cyan;
        GameObject enemySkin = Instantiate(myPrefab, transform.position, transform.rotation);
        enemySkin.transform.parent = gameObject.transform;
        // Esconder la lower nose
        transform.GetChild(0).gameObject.SetActive(false);
    }

    [Command]
    void CmdFire()
    {
        GameObject newBullet = GameObject.Instantiate(bulletPrefab, transform.position + transform.forward, transform.rotation);
        //newBullet.GetComponent<Rigidbody>().AddForce(newBullet.transform.forward * 100.0f, ForceMode.Impulse);
        newBullet.GetComponent<Rigidbody>().velocity = newBullet.transform.forward * 10f;

        NetworkServer.Spawn(newBullet);

        Destroy(newBullet, 4.0f);
    }
}
