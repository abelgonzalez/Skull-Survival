///-----------------------------------------------------------------
///   Namespace:      CompleteProject
///   Class:          PauseMenu
///   Description:    Manage Pause Menu on the game, principally was necessary for behavior with Kinect.
///   Author:         Abel                    Date: 21/05/2018
///-----------------------------------------------------------------

using UnityEngine;
using UnityEngine.SceneManagement;


namespace CompleteProject
{
    public class PauseMenu : MonoBehaviour
    {

        private GameObject ObjPausa;
        private GameObject menu;


        void Awake()
        {
            // Getting the Game Complete and PauseMenu
            ObjPausa = GameObject.FindGameObjectWithTag("GameComplete");
            menu = GameObject.FindGameObjectWithTag("MenuPausa");
        }


        void Start()
        {
            // Initially, menu will be inactive
            menu.SetActive(false);
        }


        void Update()
        {
            // If it is pressed Escape key ...
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Change();
            }
        }


        public void Change()
        {
            // Detects changes on game
            if (Time.timeScale == 1)
                DoPause();

            else if (Time.timeScale == 0)
                Continue();
        }


        public void DoPause()
        {
            // Pauses the game and shows Menu Pause
            ObjPausa = GameObject.FindGameObjectWithTag("GameComplete");
            ObjPausa.SetActive(false);
            menu.SetActive(true);
            Time.timeScale = 0;
        }


        public void Continue()
        {
            // Continues the current Game
            ObjPausa.SetActive(true);
            menu.SetActive(false);
            Time.timeScale = 1;
        }


        public void Menu()
        {
            // gets the current scene
            int level = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(level);
        }

        public void RestartLevel()
        {
            // gets the current scene for restart the game
            int level = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(level);
            Continue();
        }
    }
}