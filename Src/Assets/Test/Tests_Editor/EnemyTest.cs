using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEditor;

namespace EnemyTest
{   
    public class EnemyHealthTest
    {
        [UnityTest]
        public IEnumerator Instantiates_GameObjectEnemy_From_Prefab_Test()
        {
            // ------------- SETUP ------------- //
            // Creating o enemy 
            var enemyPrefab = Resources.Load("Test/enemy");    
            
            // Searching o enemy com o tag 
            var spawedEnemy = GameObject.FindWithTag("Enemy");

            // Getting the prefab where this enemy is allocated in scene
            var prefabOfTheSpawnedEnemy = PrefabUtility.GetPrefabParent(spawedEnemy);

            // ------------- ASSERT TEST ------------- //
            // Checking if the loaded enemy is equal to will be spawned
            Assert.AreEqual(enemyPrefab, prefabOfTheSpawnedEnemy);

            // ------------- CLEAN UP ------------- //
            //Clean up the game scene
            foreach (var gameObject in GameObject.FindGameObjectsWithTag("Enemy"))
                Object.Destroy(gameObject);
            yield return null;

        }

    }

}
