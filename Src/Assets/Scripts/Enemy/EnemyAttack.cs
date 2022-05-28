///-----------------------------------------------------------------
///   Namespace:      CompleteProject
///   Class:          EnemyAttack
///   Description:    Managages a attack from an enemy
///   Author:         Abel                    Date: 25/04/2018
///-----------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

namespace CompleteProject
{
    public class EnemyAttack : MonoBehaviour
    {
        public float timeBetweenAttacks = 0.5f;     // The time in seconds between each attack.
        public int attackDamage = 10;               // The amount of health taken away per attack.

        GameObject player;                          // Reference to the player GameObject.
        PlayerHealth playerHealth;                  // Reference to the player's health
        ThirdPersonUserControl playerControl;       // Reference to the ThirdPersonUserControl
        Animator anim;                              // Reference to the animator component.
        EnemyHealth enemyHealth;                    // Reference to this enemy's health.
        bool playerInRange;                         // Whether player is within the trigger collider and can be attacked.
        float timer;                                // Timer for counting up to the next attack.
        bool isKicking;                             // To know if player is kicking
        bool isPunching;                            // To know if player is punching


        // On awake of the game..
        void Awake()
        {
            // Setting up the references.
            player = GameObject.FindGameObjectWithTag("Player");
            playerHealth = player.GetComponent<PlayerHealth>();
            playerControl = player.GetComponent<ThirdPersonUserControl>();
            anim = GetComponent<Animator>();
            enemyHealth = GetComponent<EnemyHealth>();
            timer = timeBetweenAttacks;
        }


        // Update is called on each frame
        void Update()
        {
            // Add the time since Update was last called to the timer.
            timer += Time.deltaTime;

            // If the timer exceeds the time between attacks, the player is in range and this enemy is alive...
            if (timer >= timeBetweenAttacks && playerInRange && enemyHealth.currentHealth > 0)
            {
                // ... attack.
                Attack();
            }

            // If the player has zero or less health...
            if (playerHealth.currentHealth <= 0)
            {
                // ... tell the animator the player is dead.
                anim.SetTrigger("PlayerDead");
            }
        }


        // When collision occurs
        void OnTriggerEnter(Collider other)
        {
            // If the entering collider is the player...
            if (other.gameObject == player)
            {
                // ... the player is in range.
                playerInRange = true;
            }
        }


        // When collision occurs
        void OnTriggerExit(Collider other)
        {
            // If the entering collider is the player...
            if (other.gameObject == player)
            {
                // ... the player is in range.
                playerInRange = false;
            }
        }



        void Attack()
        {
            isKicking = playerControl.playerKicking();
            isPunching = playerControl.playerPunching();

            if (isKicking || isPunching)
            {
                if (isKicking)
                {
                    enemyHealth.TakeDamage(101, transform.position);
                    timer = -1.5f;
                }
                else
                {
                    enemyHealth.TakeDamage(101, transform.position);
                    timer = -1.5f;
                }
            }
            else
            {
                timer = -0.5f;
                anim.SetTrigger("Attack");

                if (playerHealth.healthSlider.value > 0)
                {
                    playerHealth.TakeDamage(attackDamage);
                }
            }
        }
    }
}