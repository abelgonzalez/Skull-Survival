///-----------------------------------------------------------------
///   Namespace:      CompleteProject
///   Class:          BodySourceManager
///   Description:    Third-party class to get information for use Kinect.
///   Author:         -                    Date: 02/06/2018
///-----------------------------------------------------------------

using UnityEngine;
using Windows.Kinect;

namespace CompleteProject
{
    public class BodySourceManager : MonoBehaviour
    {
        private KinectSensor _Sensor;
        private BodyFrameReader _Reader;
        private Body[] _Data = null;
        public bool isConected = false;

        public Body[] GetData()
        {
            return _Data;
        }


        void Start()
        {
            _Sensor = KinectSensor.GetDefault();

            if (_Sensor != null)
            {
                _Reader = _Sensor.BodyFrameSource.OpenReader();

                if (!_Sensor.IsOpen)
                {
                    _Sensor.Open();
                    isConected = true;
                }
            }
        }

        void Update()
        {
            if (_Reader != null)
            {
                var frame = _Reader.AcquireLatestFrame();
                if (frame != null)
                {
                    if (_Data == null)
                    {
                        _Data = new Body[_Sensor.BodyFrameSource.BodyCount];
                    }

                    frame.GetAndRefreshBodyData(_Data);

                    frame.Dispose();
                    frame = null;
                }
            }
        }

        void OnApplicationQuit()
        {
            if (_Reader != null)
            {
                _Reader.Dispose();
                _Reader = null;
            }

            if (_Sensor != null)
            {
                if (_Sensor.IsOpen)
                {
                    _Sensor.Close();
                    isConected = false;
                }

                _Sensor = null;
            }
        }
    }
}
