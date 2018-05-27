using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EnemyControl : NetworkBehaviour {

    public float movementSpeed = 1;
    public float playerCheckRate = 2;
    public GameObject weaponPrefab;
    public GameObject bulletPrefab;
    public Transform weaponPoint;
    public float attackDistance = 10;
    public float fireRate = 1;

    protected CharacterController cc;
    protected List<PlayerController> players;
    protected PlayerController objectivePlayer;
    //
    protected float playerCheckCounter;
    protected float verticalSpeed;
    protected Vector3 gravity = new Vector3(0.0f, -9.81f, 0.0f);
    protected GameObject weapon;
    protected Transform weaponClass;
    protected WeaponController weaponController;
    private float fireCounter;

    protected Effects effectManager;

	// Use this for initialization
	protected virtual void Start () {
        //
        if (!isServer) return;

        //
        cc = GetComponent<CharacterController>();
        players = new List<PlayerController>();
        GetPlayers();
        objectivePlayer = GetNearestPlayer();
        //
        weapon = Instantiate(weaponPrefab, weaponPoint);
        weaponClass = weapon.GetComponent<BaseWeapon>().shootPoint;
        weaponController = GetComponent<WeaponController>();
        //
        effectManager = FindObjectOfType<Effects>();
    }

    // Update is called once per frame
    protected virtual void Update () {
        //
        if (!isServer) return;
        //
        float dt = Time.deltaTime;
        //
        if(objectivePlayer != null)
        {
            Vector3 objectiveOffset = objectivePlayer.transform.position - transform.position;
            objectiveOffset.y = 0;
            transform.rotation = Quaternion.LookRotation(objectiveOffset);
            if (objectiveOffset.magnitude > attackDistance)
            {
                Vector3 movementToApply = ((transform.forward * movementSpeed) + (transform.up * verticalSpeed)) * dt;
                cc.Move(movementToApply);
            }
            else
            {
                // Shooting stuff
                fireCounter += dt;
                if (fireCounter >= fireRate)
                {
                    CmdFire();
                    weaponController.wasteBullet();
                    fireCounter -= fireRate;
                    effectManager.playEffect(0);
                }
            }
        }
        else
        {
            GetPlayers();
            objectivePlayer = GetNearestPlayer();
        }
        //
        playerCheckCounter += dt;
        if(playerCheckCounter >= playerCheckRate)
        {
            GetPlayers();
            objectivePlayer = GetNearestPlayer();
            Debug.Log("Players " + players.Count + ", nearest one" + objectivePlayer);
            playerCheckCounter -= playerCheckRate;
        }
        //
        ApplyGravity(dt);
	}

    #region User Methods

    protected void ApplyGravity(float dt)
    {
        
        if (cc.isGrounded)
            verticalSpeed = 0.0f;
        else
            verticalSpeed += gravity.y * dt;
    }

    protected void GetPlayers()
    {
        // Clear the precious list
        players.Clear();
        // And fill it
        PlayerController[] playerArray = FindObjectsOfType<PlayerController>();
        for(int i = 0; i < playerArray.Length; i++)
        {
            players.Add(playerArray[i]);
        }
    }

    protected PlayerController GetNearestPlayer()
    {
        if (players == null || players.Count == 0)
            return null;

        PlayerController nearestPlayer = players[0];
        float minDistance = (nearestPlayer.transform.position - transform.position).magnitude;
        for (int i = 1; i < players.Count; i++)
        {
            if((players[i].transform.position - transform.position).magnitude < minDistance)
            {
                nearestPlayer = players[i];
                minDistance = (nearestPlayer.transform.position - transform.position).magnitude;
            }
        }
        return nearestPlayer;
    }

    #endregion

    #region Commands

    [Command]
    protected void CmdFire()
    {
        GameObject newBullet = GameObject.Instantiate(bulletPrefab,
            weaponClass.position,
            weaponClass.rotation);

        newBullet.GetComponent<Rigidbody>().velocity = newBullet.transform.forward * 10f;

        NetworkServer.Spawn(newBullet);

        Destroy(newBullet, 4.0f);
    }

    #endregion
}
