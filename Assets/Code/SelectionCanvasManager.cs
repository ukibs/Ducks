using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SelectionCanvasManager : NetworkBehaviour {

    public GameObject selectionButton;
    public GameObject loadingText;

	// Use this for initialization
	void Start () {
        if (isServer)
        {
            Debug.Log("In Server");
            loadingText.SetActive(false);
            selectionButton.SetActive(true);
        }
        else if (isClient)
        {
            Debug.Log("In Client");
            selectionButton.SetActive(false);
            loadingText.SetActive(true);
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
