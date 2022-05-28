///-----------------------------------------------------------------
///   Namespace:      CompleteProject
///   Class:          Difficulty
///   Description:    Manage difficulty on the game
///   Author:         Abel                    Date: 19/05/2018
///-----------------------------------------------------------------

using UnityEngine;

namespace CompleteProject
{
    public class Difficulty : MonoBehaviour
    {

        // String of points where enemies will be appear on scene.
        private EnemyManager[] enemyManagers;


        void Awake()
        {
            // Use this for initialization
            // Geting up the enemyes os scene
            enemyManagers = GameObject.FindGameObjectWithTag("EnemyManager").GetComponents<EnemyManager>();
        }


        public void Facil()
        {
            // For easy game mode, during 14 or 15 seconds, enemies will be appear 
            // on 4 corners of scene
            enemyManagers[0].spawnTime = 14;
            enemyManagers[1].spawnTime = 15;
            enemyManagers[2].spawnTime = 14;
            enemyManagers[3].spawnTime = 15;
        }


        public void Medio()
        {
            // For easy game mode, during 10 or 11 seconds, enemies will be appear 
            // on 4 corners of scene.
            enemyManagers[0].spawnTime = 11;
            enemyManagers[1].spawnTime = 10;
            enemyManagers[2].spawnTime = 11;
            enemyManagers[3].spawnTime = 10;
        }


        public void Dificil()
        {
            // For easy game mode, during 3, 4, 5 or 6 seconds, enemies will be 
            // appear on 4 corners of scene. On this case was necessary make an 
            // "progressive" because render will be chaotic on computers with 
            // limited performance.
            enemyManagers[0].spawnTime = 3;
            enemyManagers[1].spawnTime = 4;
            enemyManagers[2].spawnTime = 5;
            enemyManagers[3].spawnTime = 6;
        }


        public void Imposible()
        {
            // For easy game mode, during 1 or 2 seconds, enemies will be 
            // appear on 4 corners of scene. On this case was necessary make an 
            // "progressive" because render will be chaotic on computers with 
            // limited performance.
            enemyManagers[0].spawnTime = 1;
            enemyManagers[1].spawnTime = 2;
            enemyManagers[2].spawnTime = 1;
            enemyManagers[3].spawnTime = 2;
        }
    }
}