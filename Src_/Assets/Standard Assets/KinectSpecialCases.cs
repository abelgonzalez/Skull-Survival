using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Windows.Kinect
{
    [StructLayout(LayoutKind.Sequential)]
    public struct PointF
    {
        public float X { get; set; }
        public float Y { get; set; }
        
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is PointF))
            {
                return false;
            }
            return this.Equals((ColorSpacePoint)obj);
        }

        public bool Equals(ColorSpacePoint obj)
        {
            return (X == obj.X) && (Y == obj.Y);
        }

        public static bool operator ==(PointF a, PointF b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(PointF a, PointF b)
        {
            return !(a.Equals(b));
        }
    }

    public sealed partial class AudioBeamSubFrame
    {
        [DllImport(
            "KinectUnityAddin",
            EntryPoint = "Windows_Kinect_AudioBeamSubFrame_CopyFrameDataToArray",
            CallingConvention = CallingConvention.Cdecl,
            SetLastError = true)]
        private static extern void Windows_Kinect_AudioBeamSubFrame_CopyFrameDataToIntPtr(IntPtr pNative, IntPtr frameData, uint frameDataSize);

        public void CopyFrameDataToIntPtr(IntPtr frameData, uint size)
        {
            if (_pNative == IntPtr.Zero)
            {
                throw new ObjectDisposedException("AudioBeamSubFrame");
            }

            Windows_Kinect_AudioBeamSubFrame_CopyFrameDataToIntPtr(_pNative, frameData, size);
            Helper.ExceptionHelper.CheckLastError();
        }

        [DllImport("KinectUnityAddin", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern IntPtr Windows_Kinect_AudioBeamSubFrame_LockAudioBuffer(IntPtr pNative);

        public KinectBuffer LockAudioBuffer()
        {
            if (_pNative == IntPtr.Zero)
            {
                throw new ObjectDisposedException("AudioBeamSubFrame");
            }

            IntPtr objectPointer = Windows_Kinect_AudioBeamSubFrame_LockAudioBuffer(_pNative);
            Helper.ExceptionHelper.CheckLastError();

            if (objectPointer == IntPtr.Zero)
            {
                return null;
            }
            return Helper.NativeObjectCache.CreateOrGetObject<KinectBuffer>(objectPointer, n => new KinectBuffer(n));
        }
    }

    public sealed partial class AudioBeamFrame
    {
        private AudioBeamSubFrame[] _subFrames = null;

        private void Dispose(bool disposing)
        {
            if (_pNative == IntPtr.Zero)
            {
                return;
            }

            if (_subFrames != null)
            {
                foreach (var subFrame in _subFrames)
                {
                    subFrame.Dispose();
                }
                _subFrames = null;
            }

            __EventCleanup();

            Helper.NativeObjectCache.RemoveObject<AudioBeamFrame>(_pNative);
            Windows_Kinect_AudioBeamFrame_ReleaseObject(ref _pNative);

            if (disposing)
            {
                Windows_Kinect_AudioBeamFrame_Dispose(_pNative);
            }
            _pNative = IntPtr.Zero;
        }

        [DllImport("KinectUnityAddin", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Windows_Kinect_AudioBeamFrame_Dispose(IntPtr pNative);
        public void Dispose()
        {
            if (_pNative == IntPtr.Zero)
            {
                return;
            }

            Dispose(true);
            GC.SuppressFinalize(this);
        }

        [DllImport("KinectUnityAddin", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern int Windows_Kinect_AudioBeamFrame_get_SubFrames(IntPtr pNative, [Out] IntPtr[] outCollection, int outCollectionSize);
        [DllImport("KinectUnityAddin", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Windows_Kinect_AudioBeamFrame_get_SubFrames_Length(IntPtr pNative);
        public IList<AudioBeamSubFrame> SubFrames
        {
            get
            {
                if (_pNative == IntPtr.Zero)
                {
                    throw new ObjectDisposedException("AudioBeamFrame");
                }

                if (_subFrames == null)
                {
                    int collectionSize = Windows_Kinect_AudioBeamFrame_get_SubFrames_Length(_pNative);
                    var outCollection = new IntPtr[collectionSize];
                    _subFrames = new AudioBeamSubFrame[collectionSize];

                    collectionSize = Windows_Kinect_AudioBeamFrame_get_SubFrames(_pNative, outCollection, collectionSize);
                    Helper.ExceptionHelper.CheckLastError();

                    for (int i = 0; i < collectionSize; i++)
                    {
                        if (outCollection[i] == IntPtr.Zero)
                        {
                            continue;
                        }

                        var obj = Helper.NativeObjectCache.GetObject<AudioBeamSubFrame>(outCollection[i]);
                        if (obj == null)
                        {
                            obj = new AudioBeamSubFrame(outCollection[i]);
                            Helper.NativeObjectCache.AddObject(outCollection[i], obj);
                        }

                        _subFrames[i] = obj;
                    }
                }

                return _subFrames;
            }
        }
    }

    public sealed partial class BodyFrame
    {
        [DllImport("KinectUnityAddin", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern void Windows_Kinect_BodyFrame_GetAndRefreshBodyData(IntPtr pNative, [Out] IntPtr[] bodies, int bodiesSize);
        public void GetAndRefreshBodyData(IList<Body> bodies)
        {
            if (_pNative == IntPtr.Zero)
            {
                throw new ObjectDisposedException("BodyFrame");
            }

            int _bodies_idx = 0;
            var _bodies = new IntPtr[bodies.Count];
            for (int i = 0; i < bodies.Count; i++)
            {
                if (bodies[i] == null)
                {
                    bodies[i] = new Body();
                }

                _bodies[_bodies_idx] = bodies[i].GetIntPtr();
                _bodies_idx++;
            }

            Windows_Kinect_BodyFrame_GetAndRefreshBodyData(_pNative, _bodies, bodies.Count);
            Helper.ExceptionHelper.CheckLastError();

            for (int i = 0; i < bodies.Count; i++)
            {
                bodies[i].SetIntPtr(_bodies[i]);
            }
        }
    }

    public sealed partial class Body
    {
        internal void SetIntPtr(IntPtr value) { _pNative = value; }
        internal IntPtr GetIntPtr() { return _pNative; }

        internal Body() { }

        [DllImport("KinectUnityAddin", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern IntPtr Windows_Kinect_Body_get_Lean(IntPtr pNative);
        public PointF Lean
        {
            get
            {
                if (_pNative == IntPtr.Zero)
                {
                    throw new ObjectDisposedException("Body");
                }

                var objectPointer = Windows_Kinect_Body_get_Lean(_pNative);
                Helper.ExceptionHelper.CheckLastError();

                var obj = (PointF)Marshal.PtrToStructure(objectPointer, typeof(PointF));
                Marshal.FreeHGlobal(objectPointer);
                return obj;
            }
        }
    }

    public sealed partial class ColorFrame
    {
        [DllImport(
            "KinectUnityAddin",
            EntryPoint = "Windows_Kinect_ColorFrame_CopyRawFrameDataToArray",
            CallingConvention = CallingConvention.Cdecl,
            SetLastError = true)]
        private static extern void Windows_Kinect_ColorFrame_CopyRawFrameDataToIntPtr(IntPtr pNative, IntPtr frameData, uint frameDataSize);
        public void CopyRawFrameDataToIntPtr(IntPtr frameData, uint size)
        {
            if (_pNative == IntPtr.Zero)
            {
                throw new ObjectDisposedException("ColorFrame");
            }

            Windows_Kinect_ColorFrame_CopyRawFrameDataToIntPtr(_pNative, frameData, size);
            Helper.ExceptionHelper.CheckLastError();
        }

        [DllImport(
            "KinectUnityAddin",
            EntryPoint = "Windows_Kinect_ColorFrame_CopyConvertedFrameDataToArray",
            CallingConvention = CallingConvention.Cdecl,
            SetLastError = true)]
        private static extern void Windows_Kinect_ColorFrame_CopyConvertedFrameDataToIntPtr(IntPtr pNative, IntPtr frameData, uint frameDataSize, ColorImageFormat colorFormat);
        public void CopyConvertedFrameDataToIntPtr(IntPtr frameData, uint size, ColorImageFormat colorFormat)
        {
            if (_pNative == IntPtr.Zero)
            {
                throw new ObjectDisposedException("ColorFrame");
            }

            Windows_Kinect_ColorFrame_CopyConvertedFrameDataToIntPtr(_pNative, frameData, size, colorFormat);
            Helper.ExceptionHelper.CheckLastError();
        }

        [DllImport("KinectUnityAddin", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern IntPtr Windows_Kinect_ColorFrame_LockRawImageBuffer(IntPtr pNative);
        public KinectBuffer LockRawImageBuffer()
        {
            if (_pNative == IntPtr.Zero)
            {
                throw new ObjectDisposedException("ColorFrame");
            }

            IntPtr objectPointer = Windows_Kinect_ColorFrame_LockRawImageBuffer(_pNative);
            Helper.ExceptionHelper.CheckLastError();

            if (objectPointer == IntPtr.Zero)
            {
                return null;
            }

            return Helper.NativeObjectCache.CreateOrGetObject<KinectBuffer>(objectPointer, n => new KinectBuffer(n));
        }

    }

    public sealed partial class DepthFrame
    {
        [DllImport(
            "KinectUnityAddin",
            EntryPoint = "Windows_Kinect_DepthFrame_CopyFrameDataToArray",
            CallingConvention = CallingConvention.Cdecl,
            SetLastError = true)]
        private static extern void Windows_Kinect_DepthFrame_CopyFrameDataToIntPtr(IntPtr pNative, IntPtr frameData, uint frameDataSize);
        public void CopyFrameDataToIntPtr(IntPtr frameData, uint size)
        {
            if (_pNative == IntPtr.Zero)
            {
                throw new ObjectDisposedException("DepthFrame");
            }

            Windows_Kinect_DepthFrame_CopyFrameDataToIntPtr(_pNative, frameData, size / sizeof(ushort));
            Helper.ExceptionHelper.CheckLastError();
        }

        [DllImport("KinectUnityAddin", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern IntPtr Windows_Kinect_DepthFrame_LockImageBuffer(IntPtr pNative);
        public KinectBuffer LockImageBuffer()
        {
            if (_pNative == IntPtr.Zero)
            {
                throw new ObjectDisposedException("DepthFrame");
            }

            IntPtr objectPointer = Windows_Kinect_DepthFrame_LockImageBuffer(_pNative);
            Helper.ExceptionHelper.CheckLastError();

            if (objectPointer == IntPtr.Zero)
            {
                return null;
            }

            return Helper.NativeObjectCache.CreateOrGetObject<KinectBuffer>(objectPointer, n => new KinectBuffer(n));
        }
    }

    public sealed partial class BodyIndexFrame
    {
        [DllImport(
            "KinectUnityAddin",
            EntryPoint = "Windows_Kinect_BodyIndexFrame_CopyFrameDataToArray",
            CallingConvention = CallingConvention.Cdecl,
            SetLastError = true)]
        private static extern void Windows_Kinect_BodyIndexFrame_CopyFrameDataToIntPtr(IntPtr pNative, IntPtr frameData, uint frameDataSize);
        public void CopyFrameDataToIntPtr(IntPtr frameData, uint size)
        {
            if (_pNative == IntPtr.Zero)
            {
                throw new ObjectDisposedException("BodyIndexFrame");
            }

            Windows_Kinect_BodyIndexFrame_CopyFrameDataToIntPtr(_pNative, frameData, size);
            Helper.ExceptionHelper.CheckLastError();
        }

        [DllImport("KinectUnityAddin", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern IntPtr Windows_Kinect_BodyIndexFrame_LockImageBuffer(IntPtr pNative);
        public KinectBuffer LockImageBuffer()
        {
            if (_pNative == IntPtr.Zero)
            {
                throw new ObjectDisposedException("BodyIndexFrame");
            }

            IntPtr objectPointer = Windows_Kinect_BodyIndexFrame_LockImageBuffer(_pNative);
            Helper.ExceptionHelper.CheckLastError();

            if (objectPointer == IntPtr.Zero)
            {
                return null;
            }

            return Helper.NativeObjectCache.CreateOrGetObject<KinectBuffer>(objectPointer, n => new KinectBuffer(n));
        }

    }

    public sealed partial class InfraredFrame
    {
        [DllImport(
            "KinectUnityAddin",
            EntryPoint = "Windows_Kinect_InfraredFrame_CopyFrameDataToArray",
            CallingConvention = CallingConvention.Cdecl,
            SetLastError = true)]
        private static extern void Windows_Kinect_InfraredFrame_CopyFrameDataToIntPtr(IntPtr pNative, IntPtr frameData, uint frameDataSize);
        public void CopyFrameDataToIntPtr(IntPtr frameData, uint size)
        {
            if (_pNative == IntPtr.Zero)
            {
                throw new ObjectDisposedException("InfraredFrame");
            }

            Windows_Kinect_InfraredFrame_CopyFrameDataToIntPtr(_pNative, frameData, size / sizeof(ushort));
            Helper.ExceptionHelper.CheckLastError();
        }

        [DllImport("KinectUnityAddin", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern IntPtr Windows_Kinect_InfraredFrame_LockImageBuffer(IntPtr pNative);
        public KinectBuffer LockImageBuffer()
        {
            if (_pNative == IntPtr.Zero)
            {
                throw new ObjectDisposedException("InfraredFrame");
            }

            IntPtr objectPointer = Windows_Kinect_InfraredFrame_LockImageBuffer(_pNative);
            Helper.ExceptionHelper.CheckLastError();

            if (objectPointer == IntPtr.Zero)
            {
                return null;
            }

            return Helper.NativeObjectCache.CreateOrGetObject<KinectBuffer>(objectPointer, n => new KinectBuffer(n));
        }

    }

    public sealed partial class KinectSensor
    {
        private void Dispose(bool disposing)
        {
            if (_pNative == IntPtr.Zero)
            {
                return;
            }

            if (IsOpen)
            {
                Close();
            }

            __EventCleanup();

            Helper.NativeObjectCache.RemoveObject<KinectSensor>(_pNative);
            Windows_Kinect_KinectSensor_ReleaseObject(ref _pNative);

            _pNative = IntPtr.Zero;
        }
    }

    public sealed partial class LongExposureInfraredFrame
    {
        [DllImport(
            "KinectUnityAddin",
            EntryPoint = "Windows_Kinect_LongExposureInfraredFrame_CopyFrameDataToArray",
            CallingConvention = CallingConvention.Cdecl,
            SetLastError = true)]
        private static extern void Windows_Kinect_LongExposureInfraredFrame_CopyFrameDataToIntPtr(IntPtr pNative, IntPtr frameData, uint frameDataSize);
        public void CopyFrameDataToIntPtr(IntPtr frameData, uint size)
        {
            if (_pNative == IntPtr.Zero)
            {
                throw new ObjectDisposedException("LongExposureInfraredFrame");
            }

            Windows_Kinect_LongExposureInfraredFrame_CopyFrameDataToIntPtr(_pNative, frameData, size / sizeof(ushort));
            Helper.ExceptionHelper.CheckLastError();
        }

        [DllImport("KinectUnityAddin", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern IntPtr Windows_Kinect_LongExposureInfraredFrame_LockImageBuffer(IntPtr pNative);
        public KinectBuffer LockImageBuffer()
        {
            if (_pNative == IntPtr.Zero)
            {
                throw new ObjectDisposedException("LongExposureInfraredFrame");
            }

            IntPtr objectPointer = Windows_Kinect_LongExposureInfraredFrame_LockImageBuffer(_pNative);
            Helper.ExceptionHelper.CheckLastError();

            if (objectPointer == IntPtr.Zero)
            {
                return null;
            }

            return Helper.NativeObjectCache.CreateOrGetObject<KinectBuffer>(objectPointer, n => new KinectBuffer(n));
        }

    }

    public sealed partial class CoordinateMapper
    {
        private PointF[] _DepthFrameToCameraSpaceTable = null;

        [DllImport("KinectUnityAddin", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern IntPtr Windows_Kinect_CoordinateMapper_GetDepthCameraIntrinsics(IntPtr pNative);
        public CameraIntrinsics GetDepthCameraIntrinsics()
        {
            if (_pNative == IntPtr.Zero)
            {
                throw new ObjectDisposedException("CoordinateMapper");
            }

            var objectPointer = Windows_Kinect_CoordinateMapper_GetDepthCameraIntrinsics(_pNative);
            Helper.ExceptionHelper.CheckLastError();

            var obj = (CameraIntrinsics)Marshal.PtrToStructure(objectPointer, typeof(CameraIntrinsics));
            Marshal.FreeHGlobal(objectPointer);
            return obj;
        }

        [DllImport("KinectUnityAddin", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern int Windows_Kinect_CoordinateMapper_GetDepthFrameToCameraSpaceTable(IntPtr pNative, IntPtr outCollection, uint outCollectionSize);
        public PointF[] GetDepthFrameToCameraSpaceTable()
        {
            if (_pNative == IntPtr.Zero)
            {
                throw new ObjectDisposedException("CoordinateMapper");
            }

            if (_DepthFrameToCameraSpaceTable == null)
            {
                var desc = KinectSensor.GetDefault().DepthFrameSource.FrameDescription;
                _DepthFrameToCameraSpaceTable = new PointF[desc.Width * desc.Height];

                var pointsSmartGCHandle = new Helper.SmartGCHandle(GCHandle.Alloc(_DepthFrameToCameraSpaceTable, GCHandleType.Pinned));
                var _points = pointsSmartGCHandle.AddrOfPinnedObject();
                Windows_Kinect_CoordinateMapper_GetDepthFrameToCameraSpaceTable(_pNative, _points, (uint)_DepthFrameToCameraSpaceTable.Length);
                Helper.ExceptionHelper.CheckLastError();
            }

            return _DepthFrameToCameraSpaceTable;
        }

        [DllImport("KinectUnityAddin", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern void Windows_Kinect_CoordinateMapper_MapColorFrameToDepthSpace(
            IntPtr pNative,
            IntPtr depthFrameData,
            uint depthFrameDataSize,
            IntPtr depthSpacePoints,
            uint depthSpacePointsSize);
        public void MapColorFrameToDepthSpaceUsingIntPtr(IntPtr depthFrameData, uint depthFrameSize, IntPtr depthSpacePoints, uint depthSpacePointsSize)
        {
            if (_pNative == IntPtr.Zero)
            {
                throw new ObjectDisposedException("CoordinateMapper");
            }

            uint length = depthFrameSize / sizeof(UInt16);
            Windows_Kinect_CoordinateMapper_MapColorFrameToDepthSpace(_pNative, depthFrameData, length, depthSpacePoints, depthSpacePointsSize);
            Helper.ExceptionHelper.CheckLastError();
        }

        [DllImport("KinectUnityAddin", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern void Windows_Kinect_CoordinateMapper_MapColorFrameToCameraSpace(
            IntPtr pNative,
            IntPtr depthFrameData,
            uint depthFrameDataSize,
            IntPtr cameraSpacePoints,
            uint cameraSpacePointsSize);
        public void MapColorFrameToCameraSpaceUsingIntPtr(IntPtr depthFrameData, int depthFrameSize, IntPtr cameraSpacePoints, uint cameraSpacePointsSize)
        {
            if (_pNative == IntPtr.Zero)
            {
                throw new ObjectDisposedException("CoordinateMapper");
            }

            uint length = (uint)depthFrameSize / sizeof(UInt16);
            Windows_Kinect_CoordinateMapper_MapColorFrameToCameraSpace(_pNative, depthFrameData, length, cameraSpacePoints, cameraSpacePointsSize);
            Helper.ExceptionHelper.CheckLastError();
        }

        [DllImport("KinectUnityAddin", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern void Windows_Kinect_CoordinateMapper_MapDepthFrameToColorSpace(
            IntPtr pNative,
            IntPtr depthFrameData,
            uint depthFrameDataSize,
            IntPtr colorSpacePoints,
            uint colorSpacePointsSize);
        public void MapDepthFrameToColorSpaceUsingIntPtr(IntPtr depthFrameData, int depthFrameSize, IntPtr colorSpacePoints, uint colorSpacePointsSize)
        {
            if (_pNative == IntPtr.Zero)
            {
                throw new ObjectDisposedException("CoordinateMapper");
            }

            uint length = (uint)depthFrameSize / sizeof(UInt16);
            Windows_Kinect_CoordinateMapper_MapDepthFrameToColorSpace(_pNative, depthFrameData, length, colorSpacePoints, colorSpacePointsSize);
            Helper.ExceptionHelper.CheckLastError();
        }


        [DllImport("KinectUnityAddin", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern void Windows_Kinect_CoordinateMapper_MapDepthFrameToCameraSpace(
            IntPtr pNative,
            IntPtr depthFrameData,
            uint depthFrameDataSize,
            IntPtr cameraSpacePoints,
            uint cameraSpacePointsSize);
        public void MapDepthFrameToCameraSpaceUsingIntPtr(IntPtr depthFrameData, int depthFrameSize, IntPtr cameraSpacePoints, uint cameraSpacePointsSize)
        {
            if (_pNative == IntPtr.Zero)
            {
                throw new ObjectDisposedException("CoordinateMapper");
            }

            uint length = (uint)depthFrameSize / sizeof(UInt16);
            Windows_Kinect_CoordinateMapper_MapDepthFrameToCameraSpace(_pNative, depthFrameData, length, cameraSpacePoints, cameraSpacePointsSize);
            Helper.ExceptionHelper.CheckLastError();
        }
    }
}