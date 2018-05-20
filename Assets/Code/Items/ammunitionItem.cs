using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AmmunitionItem : MonoBehaviour {

	public bool isItem = false;
	private int amountBullets = 0;

	public bool IsItem
	{
		get { return isItem; }
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

	/*private void OnCollisionEnter(Collision collision)
	{
		var hit = collision.gameObject;
		Debug.Log ("Estoy: " + isItem);
		if (hit.CompareTag("Player") && isItem)
		{
			Debug.Log ("Cojo municion");
			hit.GetComponent<PlayerController> ().takeWeapon ( this.gameObject.GetComponent<BaseWeapon>(), amountBullets);
			Destroy (this.gameObject);
		}
	}*/

    private void OnTriggerEnter(Collider collider)
    {
        var hit = collider.gameObject;
        if (hit.CompareTag("Player") && isItem)
        {
			hit.GetComponent<PlayerController>().takeWeapon(this.gameObject.GetComponent<BaseWeapon>(), amountBullets);
            Destroy(gameObject);
        }
    }
}
