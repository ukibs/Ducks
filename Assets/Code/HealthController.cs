using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HealthController : NetworkBehaviour {

    public const int maxHealth = 100;
    public bool destroyOnDeath;

    [SyncVar(hook = "OnChangeHealth")]
    private int currentHealth = maxHealth;
	private NetworkStartPosition[] spawnPoints;

    //
    public RectTransform healthBar;
	public RectTransform healthBarScreen;

	// Use this for initialization
	void Start () {
		spawnPoints = FindObjectsOfType<NetworkStartPosition> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void TakeDamage(int amount)
    {
        if (!isServer)
        {
            return;
        }
        else
        {
            currentHealth -= amount;
            if(currentHealth <= 0)
            {
                if(destroyOnDeath)
                {
                    Destroy(gameObject);
                }
                else
                {
                    currentHealth = maxHealth;
                    Debug.Log("Dead!");
                    //
                    RpcRespawn();
                }
            }
        }
    }

    void OnChangeHealth(int _currentHealth)
    {
		currentHealth = _currentHealth;
		healthBar.sizeDelta = new Vector2 (currentHealth, healthBar.sizeDelta.y);
		healthBar.localPosition = new Vector3 (maxHealth - currentHealth, 0, 0);
		healthBarScreen = healthBar;

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
