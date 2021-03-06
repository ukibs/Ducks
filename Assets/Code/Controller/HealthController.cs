﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HealthController : NetworkBehaviour {

    public const int maxHealth = 100;
    public const int initialScore = 0;
    public bool destroyOnDeath;

    [SyncVar(hook = "OnChangeHealth")]
    private int currentHealth = maxHealth;

	private NetworkStartPosition[] spawnPoints;
	private PlayerController player;

	[SyncVar(hook = "OnChangeScore")]
	private int score = initialScore;

    //
    public RectTransform healthBar;


	public int Health{
		get { return currentHealth; }
	}
	public int Score{
		get{ return score; }
        set { score = value; }
	}

	// Use this for initialization
	void Start () {
		spawnPoints = FindObjectsOfType<NetworkStartPosition> ();
		player = GetComponent<PlayerController> ();
	}
	
	// Update is called once per frame
	void Update () {		
	}

	public void TakeDamage(int amount, GameObject attacker)
    {
        if (!isServer)
        {
            return;
        }
        else
        {
            currentHealth -= amount;
			//if you died
            if(currentHealth <= 0)
            {
                if (destroyOnDeath)//You are a enemy
                {
                    if (attacker != null)
                    {
                        //if a player kill an enemy
                        HealthController playerController = attacker.GetComponent<HealthController>();
                        if (playerController != null)
                        {
                            playerController.Score += 10;
                        }
                        
                    }
                    Destroy(gameObject);
                }
                else//You are a player
                {
                    player.CmdThrowItems();
                    // If the player was in the vehicle
                    if (player.State == PlayerStates.InVehicleDriving || player.State == PlayerStates.InVehicleTurret)
                    {
                        VehicleController vehicleController = player.CurrentVehicle;
                        vehicleController.CmdQuitVehicle(player.gameObject);
                    }

                    currentHealth = maxHealth;
                    //
                    RpcRespawn();

					//if a player kill another player
					if (attacker != null) 
					{
                        HealthController playerController = attacker.GetComponent<HealthController>();
                        if (playerController != null) {
                            playerController.Score += 100;
						}
					}
                }
            }
        }
    }

	public void receiveLife(int amount)
	{
        if (!isServer)
        {
            return;
        }
        else
        {
            currentHealth += amount;
        }
	}

    void OnChangeHealth(int _currentHealth)
    {
		currentHealth = _currentHealth;
		healthBar.sizeDelta = new Vector2 (currentHealth, healthBar.sizeDelta.y);
        healthBar.localPosition = new Vector3((100 - currentHealth)/2, 0, 0);
    }

	void OnChangeScore(int _score)
	{
		score = _score;
	}

    [ClientRpc]
    private void RpcRespawn()
    {
        if(isLocalPlayer)
        {
			Vector3 spawnPoint = Vector3.zero;
			if (spawnPoints != null && spawnPoints.Length > 0)
			{
				spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
			}
			transform.position = spawnPoint;
        }
    }
}
