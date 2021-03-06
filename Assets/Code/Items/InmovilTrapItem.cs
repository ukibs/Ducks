﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InmovilTrapItem : MonoBehaviour {
	private void OnTriggerEnter(Collider collider)
	{
		var hit = collider.gameObject;
		if (hit.CompareTag("Player"))
		{
			hit.GetComponent<PlayerController> ().CmdChangeState (MovementStates.Inmovile, 5);
			Destroy (gameObject);
		}
	}
}
