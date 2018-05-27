using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AmmunitionItem : MonoBehaviour {

	private bool isItem = false;
    private int type = 0;
	private int amountBullets = 0;

	public bool IsItem
	{
		get { return isItem; }
		set { isItem = value;}
	}

    public int Special
    {
        set { type = value; }
    }

	public int Bullets {
		set { amountBullets = value; }
	}

    private void OnTriggerEnter(Collider collider)
    {
        var hit = collider.gameObject;
        var player = hit.GetComponent<WeaponController2>();
        if (hit.CompareTag("Player") && isItem)
        {
            player.addAmmo(amountBullets, type);
            Destroy(gameObject);
        }
    }
}
