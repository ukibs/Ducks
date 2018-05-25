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

    private void OnTriggerEnter(Collider collider)
    {
        var hit = collider.gameObject;
        if (hit.CompareTag("Player") && isItem)
        {
			Debug.Log ("Toco munición");
			hit.GetComponent<PlayerController>().CmdTakeWeapon(gameObject.GetComponent<BaseWeapon>(), amountBullets);
            Destroy(gameObject);
        }
    }
}
