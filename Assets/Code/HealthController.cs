using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HealthController : NetworkBehaviour {

    public const int maxHealth = 100;
    public bool destroyOnDeath;

    [SyncVar(hook = "OnChangeHealth")]
    private int currentHealth = maxHealth;

    //
    public RectTransform healthBar;

	// Use this for initialization
	void Start () {

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
        Debug.Log("Health change");
        //healthBar.sizeDelta = new Vector2(_currentHealth, )
    }

    [ClientRpc]
    private void RpcRespawn()
    {
        if(isLocalPlayer)
        {
            transform.position = Vector3.zero;
        }
    }
}
