using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine;

namespace EnvironmentTest
{
    public class SceneTest
    {
        [UnityTest]
        public IEnumerator Scene_Loading_Test()
        {
            // ------------- SETUP ------------- //
            // Store the test scene in a temp variable
            Scene testScene = SceneManager.GetActiveScene();

            // Load the game scene you want to use
            yield return SceneManager.LoadSceneAsync("MainGame", LoadSceneMode.Additive);

            // After it is loaded, set the scene as Active
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("MainGame"));


            // ------------- ASSERT TEST ------------- //
            // Assert that the game scene has been set to active. If not, an exception will be thrown
            Assert.IsTrue(SceneManager.GetActiveScene().name == "MainGame");


            // ------------- CLEAN UP ------------- //
            //Set the active scene back to the test scene to cloe the test
            SceneManager.SetActiveScene(testScene);

            //Clean up the game scene
            yield return SceneManager.UnloadSceneAsync("MainGame");
        }

        [UnityTest]
        public IEnumerator GameObject_WithRigidBody_WillBeAffectedByPhysics()
        {

            // ------------- SETUP ------------- //
            // Creating GameObject to be tested
            var go = new GameObject();

            // Adding Rigidbody component
            go.AddComponent<Rigidbody>();

            // Changing position 
            var originalPosition = go.transform.position.y;

            // ------------- ASSERT TEST ------------- //
            // Waiting for execution FixedUpdate
            yield return new WaitForFixedUpdate();   
            
            // Checking if are not equal a original position with other one
            Assert.AreNotEqual(originalPosition, go.transform.position.y);

            // ------------- CLEAN UP ------------- //
            // Finding all GameObjects from Scene test 
            GameObject[] allObjects = Object.FindObjectsOfType<GameObject>();

            //Clean up the game scene
            foreach (var obj in allObjects)            
                Object.Destroy(obj);            
            yield return null;

        }
    }

    /*
    public class PlayerTest
    {
        [UnityTest]
        public IEnumerator PlayerPrefabTest()
        {

            // ------------- SETUP ------------- //
            // SET UP THE GAME SCENE YOU WANT TO TEST
            // Store the test scene in a temp variable
            Scene testScene = SceneManager.GetActiveScene();

            // Load the game scene you want to use
            yield return SceneManager.LoadSceneAsync("MainGame", LoadSceneMode.Additive);

            // Wait until the asynchronous scene fully loads
            LoadYourAsyncScene();
            
            // After it is loaded, set the scene as Active
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("MainGame"));


            // ------------- ASSERT TEST ------------- //
            // TEST THE PLAYER PREFAB
            // Store the test player in a temp variable
            GameObject testPlayer = GameObject.FindGameObjectWithTag("Player");

            // Assert that the player game object is not null. If is it, an exception will be thrown
            Assert.IsTrue(testPlayer != null);


            // ------------- CLEAN UP ------------- //
            //Set the active scene back to the test scene to close the test
            SceneManager.SetActiveScene(testScene);

            //Clean up the game scene
            yield return SceneManager.UnloadSceneAsync("MainGame");
        }

        IEnumerator LoadYourAsyncScene()
        {
            // The Application loads the Scene in the background as the current Scene runs.
            // This is particularly good for creating loading screens.
            // You could also load the Scene by using sceneBuildIndex. In this case MainGame has
            // a sceneBuildIndex of 1 as shown in Build Settings.

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MainGame");

            // Wait until the asynchronous scene fully loads
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }
        */
}
