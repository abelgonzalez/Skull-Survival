///-----------------------------------------------------------------
///   Namespace:      CompleteProject
///   Class:          EnemyMovement
///   Description:    Managages enemies movement and reference to a player with AI engine
///   Author:         Abel                    Date: 08/05/2018
///-----------------------------------------------------------------

using UnityEngine;
using UnityEngine.AI;

namespace CompleteProject
{
    public class EnemyMovement : MonoBehaviour
    {
        private Transform player;               // Reference to the player's position.
        private PlayerHealth playerHealth;      // Reference to the player's health.
        private EnemyHealth enemyHealth;        // Reference to this enemy's health.
        private NavMeshAgent nav;               // Reference to the nav mesh agent.


        // On awake of the game..
        void Awake()
        {
            // Set up the references.
            player = GameObject.FindGameObjectWithTag("Player").transform;
            playerHealth = player.GetComponent<PlayerHealth>();
            enemyHealth = GetComponent<EnemyHealth>();
            nav = GetComponent<NavMeshAgent>();
        }


        // On each frame of the game..
        void Update()
        {         
                // If the enemy and the player have health left ...
            if (enemyHealth.currentHealth > 0 && playerHealth.currentHealth > 0)
            {
                // ... set the destination of the nav mesh agent to the player.
                nav.SetDestination(player.position);
            }
            // Otherwise ...
            else
            {
                // ... disable the nav mesh agent.
                nav.enabled = false;
            }
        }
    }
}