///-----------------------------------------------------------------
///   Namespace:      CompleteProject
///   Class:          Scene
///   Description:    Managages some aspect from Scene
///   Author:         Abel                    Date: 13/05/2018
///-----------------------------------------------------------------

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CompleteProject
{
    public class Scene : MonoBehaviour
    {
        public Text countText;                  // The current text to show the count on for the Text UI game objects
        public Text goalText;                   // The current goal text to show the count on for the Text UI game objects
        public PlayerHealth playerHealth;       // Reference to the player's health.
        public float restartDelay = 5f;         // Time to wait before restarting the level
        public Text winText;                    // The current text to show the text when player gets all the objects on game

        private float restartTimer;             // Timer to count up to restarting the level
        private Animator anim;                  // Reference to the animator component.        


        // On awake of the game..
        void Awake()
        {
            // Set up the reference.
            anim = GetComponent<Animator>();
        }


        // On each frame of the game..
        void Update()
        {
            // If the player has run out of health...
            if (playerHealth.currentHealth <= 0)
            {
                // ... plays Game Over Animation clip
                GameOverAnimation();

                // .. increment a timer to count up to restarting.
                restartTimer += Time.deltaTime;

                // .. if it reaches the restart delay or player wants restart the game...
                if (restartTimer >= restartDelay)
                {
                    // ... then restart the current scene
                    RestartScene();
                }
            }

            // If player has reached all goals
            if (goalText.text == "10/10")
            {
                // ... plays Win Animation clip
                WinAnimation();
                ClearScene();

                // .. increment a timer to count up to restarting.
                restartTimer += Time.deltaTime;

                // .. if it reaches the restart delay or player wants restart the game...
                if (restartTimer >= restartDelay)
                {
                    // ... then restart the current scene
                    RestartScene();
                }
            }
        }

        void WinAnimation()
        {
            // ... tell the animator the game is over.
            Animator animPlayer = playerHealth.GetComponent<Animator>();
            animPlayer.SetBool("Taunt",true);

            winText.enabled = true;
            winText.text = "You Win!";           
        }

        void GameOverAnimation()
        {            
            // ... tell the animator the game is over.
            anim.SetTrigger("GameOver");
            countText.enabled = false;
            goalText.enabled = false;
        }

        void ClearScene()
        {
            // Get all enemies on scene
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

            foreach (GameObject e in enemies)
            {
                // Mark it as inactive
                e.SetActive(false);
            }
        }

        void RestartScene()
        {
            // .. then reload the currently loaded level.     
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}