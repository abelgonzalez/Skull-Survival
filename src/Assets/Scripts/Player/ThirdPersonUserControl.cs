///-----------------------------------------------------------------
///   Namespace:      CompleteProject
///   Class:          ThirdPersonUserControl
///   Description:    Third-party class to get information for use Kinect. Several modifications was needed.
///   Author:         -                    Date: 04/06/2018
///-----------------------------------------------------------------

using UnityEngine;
using Windows.Kinect;
using UnityEngine.UI;

namespace CompleteProject
{
    [RequireComponent(typeof(ThirdPersonCharacter))]    
    public class ThirdPersonUserControl : MonoBehaviour
    {
        public GameObject BodySrcManager;
        public Text feedback;
        public PlayerShooting shoot;
        public PlayerHealth health;
        public PauseMenu pausa;
        public bool balance = false;

        private int numCalibrationFrames = 200;
        private int count = 0;
        private int countMean = 0;

        private BodySourceManager bodyManager;
        private Body[] bodies;

        private Vector3 center;                 // Calibration gets these parameters to calculate movement range
        private Vector3 max_range;              // and sensibility.
        private Vector3 min_range;

        private float[] xValues = new float[200];
        private float[] zValues = new float[200];
        private float[] yValues = new float[200];

        private Vector3[] calRightAnkle = new Vector3[200];
        private Vector3[] calLeftAnkle = new Vector3[200];
        Vector3 leftAnkle = Vector3.zero;
        Vector3 rightAnkle = Vector3.zero;
        float stdDiffx;
        float maxStableDiffX;           // [balance] Max horizontal different between ankles.

        float supDiffx;
        float infDiffx;

        float supDiffz;
        float infDiffz;

        private int CalibrationPhase = 0;

        float fireTimer;
        float timeBetweenShoots = 0.125f;

        private bool crouch = false;
        private bool normalYState = true;
        private float crouchYvalue = 0f;
        private float timerCrouch = 0f;
        private float timeForCrouch = 0.5f;
        private float timeForJump = 0.3f;

        private string time = "00:00";
        private Vector3 rightKnee = Vector3.zero;
        private Vector3 leftKnee = Vector3.zero;

        private int kick = 0;               // 0 (false), 1 (right), 2 (left)
        private bool isKicking = false;

        private int punch = 0;              // 0 (false), 1 (right), 2 (left)
        private bool isPunching = false;

        float rightWrist = 0;
        float leftWrist = 0;

        float headHeight = 0;

        bool calibration = false;

        private ThirdPersonCharacter m_Character;
        private Transform m_Cam;                  // A reference to the main camera in the scenes transform
        private Vector3 m_CamForward;             // The current forward direction of the camera
        private Rigidbody playerRigidbody;
        private Vector3 m_Move;
        private bool m_Jump;                    // the world-relative desired move direction, calculated from the camForward and user input.
        float camRayLength = 100f;              // The length of the ray from the camera into the scene.
        int floorMask = 0;                        // A layer mask so that a ray can be cast just at gameobjects on the floor layer.


        private void Start()
        {
            // get kinect body source manager.
            if (BodySrcManager == null)
            {
                Debug.Log("Assign Game Object with [kinect] Body Source Manager.");
            }
            else
            {
                bodyManager = BodySrcManager.GetComponent<BodySourceManager>();
            }

            // get the transform of the main camera
            if (Camera.main != null)
            {
                m_Cam = Camera.main.transform;
            }
            else
            {
                Debug.LogWarning(
                    "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.", gameObject);
                // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
            }

            // get the third person character ( this should never be null due to require component )
            m_Character = GetComponent<ThirdPersonCharacter>();

            min_range.x = 100f;
            min_range.z = 100f;
            min_range.y = 100f;

            max_range.x = -100f;
            max_range.z = -100f;
            max_range.y = -100f;

            CalibrationPhase = 0;
            count = 0;
            
            feedback.text = "";
        }

        private void Update()
        {
            if (!m_Jump)
            {
                m_Jump = Input.GetButtonDown("Jump");
            }
        }


