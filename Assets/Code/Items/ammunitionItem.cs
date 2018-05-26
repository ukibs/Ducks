using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AmmunitionItem : MonoBehaviour {

	private bool isItem = false;
    private bool special = false;
	private int amountBullets = 0;

	public bool IsItem
	{
		get { return isItem; }
		set { isItem = value;}
	}

    public bool Special
    {
        set { special = value; }
    }

	public int Bullets {
		set { amountBullets = value; }
	}

    private void OnTriggerEnter(Collider collider)
    {
        var hit = collider.gameObject;
		var player = hit.GetComponent<WeaponController> ();
        if (hit.CompareTag("Player") && isItem)
        {
			player.addAmmo (amountBullets, special);
            Destroy(gameObject);
        }
    }
}
