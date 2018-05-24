using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CustomNetworkManager : NetworkManager {

    private int playerIndex;
    private List<PlayerController> playerList = new List<PlayerController>(8);

    public override void OnServerConnect(NetworkConnection conn)
    {
        //
        base.OnServerConnect(conn);
        //
        Debug.Log("Player connected");
        
    }

    public override void OnServerSceneChanged(string sceneName)
    {

        base.OnServerSceneChanged(sceneName);
        //
        playerList.Clear();
        playerIndex = 0;
        
    }
    

    public void RegisterPlayer(GameObject player)
    {
        //
        if (playerList == null) playerList = new List<PlayerController>();

        //
        PlayerController playerController = player.GetComponent<PlayerController>();
        playerList.Add(playerController);
        // Debug.Log("Registered players: " + playerList.Count);
    }

    public void SetColorToPlayers()
    {
        for (int i = 0; i < playerList.Count; i++)
        {
            if (playerList[i] != null)
                playerList[i].RpcChangeColor(i);
            else
                Debug.Log("Player reference lost");
        }
        // Debug.Log("Setting color to " + playerList.Count + " players");
    }

    public int GetId()
    {
        Debug.Log("Giving id");
        return playerIndex++;
    }
}