        // Fixed update is called in sync with physics
        private void FixedUpdate()
        {
            Vector3 position;

            // read inputs
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            // Debug.Log("X " + x + " Z" + z);

            crouch = Input.GetKey(KeyCode.C);

            if (Input.GetKey(KeyCode.Z))
            {
                punch = 1;
            }
            else if (Input.GetKey(KeyCode.X))
            {
                punch = 2;
            }
            else
            {
                punch = 0;
            }

            if (punch == 0)
            {
                isPunching = false;
            }
            else
            {
                isPunching = true;
            }

            if (Input.GetKey(KeyCode.V))
            {
                kick = 1;
            }
            else if (Input.GetKey(KeyCode.B))
            {
                kick = 2;
            }
            else
            {
                kick = 0;
            }

            if (kick == 0)
            {
                isKicking = false;
            }
            else
            {
                isKicking = true;
            }

            bodies = bodyManager.GetData();


            if (bodies != null)
            {
                foreach (var body in bodies)
                {
                    if (body != null)
                    {
                        if (body.IsTracked)
                        {
                            if (body.Joints[JointType.Head].TrackingState == TrackingState.Tracked &&
                                body.Joints[JointType.ShoulderRight].TrackingState == TrackingState.Tracked &&
                                body.Joints[JointType.KneeLeft].TrackingState == TrackingState.Tracked &&
                                body.Joints[JointType.KneeRight].TrackingState == TrackingState.Tracked &&
                                body.Joints[JointType.WristLeft].TrackingState == TrackingState.Tracked &&
                                body.Joints[JointType.WristRight].TrackingState == TrackingState.Tracked
                                )
                            {
                                var pos = body.Joints[JointType.Head].Position;
                                var posX = body.Joints[JointType.Head].Position;
                                var posY = body.Joints[JointType.SpineBase].Position;

                                var posLKnee = body.Joints[JointType.KneeLeft].Position;
                                var posRKnee = body.Joints[JointType.KneeRight].Position;
                                var posLWrist = body.Joints[JointType.WristLeft].Position;
                                var posRWrist = body.Joints[JointType.WristRight].Position;

                                var posRAnkle = body.Joints[JointType.AnkleRight].Position;
                                var posLAnkle = body.Joints[JointType.AnkleLeft].Position;

                                switch (CalibrationPhase)
                                {
                                    case 0:
                                        calibration = true;
                                        feedback.text = "Mean Cal. " + count;

                                        if (count < numCalibrationFrames)
                                        {
                                            count++;

                                            xValues[countMean] = posX.X;
                                            zValues[countMean] = pos.Z;
                                            yValues[countMean] = posY.Y;

                                            calRightAnkle[countMean].x = posRAnkle.X;
                                            calRightAnkle[countMean].y = posRAnkle.Y;
                                            calRightAnkle[countMean].z = posRAnkle.Z;

                                            calLeftAnkle[countMean].x = posLAnkle.X;
                                            calLeftAnkle[countMean].y = posLAnkle.Y;
                                            calLeftAnkle[countMean].z = posLAnkle.Z;

                                            rightKnee.z = posRKnee.Z;
                                            leftKnee.z = posLKnee.Z;

                                            rightWrist = posRWrist.Z;
                                            leftWrist = posLWrist.Z;

                                            headHeight = pos.Y;

                                            // Debug.Log("pos.z calibration: " + pos.Z);
                                            countMean++;
                                        }
                                        else
                                        {
                                            CalibrationPhase++;
                                        }
                                        break;
                                    case 1:
                                        center.x = getMean(xValues);
                                        center.z = getMean(zValues);
                                        center.y = getMean(yValues);

                                        rightAnkle = getMeanVector(calRightAnkle);
                                        leftAnkle = getMeanVector(calLeftAnkle);

                                        stdDiffx = Mathf.Abs(Mathf.Abs(rightAnkle.x) - Mathf.Abs(leftAnkle.x));
                                        maxStableDiffX = stdDiffx * 0.35f;

                                        feedback.text = "Fwd Calibrat. " + count;

                                        if (count < numCalibrationFrames * 2)
                                        {
                                            position.x = posX.X;
                                            position.z = pos.Z;
                                            position.y = posY.Y;
                                            UpdateMaxMinRange(position);
                                            count++;
                                        }
                                        else
                                        {
                                            CalibrationPhase++;
                                        }
                                        break;

                                    case 2:
                                        feedback.text = "Left Cal. " + count;

                                        if (count < numCalibrationFrames * 3)
                                        {
                                            position.x = posX.X;
                                            position.z = pos.Z;
                                            position.y = posY.Y;

                                            UpdateMaxMinRange(position);
                                            count++;
                                        }
                                        else
                                        {
                                            CalibrationPhase++;
                                        }
                                        break;
                                    case 3:
                                        feedback.text = "Right Cal. " + count;

                                        if (count < numCalibrationFrames * 4)
                                        {
                                            position.x = posX.X;
                                            position.z = pos.Z;
                                            position.y = posY.Y;
                                            UpdateMaxMinRange(position);
                                            count++;
                                        }
                                        else
                                        {
                                            CalibrationPhase++;

                                            supDiffx = (max_range.x - center.x) * 100;
                                            infDiffx = (center.x - min_range.x) * 100;

                                            supDiffz = (max_range.z - center.z);
                                            infDiffz = (center.z - min_range.z);

                                            crouchYvalue = center.y - 0.1f;

                                            Debug.Log("Center Left-Right     " + center.x + "  Forward " + center.z + " Height " + center.y
                                                + "\nRight  " + supDiffx + " Left  " + infDiffx + "\n\nForward " + infDiffz + "\nCrounch : "
                                                + crouchYvalue);
                                        }

                                        break;
                                    case 4:
                                        calibration = false;

                                        x = DiscoverX(posX.X, supDiffx, infDiffx);
                                        z = DiscoverZ(pos.Z, supDiffz, infDiffz);
                                        AnalyzeY(posY.Y);
                                        // Debug.Log(" POS Y " + posY.Y);
                                        // Debug.Log("X " + x + " Z " + z);

                                        fireTimer += Time.deltaTime;

                                        checkPause(posRWrist.Y, posLWrist.Y);
                                        checkPunch(posRWrist.Z, posLWrist.Z);
                                        checkKick(posRKnee.Z, posLKnee.Z, posRAnkle.Y, posLAnkle.Y);
                                        CheckTrigger(body);

                                        if (balance && checkDropped(posRAnkle.X, posRAnkle.Y, posLAnkle.X, posLAnkle.Y))
                                        {
                                            health.healthSlider.value = 0;
                                        }

                                        if (Time.time % 10 == 0)
                                        {
                                            string min = Mathf.Floor(Time.time / 60).ToString("00");
                                            string sec = (Time.time % 60).ToString("00");
                                            time = min + ":" + sec;
                                            // Debug.Log(time);
                                        }
                                        break;
                                }
                            }
                        }
                    }
                }
            }

            // calculate move direction to pass to character
            if (m_Cam != null)
            {
                // calculate camera relative direction to move:
                m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
                m_Move = z * m_CamForward + x * m_Cam.right;
            }
            else
            {
                // we use world-relative directions in the case of no main camera
                m_Move = z * Vector3.forward + x * Vector3.right;
            }
#if !MOBILE_INPUT
            // walk speed multiplier
            if (Input.GetKey(KeyCode.LeftShift)) m_Move *= 0.5f;
#endif

            // pass all parameters to the character control script
            m_Character.Move(m_Move, crouch, m_Jump, kick, punch);
            m_Jump = false;
        }

