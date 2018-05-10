using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ammunitionItem : NetworkBehaviour {

	private bool isItem = false;

	public bool IsItem
	{
		set { isItem = value;}
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
			Destroy (this);
		}
	}
}
