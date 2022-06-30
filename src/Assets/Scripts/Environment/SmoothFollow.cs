///-----------------------------------------------------------------
///   Namespace:      CompleteProject
///   Class:          SmoothFollow
///   Description:    Managages camera direction, seeking the player.
///   Author:         Abel                    Date: 16/05/2018
///-----------------------------------------------------------------

using UnityEngine;

namespace CompleteProject
{
    public class SmoothFollow : MonoBehaviour
    {        
        [SerializeField]
        private Transform target;       // The target we are following
        
        [SerializeField]
        private float distance = 10.0f; // The distance in the x-z plane to the target
       
        [SerializeField]
        private float height = 5.0f;    // the height we want the camera to be above the target

        [SerializeField]
        private float rotationDamping;

        [SerializeField]
        private float heightDamping;


        // LateUpdate is called once per frame
        void LateUpdate()
        {
            // Early out if we don't have a target...
            if (!target)
                return;

            // Calculate the current rotation angles
            float wantedRotationAngle;
            float wantedHeight;

            float currentRotationAngle;
            float currentHeight;
            float dist;

            // When player gets to close to the boundaries (rock walls) it changes
            // camera views to avoid looking through the rocks.
            if (Mathf.Abs(target.position.x) > 43 || Mathf.Abs(target.position.z) > 41.5)
            {
                // Calculate the current rotation angles
                wantedRotationAngle = target.eulerAngles.y;
                wantedHeight = target.position.y + 1.7f;

                // Debug.Log("Euler angle y " + target.eulerAngles.y);

                currentRotationAngle = transform.eulerAngles.y;
                currentHeight = transform.position.y;
                dist = 2;
            }
            // Otherwise ...
            else
            {
                // Calculate the current rotation angles
                wantedRotationAngle = target.eulerAngles.y;
                wantedHeight = target.position.y + height;

                currentRotationAngle = transform.eulerAngles.y;
                currentHeight = transform.position.y;
                dist = distance;
            }

            // Damp the rotation around the y-axis
            currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);

            // Damp the height
            currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

            // Convert the angle into a rotation
            var currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

            // Set the position of the camera on the x-z plane to:
            // distance meters behind the target
            transform.position = target.position;
            transform.position -= currentRotation * Vector3.forward * dist;

            // Set the height of the camera
            transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);

            // Always look at the target
            transform.LookAt(target);
        }
    }
}