        //
        // During calibration get range of movement in the three dimensions.
        //
        void UpdateMaxMinRange(Vector3 pos)
        {
            if (pos.x > max_range.x) max_range.x = pos.x;
            if (pos.z > max_range.z) max_range.z = pos.z;
            if (pos.y > max_range.y) max_range.y = pos.y;

            if (pos.x < min_range.x) min_range.x = pos.x;
            if (pos.z < min_range.z) min_range.z = pos.z;
            if (pos.y < min_range.y) min_range.y = pos.y;
        }

        //
        //
        bool checkDropped(float rAnkleX, float rAnkleY, float lAnkleX, float lAnkleY)
        {
            float diffX = Mathf.Abs(Mathf.Abs(rAnkleX) - Mathf.Abs(lAnkleX));
            //Debug.Log(" diff x " + Mathf.Abs(Mathf.Abs(rAnkleX) - Mathf.Abs(rightAnkle.x)));
            //Debug.Log(" diff y " + Mathf.Abs(Mathf.Abs(rAnkleY) - Mathf.Abs(rightAnkle.y)));
            //Debug.Log(" diff between " + Mathf.Abs(stdDiffx - diffX) + " bound " +  maxStableDiffX);

            // If the distance between ankles is greater than 20% and
            // If the difference of the current position of right ankle and its std position in 
            // horizontal/vertical axis is greater than 0.15 and 0.02 or the same with the left
            // ankle, it seems that the user has dropped down.
            if (Mathf.Abs(stdDiffx - diffX) > maxStableDiffX &&
               (Mathf.Abs(Mathf.Abs(rAnkleY) - Mathf.Abs(rightAnkle.y)) > 0.02f ||
               Mathf.Abs(Mathf.Abs(lAnkleY) - Mathf.Abs(leftAnkle.y)) > 0.02f))
            {
                feedback.text = "Dropped down !!!";
                return true;
            }

            return false;
        }

