using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ChangeScene : MonoBehaviour {

    public NetworkManager nm;

	// Use this for initialization
	void Start () {
        nm = FindObjectOfType<NetworkManager>();
	}
	
	// Update is called once per frame
	void Update () {

	}

    public void ChangeToScene(string sceneToChange)
    {
        nm.ServerChangeScene(sceneToChange);
    }
}
