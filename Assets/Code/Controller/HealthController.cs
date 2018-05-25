using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HealthController : NetworkBehaviour {

    public const int maxHealth = 100;
    public bool destroyOnDeath;
	public Texture t;

    [SyncVar(hook = "OnChangeHealth")]
    private int currentHealth = maxHealth;
	private NetworkStartPosition[] spawnPoints;
	private PlayerController player;

    //
    public RectTransform healthBar;


	public int Health{
		get { return currentHealth; }
	}

	// Use this for initialization
	void Start () {
		spawnPoints = FindObjectsOfType<NetworkStartPosition> ();
		player = FindObjectOfType<PlayerController> ();
	}
	
	// Update is called once per frame
	void Update () {		
	}

	private void OnGUI()
	{
		GUI.DrawTexture (new Rect (Screen.width * 8 / 10, Screen.height / 10, 100, 10), t, ScaleMode.StretchToFill);
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
                if(destroyOnDeath)//You are a enemy
                {
					//if a player kill an enemy
					PlayerController playerController = attacker.GetComponent<PlayerController> ();
					if (playerController != null) 
					{
						playerController.Score += 10;
					}
					Destroy(gameObject);
                }
                else//You are a player
                {
                    
                    // If the player was in the vehicle
                    if(player.State == PlayerStates.InVehicleDriving || player.State == PlayerStates.InVehicleTurret)
                    {
                        VehicleController vehicleController = player.CurrentVehicle;
                        vehicleController.CmdQuitVehicle(player.gameObject);
                    }

                    currentHealth = maxHealth;
                    Debug.Log("Dead!");
                    //
                    RpcRespawn();

					//if a player kill another player
					if (attacker != null) 
					{
						PlayerController playerController = attacker.GetComponent<PlayerController> ();
						if (playerController != null) {
							playerController.Score += 100;
						}
					}
                }
				player.CmdThrowItems ();
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
		healthBar.localPosition = new Vector3 (0, 0, 0);
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
