using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AmmunitionItem : NetworkBehaviour {

	private bool isItem = false;
	private int amountBullets = 0;

	public bool IsItem
	{
		set { isItem = value;}
	}

	public int Bullets {
		set { amountBullets = value; }
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void OnCollisionEnter(Collision collision)
	{
		var hit = collision.gameObject;
		if (hit.CompareTag("Player") && isItem)
		{
			var player = hit.GetComponent<PlayerController> ().takeWeapon ( this.gameObject,amountBullets);
			Destroy (this.gameObject);
		}
	}
}
