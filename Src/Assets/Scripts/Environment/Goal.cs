///-----------------------------------------------------------------
///   Namespace:      CompleteProject
///   Class:          Goal
///   Description:    Managages goals to be getted to gain the game
///   Author:         Abel                    Date: 12/05/2018
///-----------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

namespace CompleteProject
{
    public class Goal : MonoBehaviour
    {
        public Text countText;           // The current text to show the count on for the Text UI game objects
        private int count;               // The current quantity of goals avaiable for player
        public Slider ammunition;        // The slider showns the quantity of ammunition avaiable for player
        private AudioClip audioPickUp;   // Reference to the AudioClip component.
        private GameObject []goalChests;     // Reference to goal chests prefabs.
        
        // At the start of the game..
        void Start()
        {
            // Set the count to zero 
            count = 0;

            // Run the SetCountText function to update the UI (see below)
            SetCountText();

            // Set player animator.
            goalChests = GameObject.FindGameObjectsWithTag("Pick Up");
        }

        // Before rendering each frame..
        void Update()
        {
            // Rotate the game object to by 0 in the X axis, 0 in the Y axis and 20 in the Z axis,
            // multiplied by deltaTime in order to make it per second rather than per frame.
            foreach (var item in goalChests)
            {
                item.transform.Rotate(new Vector3(0, 0, 20) * Time.deltaTime);
            }
        }


        // When this game object intersects a collider with 'is trigger' checked, 
        // stores a reference to that collider in a variable named 'other'..
        void OnTriggerEnter(Collider other)
        {
            // ..and if the game object we intersect has the tag 'Pick Up' assigned to it..
            if (other.gameObject.CompareTag("Pick Up"))
            {
                //play audio clip            
                AudioSource audio = other.gameObject.GetComponent<AudioSource>();
                audio.Play();

                // Add one to the score variable 'count'
                count = count + 1;
                

                if (count < 5)
                {
                    ammunition.value += 15;
                }
                else
                {
                    ammunition.value += 20;
                }

                // Run the 'SetCountText()' function (see below)
                SetCountText();
            }
        }


        // When a player exit from the goal, mark them as inactive. Was necessary
        // put this functionality this, because if this object is marked as 
        // inactive, a sound wasn't played
        void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Pick Up"))
            {
                // Make the other game object (the pick up) inactive, to make it disappear
                other.gameObject.SetActive(false);
            }
        }

        // Create a standalone function that can update the 'countText' UI and check if the required amount to win has been achieved
        void SetCountText()
        {
            // Update the text field of our 'countText' variable
            countText.text = count.ToString() + "/10";
            // Check if our 'count' is equal to or exceeded 12             
        }
    }
}