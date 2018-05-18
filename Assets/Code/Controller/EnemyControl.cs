using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EnemyControl : NetworkBehaviour {

    private CharacterController cc;
    // private NetworkManager netMgr;
    private List<PlayerController> players;
    private PlayerController objectivePlayer;

	// Use this for initialization
	void Start () {
        cc = GetComponent<CharacterController>();
        // netMgr = FindObjectOfType<NetworkManager>();
        players = new List<PlayerController>();
        GetPlayers();
        objectivePlayer = GetNearestPlayer();
	}
	
	// Update is called once per frame
	void Update () {
        //
        if(objectivePlayer != null)
        {
            Vector3 objectiveOffset = objectivePlayer.transform.position - transform.position;
            objectiveOffset.y = 0;
            transform.rotation = Quaternion.LookRotation(objectiveOffset);
            cc.Move(transform.forward * Time.deltaTime);
        }
        else
        {
            GetPlayers();
            objectivePlayer = GetNearestPlayer();
        }
	}

    void GetPlayers()
    {
        // Clear the precious list
        players.Clear();
        // And fill it
        PlayerController[] playerArray = FindObjectsOfType<PlayerController>();
        for(int i = 0; i < playerArray.Length; i++)
        {
            players.Add(playerArray[i]);
        }
    }

    PlayerController GetNearestPlayer()
    {
        if (players == null || players.Count == 0)
            return null;

        PlayerController nearestPlayer = players[0];
        float minDistance = (nearestPlayer.transform.position - transform.position).magnitude;
        for (int i = 1; i < players.Count; i++)
        {
            if((nearestPlayer.transform.position - transform.position).magnitude < minDistance)
            {
                nearestPlayer = players[i];
                minDistance = (nearestPlayer.transform.position - transform.position).magnitude;
            }
        }
        return nearestPlayer;
    }
}
