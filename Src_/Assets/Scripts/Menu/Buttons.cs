///-----------------------------------------------------------------
///   Namespace:      CompleteProject
///   Class:          Buttons
///   Description:    Manage buttons behavior on the scene
///   Author:         Abel                    Date: 19/05/2018
///-----------------------------------------------------------------

using UnityEngine;
using Windows.Kinect;

namespace CompleteProject
{
    public class Buttons : MonoBehaviour
    {
        private GameObject ObjPausa;        // Game to be paused
        private GameObject menu;            // Main menu of game
        private KinectSensor _Sensor;       // Getting the state of sensor
        private BodyFrameReader _Reader;    // Reader of Kinect body structure


        void Awake()
        {
            ObjPausa = GameObject.FindGameObjectWithTag("GameComplete");
            ObjPausa.SetActive(false);
            Time.timeScale = 0;
            menu = GameObject.FindGameObjectWithTag("MenuPrincipal");
            _Sensor = KinectSensor.GetDefault(); //start
        }


        public void PlayButton()
        {
            // If there's connected Kinect to computer ...
            if (_Sensor.IsAvailable)
            {
                PlayButtonGO();
                // Opens information of calibration
                //menuCalibracao.SetActive(true);
            }
            // Otherwise ...
            else
            {
                // Go directly into a game
                ObjPausa.SetActive(true);
                Time.timeScale = 1;
                menu.SetActive(false);
            }
        }


        public void PlayButtonGO()
        {
            //menuCalibracao.SetActive(true);
            ObjPausa.SetActive(true);
            Time.timeScale = 1;
            menu.SetActive(false);
        }


        public void ExitButton()
        {
            // Exit completely from the game
            Application.Quit();
        }
    }
}