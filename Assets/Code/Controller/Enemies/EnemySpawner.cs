using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EnemySpawner : NetworkBehaviour {

	public GameObject[] enemyPrefabs;	
	public int numEnemies;
    public float enemySpawnRate;

    private float counter;

	public override void OnStartServer()
	{
		for (int i=0; i < numEnemies; i++)
		{
            SpawnEnemy();
		}
	}

    private void Update()
    {
        counter += Time.deltaTime;
        if(counter >= enemySpawnRate)
        {
            SpawnEnemy();
            counter -= enemySpawnRate;
        }
    }

    void SpawnEnemy()
    {
        var spawnPosition = new Vector3(Random.Range(-11.0f, 11.0f), 1.0f, Random.Range(-11.0f, 11.0f));

        var spawnRotation = Quaternion.Euler(0.0f, Random.Range(0, 180), 0.0f);

        int enemyToUse = Random.Range(0, 2);

        var enemy = (GameObject)Instantiate(enemyPrefabs[enemyToUse], spawnPosition, spawnRotation);
        NetworkServer.Spawn(enemy);
    }
}