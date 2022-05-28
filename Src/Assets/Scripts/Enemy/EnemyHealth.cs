///-----------------------------------------------------------------
///   Namespace:      CompleteProject
///   Class:          EnemyHealth
///   Description:    Managages a health from an enemy
///   Author:         Abel                    Date: 20/04/2018
///-----------------------------------------------------------------

using UnityEngine;
using UnityEngine.AI;

namespace CompleteProject
{
    public class EnemyHealth : MonoBehaviour
    {
        public int startingHealth = 100;            // The amount of health the enemy starts the game with.
        public int currentHealth;                   // The current health the enemy has.
        public float sinkSpeed = 2.5f;              // The speed at which the enemy sinks through the floor when dead.
        public int scoreValue = 10;                 // The amount added to the player's score when the enemy dies.
        public AudioClip deathClip;                 // The sound to play when the enemy dies.
        public float timeBetweenAttacks = 0.5f;     // The time in seconds between each attack.
        public bool isResucitable;                  // Indicates if this enemy can resucitate

        private Animator anim;                      // Reference to the animator.
        private AudioSource[] damagedClip;          // Reference to the audio that plays when the enemy is damaged.
        private ParticleSystem hitParticles;        // Reference to the particle system that plays when the enemy is damaged.
        private CapsuleCollider capsuleCollider;    // Reference to the capsule collider.
        private NavMeshAgent nav;                   // Reference to the nav mesh agent.
        private bool isDead;                        // Whether the enemy is dead.
        private bool isSinking;                     // Whether the enemy has started sinking through the floor.
        private bool isDamaged;                     // Whether the enemy gets damaged.
        private float timer;                        // Timer for counting up to the next attack.



        // On awake of the game..
        void Awake()
        {
            // Setting up the references.
            anim = GetComponent<Animator>();
            damagedClip =  GetComponents<AudioSource>();
            hitParticles = GetComponentInChildren<ParticleSystem>();
            capsuleCollider = GetComponent<CapsuleCollider>();

            // Setting the current health when the enemy first spawns.
            currentHealth = startingHealth;
            timer = timeBetweenAttacks;

        }


        // On each frame of the game..
        void Update()
        {
            // Add the time since Update was last called to the timer.
            timer += Time.deltaTime;

            // If the enemy should be sinking...
            if (isSinking)
            {
                // ... move the enemy down by the sinkSpeed per second.
                transform.Translate(-Vector3.up * sinkSpeed * Time.deltaTime);
            }
        }


        public void TakeDamage(int amount, Vector3 hitPoint)
        {
            // If the enemy is dead...
            if (isDead)
                // ... no need to take damage so exit the function.
                return;

            // Reduce the current health by the amount of damage sustained.
            currentHealth -= amount;

            if (hitParticles) // TODO: Incorporate Particle System for this!
            {
                // Set the position of the particle system to where the hit was sustained.
                hitParticles.transform.position = hitPoint;

                // And play the particles.
                hitParticles.Play();
            }

            // Play the hurt sound effect.
            damagedClip[0].Play();

            // Play the animation for indicate enemy was affected.
            anim.SetTrigger("Damage");

            // If the current health is less than or equal to zero...
            if (currentHealth <= 0)
            {
                if (isResucitable)
                {
                    // Resucitate the enemy
                    Resurrection();
                }
                else
                {
                    // ... the enemy is dead.
                    Death();
                }
            }
        }


        void Resurrection()
        {
            // Play the hurt sound effect.
            damagedClip[1].Play();

            // Play effect for resurrection
            anim.SetTrigger("Resurrection");

            // O enemy wal again, but, with half of life
            currentHealth = 50;

            // Prevent a infinite enemy life 
            isResucitable = false;

            // Stopping the nav mesh during an execution of an animation
            //nav.isStopped = true; 
        }


        void Death()
        {
            // The enemy is dead.
            isDead = true;

            // Turn the collider into a trigger so shots can pass through it.
            capsuleCollider.isTrigger = true;

            // Tell the animator that the enemy is dead.
            anim.SetTrigger("Dead");

            // Change the audio clip of the audio source to the death clip and play it (this will stop the hurt clip playing).
            damagedClip[0].clip = deathClip;
            damagedClip[0].Play();

            // Routine for deletion of a GameObject
            StartSinking();
        }


        void StartSinking()
        {
            // Find and disable the Nav Mesh Agent.
            GetComponent<NavMeshAgent>().enabled = false;

            // Find the rigidbody component and make it kinematic (since we use Translate to sink the enemy).
            GetComponent<Rigidbody>().isKinematic = true;

            // The enemy should no sink.
            isSinking = true;

            // Increase the score by the enemy's score value.
            Score.score += scoreValue;

            // After 3 seconds destory the enemy.
            Destroy(gameObject, 3f);
        }
    }
}