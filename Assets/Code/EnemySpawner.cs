using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EnemySpawner : NetworkBehaviour {

	public GameObject enemyPrefab;	
	public int numEnemies;

	public override void OnStartServer()
	{
		for (int i=0; i < numEnemies; i++)
		{
			var spawnPosition = new Vector3(Random.Range(-11.0f, 11.0f), 0.0f,	Random.Range(-11.0f, 11.0f));

			var spawnRotation = Quaternion.Euler(0.0f, Random.Range(0,180), 0.0f);

			var enemy = (GameObject)Instantiate(enemyPrefab, spawnPosition, spawnRotation);
			NetworkServer.Spawn(enemy);
		}
	}
}