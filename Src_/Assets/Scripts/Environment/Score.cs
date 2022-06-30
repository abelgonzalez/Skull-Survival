///-----------------------------------------------------------------
///   Namespace:      CompleteProject
///   Class:          Score
///   Description:    Managages score UI on the Scene
///   Author:         Abel                    Date: 14/05/2018
///-----------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

namespace CompleteProject
{
    public class Score : MonoBehaviour
    {
        public Text text;               // Reference to the Text component.
        public static int score;        // The player's score.
        int lastScore = -1;
        public PlayerHealth health;

        private void Start()
        {
            health = GetComponent<PlayerHealth>();
        }

        void Awake()
        {
            // Reset the score.
            score = 0;
        }

        void FixedUpdate()
        {
            // Set the displayed text to be the word "Score" followed by the score value.
            text.text = "Score: " + score;

            // For each 100 points made earns 10 on health.
            if (score > 0 && score % 100 == 0 && score != lastScore)
            {                
                health.TakeDamage(-20);
                lastScore = score;
            }
        }
    }
}