        Vector3 getMeanVector(Vector3[] vector)
        {
            float sumX = 0, sumY = 0, sumZ = 0;
            int countMeanX = 0;
            int countMeanY = 0;
            int countMeanZ = 0;
            Vector3 mean = Vector3.zero;

            for (int i = 0; i < vector.Length; i++)
            {
                if (vector[i].x < 0 || vector[i].x > 0)
                {
                    sumX += vector[i].x;
                    countMeanX++;
                }
                if (vector[i].y < 0 || vector[i].y > 0)
                {
                    sumY += vector[i].y;
                    countMeanY++;
                }
                if (vector[i].z < 0 || vector[i].z > 0)
                {
                    sumZ += vector[i].z;
                    countMeanZ++;
                }
            }

            mean.x = sumX / countMeanX;
            mean.y = sumY / countMeanY;
            mean.z = sumZ / countMeanZ;

            return (mean);
        }


        //
        //
        float getMean(float[] array)
        {
            float sum = 0;
            int countMean = 0;

            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] < 0 || array[i] > 0)
                {
                    sum += array[i];
                    countMean++;
                }
            }

            return (sum / countMean);
        }

        // Turn left or Right.
        //
        float DiscoverX(float x, float supDiff, float infDiff)
        {
            float diff = x - center.x;
            // float percent;
            diff = diff * 100;

            if (Mathf.Abs(diff) <= ((supDiff + infDiff) * 0.1f))
            {
                x = 0;
                feedback.text = time + " Neutral";
                // percent = (diff / (supDiff + infDiff)) * 100;
                // Debug.Log("Neutral X: " + diff + " % " + percent);
            }
            else
            {
                if (diff > 0)
                {
                    feedback.text = time + " Right";
                    // percent = (diff / supDiff) * 100;

                    if (diff >= (supDiff * 0.9f)) x = 50 * Time.deltaTime;
                    else if (diff >= (supDiff * 0.6f)) x = 40 * Time.deltaTime;
                    else if (diff >= (supDiff * 0.3f)) x = 25 * Time.deltaTime;
                    else if (diff >= (supDiff * 0.2f)) x = 15 * Time.deltaTime;
                    else x = 5 * Time.deltaTime;

                    // Debug.Log("Right X % " + Mathf.Round(percent) + " x " + x);
                }
                else
                {
                    feedback.text = time + " Left";
                    // percent = (diff / infDiff) * 100;

                    if (diff < -(infDiff * 0.90f)) x = -50 * Time.deltaTime;
                    else if (diff < -(infDiff * 0.6f)) x = -40 * Time.deltaTime;
                    else if (diff < -(infDiff * 0.3f)) x = -25 * Time.deltaTime;
                    else if (diff < -(infDiff * 0.2f)) x = -15 * Time.deltaTime;
                    else x = -5f * Time.deltaTime;

                    // Debug.Log("Left X: % " + Mathf.Round(percent) + " x " + x);
                }
            }

            return x;
        }

        // Goes forward. Setup avatar speed.
        // Only going forward, I will try to fix it later on.
        float DiscoverZ(float z, float supDiff, float infDiff)
        {
            float diff = center.z - z;
            //float percent;

            if (Mathf.Abs(diff) <= (infDiff + Mathf.Abs(supDiffz)) * 0.1f)
            {
                z = 0;
            }
            else
            {
                if (diff > 0)
                {
                   // percent = (diff / infDiff) * 100;

                    if (diff > (infDiff * 0.75f)) z = 60 * Time.deltaTime;
                    else if (diff > (infDiff * 0.5f)) z = 40 * Time.deltaTime;
                    else if (diff > (infDiff * 0.25f)) z = 25 * Time.deltaTime;
                    else z = 10 * Time.deltaTime;

                    // Debug.Log("Diff Z " + diff + " % " + percent + "z  " + z);
                }
                else
                {
                    //percent = (diff / supDiff) * 100;
                    diff = Mathf.Abs(diff);

                    if (diff > (supDiff * 0.75f)) z = -60 * Time.deltaTime;
                    else if (diff > (supDiff * 0.5f)) z = -40 * Time.deltaTime;
                    else if (diff > (supDiff * 0.25f)) z = -25 * Time.deltaTime;
                    else z = -10 * Time.deltaTime;

                    // Debug.Log("Diff Z " + diff + " % " + percent + "z  " + z);
                }
            }

            return z;
        }

        // Gets height differences to see if crouch and/or jump.
        //
        void AnalyzeY(float y)
        {
            if (y < crouchYvalue)
            {
                if (normalYState)
                {
                    timerCrouch = Time.time;
                    timeForCrouch += timerCrouch;
                    timeForJump += timerCrouch;
                }
                else
                {
                    if (timerCrouch > timeForCrouch)
                    {
                        crouch = true;
                        feedback.text = time + " Crouch !!!";
                    }
                }
                timerCrouch += Time.deltaTime;
                normalYState = false;
            }
            else
            {
                if (!normalYState && timerCrouch < timeForJump)
                {
                    m_Jump = true;
                    feedback.text = time + " Jump !!!";
                }
                else
                {
                    if (timerCrouch > timeForJump)
                    {
                        m_Jump = false;
                        timeForJump = 0.5f;
                        timerCrouch = 1;            // must be greater than timeForJump. It it updated when Y is low enough.
                    }
                    else
                    {
                        timerCrouch += Time.deltaTime;
                    }
                }

                timeForCrouch = 0.5f;
                crouch = false;
                normalYState = true;
            }
        }

        void checkKick(float rKnee, float lKnee, float rAnkleY, float lAnkleY)
        {
            //Debug.Log("R STD z " + rightKnee.z + " cur z " + rKnee);
            //Debug.Log("L STD z " + leftKnee.z + " cur z " + lKnee);

            // Can't kick if crouching or jumping.
            if (!crouch && !m_Jump)
            {
                float diffR = Mathf.Abs(rAnkleY) - Mathf.Abs(rightAnkle.y);
                float diffL = Mathf.Abs(lAnkleY) - Mathf.Abs(leftAnkle.y);

                if ((rightKnee.z - rKnee) > 0.2f && diffR > 0.1f)
                {
                    kick = 1;
                    feedback.text = "kick right";
                }
                else if ((leftKnee.z - lKnee) > 0.2f && diffL > 0.1f)
                {
                    kick = 2;
                    feedback.text = "kick left";
                }
                else
                {
                    kick = 0;
                }
            }
            else
            {
                kick = 0;
            }

            if (kick == 0)
            {
                isKicking = false;
            }
            else
            {
                isKicking = true;
            }
        }


        void checkPause(float rWrist, float lWrist)
        {
            // If any wrist is above head, pauses the game.
            if (rWrist > headHeight || lWrist > headHeight)
            {
                pausa.DoPause();
            }
        }


        void checkPunch(float rWrist, float lWrist)
        {

            if ((rightWrist - rWrist) > 0.35f)
            {
                punch = 1;
                feedback.text = "punch right";
            }
            else if ((leftWrist - lWrist) > 0.35f)
            {
                punch = 2;
                feedback.text = "punch left";
            }
            else
            {
                punch = 0;
            }


            if (punch == 0)
            {
                isPunching = false;
            }
            else
            {
                isPunching = true;
            }
        }



        //
        // Trigger the gun if one or both hands are closed.
        //
        void CheckTrigger(Body body)
        {
            if ((body.HandRightState == HandState.Closed || body.HandLeftState == HandState.Closed) &&
                (punch == 0 && kick == 0))
            {
                // Debug.Log("Trigger after hand test" + fireTimer + " tbs " + timeBetweenShoots);
                if (fireTimer > timeBetweenShoots)
                {
                    // Debug.Log("Shooting .... ");
                    feedback.text = "Shooting !";
                    shoot.Shoot();
                    fireTimer = 0;
                }
            }
        }

        public bool playerPunching()
        {
            return isPunching;
        }

        public bool playerKicking()
        {
            return isKicking;
        }


        public bool playerCalibration()
        {
            return calibration;
        }

        void Turning()
        {
#if !MOBILE_INPUT
            // Create a ray from the mouse cursor on screen in the direction of the camera.
            Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Create a RaycastHit variable to store information about what was hit by the ray.
            RaycastHit floorHit;

            // Perform the raycast and if it hits something on the floor layer...
            if (Physics.Raycast(camRay, out floorHit, camRayLength, floorMask))
            {
                // Create a vector from the player to the point on the floor the raycast from the mouse hit.
                Vector3 playerToMouse = floorHit.point - transform.position;

                // Ensure the vector is entirely along the floor plane.
                playerToMouse.y = 0f;

                // Create a quaternion (rotation) based on looking down the vector from the player to the mouse.
                Quaternion newRotatation = Quaternion.LookRotation(playerToMouse);

                // Set the player's rotation to this new rotation.
                playerRigidbody.MoveRotation(newRotatation);
            }
#endif
        }
    }
}