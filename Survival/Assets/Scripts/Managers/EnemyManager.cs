using UnityEngine;

public class EnemyManager : MonoBehaviour
{
	public PlayerHealth playerHealth;       // Reference to the player's heatlh.
	public GameObject enemy;                // The enemy prefab to be spawned.
	public float spawnTime = 3f;            // How long between each spawn.
	public Transform[] spawnPoints;         // An array of the spawn points this enemy can spawn from.
	
	
	void Start ()
	{
		// Call the Spawn function after a delay of the spawnTime and then continue to call after the same amount of time.
		InvokeRepeating ("Spawn", spawnTime, spawnTime);
	}
	
	
	void Spawn ()
	{
        // Check that the player still has health
		if(playerHealth.currentHealth <= 0f)
		{
			return;
		}

        // Check that there are still enemies remaining this wave
        if (WaveManager.totalEnemies <= 0)
        {
            return;
        }

        // Decement the number of enemies to spawn
        WaveManager.totalEnemies--;
		
		// Find a random index between zero and one less than the number of spawn points.
		int spawnPointIndex = Random.Range (0, spawnPoints.Length);
		
		// Create an instance of the enemy prefab at the randomly selected spawn point's position and rotation.
		Instantiate (enemy, spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation);
	}
}