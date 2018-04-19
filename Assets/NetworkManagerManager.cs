using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManagerManager : MonoBehaviour {
	public int maxPlayers = 8;
	NetworkManager manager;

	// Use this for initialization
	void Start () {
		manager = GetComponent<NetworkManager> ();
		manager.maxConnections = maxPlayers;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
