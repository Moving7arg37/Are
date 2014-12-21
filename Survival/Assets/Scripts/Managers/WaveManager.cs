using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WaveManager : MonoBehaviour {
    #region Public Members
    public static WaveManager Instance;     // The singleton accessor for the WaveManager

	public int currentWave = 0;				// The wave the game is currently on.
	public int totalEnemies = 0;			// The number of enemies remaining that need to be spawned in the current wave.
    public int enemiesAlive = 0;            // The number of enemies in the current wave that still need to be killed.

    public PlayerHealth playerHealth;       // Reference to the player's health

    public static WaveStates waveState = WaveStates.None;   // The current state of the wave manager.

    public GameObject[] enemies;            // The enemy types that can be spawned in the current wave.
    public Transform[] spawnPoints;         // The spawn points for enemies
    public Text waveText;                   // The UI text displaying the current wave number.
    public Text enemiesText;                // The UI text displaying the number of enemies left in the current wave.
    public Animator anim;                   // The HUDCanvas animator component.
    #endregion

    #region Private Members
    private int[] populations;              // The number of each enemy type that are to be spawned in the current wave.
    private int _numberOfWaves = 5;         // The number of waves the player must survive to beat the game.
    private float _timeBetweenWaves = 5f;   // The amount of time the game should wait between waves.
    private float _spawnTime = 3f;          // The time beween spawning enemies during a wave.
    private int _maximumGroupSpawn = 1;     // The maximum number of enemies that can be spawned at once.
    #endregion

    public enum WaveStates
    {
        None = 0,
        WaveOver = 1,
        InBetween = 2,
        InProgress = 3,
        NoMoreWaves = 4
    }

    #region Initialization
	// Use this for initialization
	void Start ()
    {
        Instance = this;
        waveState = WaveStates.None;
        populations = new int[] {0,0,0};
        currentWave = 0;
        totalEnemies = 0;
        enemiesAlive = 0;
        waveText.text = "Wave: 0";
        enemiesText.text = "Enemies: 0";
        Invoke("BeginNextWave", _timeBetweenWaves);         // Start the first wave after a delay
        InvokeRepeating("Spawn", _spawnTime, _spawnTime);   // Repeatedly attempt to spawn enemies
    }
    #endregion



    #region Updates
    void Update()
    {
        // Update the enemies text
        enemiesText.text = "Enemies: " + enemiesAlive.ToString();

        // Check that all of the enemies in the current wave are dead
        if (enemiesAlive <= 0 && waveState == WaveStates.InProgress)
        {
            waveState = WaveStates.WaveOver;                // Set the wave state to indicate we have finished a wave
            WaveOver();                                     // Call the wave over function for end of wave processes
        }
        // Check that 
        if (enemiesAlive <= 0 && waveState == WaveStates.WaveOver)
        {
            waveState = WaveStates.InBetween;               // Set the wave state to indicate we are now between waves
            Invoke ("BeginNextWave", _timeBetweenWaves);    // Start the next wave after a delay
        }
    }
    #endregion

    #region Wave Logic
    void BeginNextWave()
    {
        // Check that there are waves remaining in the game
        if (currentWave <= _numberOfWaves)
        {
            // Set the wave state to indicate we are now in a wave
            waveState = WaveStates.InProgress;

            // Set the number of enemies to be spawned depending on the current wave.
            switch (currentWave)
            {
                case 1:
                    populations = new int[] {10, 0, 0};
                    _maximumGroupSpawn = 2;
                    break;
                case 2:
                    populations = new int[] {10, 5, 0};
                    _maximumGroupSpawn = 3;
                    break;
                case 3:
                    populations = new int[] {12, 7, 1};
                    _maximumGroupSpawn = 3;
                    break;
                case 4:
                    populations = new int[] {12, 9, 4};
                    _maximumGroupSpawn = 4;
                    break;
                case 5:
                    populations = new int[] {13, 10, 7};
                    _maximumGroupSpawn = 6;
                    break;
                default:
                    populations = new int[] {0, 0, 0};
                    break;
            }

            // Sum the total number of enemies to be spawned in the current wave.
            totalEnemies = populations[0] + populations[1] + populations[2];

            // Set the number of enemies alive to match the number of enemies to be spawned.
            enemiesAlive = totalEnemies;
        }
    }


    void WaveOver()
    {
        if (currentWave == _numberOfWaves)
        {
            waveState = WaveStates.NoMoreWaves;         // All waves have been completed
        }
        else
        {
            // Increment the current wave
            currentWave++;

            // Set the wave text
            Invoke("UpdateWaveText", 1f);               // Change the text after a 1 second delay to sync up with animation

            // Make the Wave text flash
            anim.SetTrigger("WaveComplete");
        }
    }

    void UpdateWaveText()
    {
        waveText.text ="Wave: " + currentWave.ToString();
    }
    #endregion

    #region Spawning Logic
    GameObject EnemyToSpawn()
    {
        // Select a random enemy that has remaining population to spawn
        int enemyTypeIndex = -1;
        do
        {
            enemyTypeIndex = Random.Range(0, populations.Length);
        } while(populations[enemyTypeIndex] <= 0);

        // Decrement the selected unit population
        populations[enemyTypeIndex]--;

        // Decrement the total enemies count
        totalEnemies--;

        // Return the selected unit
        return enemies[enemyTypeIndex];
    }



    void Spawn()
    {
        // Randomly select the number of enemies to attempt to spawn
        int enemiesToSpawn = Random.Range(1, _maximumGroupSpawn + 1);

        // Find a random index between zero and one less than the number of spawn points
        int spawnPointIndex = Random.Range (0, spawnPoints.Length);

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            // Check that the player still has health
            if(playerHealth.currentHealth <= 0f)
            {
                return;
            }

            // Check that there are still enemies remaining this wave
            if (totalEnemies <= 0)
            {
                return;
            }

            // Get the enemy type to spawn
            GameObject enemy = this.EnemyToSpawn();

            // Create an instance of the enemy prefab at the randomly selected spawn point's position and rotation
            Instantiate (enemy, spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation);
        }
    }
    #endregion
}
