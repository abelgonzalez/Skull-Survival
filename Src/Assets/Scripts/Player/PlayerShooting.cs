///-----------------------------------------------------------------
///   Namespace:      CompleteProject
///   Class:          PlayerShooting
///   Description:    Manage player shooting settings
///   Author:         Abel                    Date: 26/05/2018
///-----------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

namespace CompleteProject
{
    public class PlayerShooting : MonoBehaviour
    {
        public int damagePerShot = 20;                  // The damage inflicted by each bullet.
        public float timeBetweenBullets = 0.15f;        // The time between each shot.
        public float range = 100f;                      // The distance the gun can fire.
        public Light faceLight;							// Duh
        public Slider ammunition;                       // Reference to the ammunition slider

        private int shootableMask;                      // A layer mask so the raycast only hits things on the shootable layer.
        private float timer;                            // A timer to determine when to fire.
        private Ray shootRay;                           // A ray from the gun end forwards.
        private RaycastHit shootHit;                    // A raycast hit to get information about what was hit.        
        private ParticleSystem gunParticles;            // Reference to the particle system.
        private LineRenderer gunLine;                   // Reference to the line renderer.
        private AudioSource gunAudio;                   // Reference to the audio source.
        private Light gunLight;                         // Reference to the light component.       
        private float effectsDisplayTime;               // The proportion of the timeBetweenBullets that the effects will display for.



        // On awake of the game..
        void Awake()
        {
            // Create a layer mask for the Shootable layer.
            shootableMask = LayerMask.GetMask("Shootable");

            // Set up the references.
            shootRay = new Ray();
            gunParticles = GetComponent<ParticleSystem>();
            gunLine = GetComponent<LineRenderer>();
            gunAudio = GetComponent<AudioSource>();
            gunLight = GetComponent<Light>();
            effectsDisplayTime = 0.2f;
            ammunition.value = 200f;
            //faceLight = GetComponentInChildren<Light> ();

        }


        // On each frame of the game..
        void Update()
        {
            // Add the time since Update was last called to the timer.
            timer += Time.deltaTime;

#if !MOBILE_INPUT
            // If the Fire1 button is being press and it's time to fire...
            if (Input.GetButton("Fire1") && timer >= timeBetweenBullets && Time.timeScale != 0)
            {
                // ... shoot the gun.
                Shoot();
            }
#else
            // If there is input on the shoot direction stick and it's time to fire...
            if ((CrossPlatformInputManager.GetAxisRaw("Mouse X") != 0 || CrossPlatformInputManager.GetAxisRaw("Mouse Y") != 0) && timer >= timeBetweenBullets)
            {
                // ... shoot the gun
                Shoot();
            }
#endif
            // If the timer has exceeded the proportion of timeBetweenBullets that the effects should be displayed for...
            if (timer >= timeBetweenBullets * effectsDisplayTime)
            {
                // ... disable the effects.
                DisableEffects();
            }
        }


        public void DisableEffects()
        {
            // Disable the line renderer and the light.
            gunLine.enabled = false;
            faceLight.enabled = false;
            gunLight.enabled = false;
        }


        public void Shoot()
        {
            // Reset the timer.
            timer = 0f;

            // Shows an visual alert to player he's without ammunitions
            if (ammunition.value <= 0)
            {
                Debug.Log("Run out of ammunition");
                ammunition.colors = ColorBlock.defaultColorBlock;
                return;
            }

            // Play the gun shot audioclip.
            gunAudio.Play();

            // Enable the lights.
            gunLight.enabled = true;
            faceLight.enabled = true;

            // Stop the particles from playing if they were, then start the particles.
            gunParticles.Stop();
            gunParticles.Play();

            // Enable the line renderer and set it's first position to be the end of the gun.
            gunLine.enabled = true;
            gunLine.SetPosition(0, transform.position);

            // Set the shootRay so that it starts at the end of the gun and points forward from the barrel.
            shootRay.origin = transform.position;
            shootRay.direction = transform.forward;

            ammunition.value -= 1f;

            // Perform the raycast against gameobjects on the shootable layer and if it hits something...
            if (Physics.Raycast(shootRay, out shootHit, range, shootableMask))
            {
                // Try and find an EnemyHealth script on the gameobject hit.
                EnemyHealth enemyHealth = shootHit.collider.GetComponent<EnemyHealth>();

                // If the EnemyHealth component exist...
                if (enemyHealth != null)
                {
                    // ... the enemy should take damage.
                    enemyHealth.TakeDamage(damagePerShot, shootHit.point);
                }

                // Set the second position of the line renderer to the point the raycast hit.
                gunLine.SetPosition(1, shootHit.point);
            }
            // If the raycast didn't hit anything on the shootable layer...
            else
            {
                // ... set the second position of the line renderer to the fullest extent of the gun's range.
                gunLine.SetPosition(1, shootRay.origin + shootRay.direction * range);
            }
        }

        public void setAmunition(float value)
        {
            ammunition.value = value;
        }
    }
}