///-----------------------------------------------------------------
///   Namespace:      CompleteProject
///   Class:          Graphics
///   Description:    Manage graphics on the game
///   Author:         Abel                    Date: 20/05/2018
///-----------------------------------------------------------------

using UnityEngine;


namespace CompleteProject
{
    public class Graphics : MonoBehaviour
    {
        private enum QualityLevel
        {
            Fastest,     // 0
            Fast,        // 1
            Simple,      // 2
            Good,        // 3
            Beautiful,   // 4
            Fantastic    // 5
        }


        public void Graphics1()
        {
            QualitySettings.SetQualityLevel(0);
        }


        public void Graphics2()
        {
            QualitySettings.SetQualityLevel(1);
        }


        public void Graphics3()
        {
            QualitySettings.SetQualityLevel(2);
        }


        public void Graphics4()
        {
            QualitySettings.SetQualityLevel(3);
        }


        public void Graphics5()
        {
            QualitySettings.SetQualityLevel(4);
        }


        public void Graphics6()
        {
            QualitySettings.SetQualityLevel(5);
        }

    }
}