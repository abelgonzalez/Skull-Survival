///-----------------------------------------------------------------
///   Namespace:      CompleteProject
///   Class:          EnemyManager
///   Description:    Managages enemies, where is going to appear on scene
///   Author:         Abel                    Date: 29/04/2018
///-----------------------------------------------------------------

using UnityEngine;

namespace CompleteProject
{
    public class EnemyManager : MonoBehaviour
    {
        public ThirdPersonUserControl player;   // Reference to the player's control.
        public PlayerHealth playerHealth;       // Reference to the player's health.        
        public GameObject enemy;                // The enemy prefab to be spawned.
        public float intiSpawnTime = 30f;       // How long between each spawn.
        public float spawnTime = 15f;           // How long between each spawn.
        public Transform[] spawnPoints;         // An array of the spawn points this enemy can spawn from.

        
        // At the start of the game..
        void Start()
        {
            // Call the Spawn function after a delay of the spawnTime and then continue to call after the same amount of time.            
            InvokeRepeating("Spawn", intiSpawnTime, spawnTime);
        }


        // Spawing enemies..
        void Spawn()
        {
            // If the player has no health left...
            if (playerHealth.currentHealth <= 0f)
            {
                // ... exit the function.
                return;
            }

            // Find a random index between zero and one less than the number of spawn points.
            int spawnPointIndex = Random.Range(0, spawnPoints.Length);

            // Create an instance of the enemy prefab at the randomly selected spawn point's position and rotation.            
            if (!player.playerCalibration())
            { 
                Instantiate(enemy, spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation);
            }
        }
    }
}