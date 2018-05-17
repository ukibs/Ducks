using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SelectionCanvasManager : NetworkBehaviour {

    public GameObject canvasHost;
    public GameObject canvasClient;

	// Use this for initialization
	void Start () {
        if (isServer)
        {
            Debug.Log("In Server");
			canvasHost.SetActive(true);
			canvasClient.SetActive(false);
        }
        else if (isClient)
        {
            Debug.Log("In Client");
			canvasHost.SetActive(false);
			canvasClient.SetActive(true);
        }
        else
        {
            Debug.Log("Something's wrong");
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
