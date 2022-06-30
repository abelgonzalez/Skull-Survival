using System;
using System.Runtime.InteropServices;

namespace Windows.Kinect
{
    // NOTE: This uses an IBuffer under the covers, it is renamed here to give parity to our managed APIs.
    public class KinectBuffer : Helper.INativeWrapper, IDisposable
    {
        internal IntPtr _pNative;

        IntPtr Helper.INativeWrapper.nativePtr { get { return _pNative; } }

        // Constructors and Finalizers
        internal KinectBuffer(IntPtr pNative)
        {
            _pNative = pNative;
            Windows_Storage_Streams_IBuffer_AddRefObject(ref _pNative);
        }

        ~KinectBuffer()
        {
            Dispose(false);
        }

        [DllImport("KinectUnityAddin", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Windows_Storage_Streams_IBuffer_ReleaseObject(ref IntPtr pNative);

        [DllImport("KinectUnityAddin", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Windows_Storage_Streams_IBuffer_AddRefObject(ref IntPtr pNative);
        private void Dispose(bool disposing)
        {
            if (_pNative == IntPtr.Zero)
            {
                return;
            }

            Helper.NativeObjectCache.RemoveObject<KinectBuffer>(_pNative);

            if (disposing)
            {
                Windows_Storage_Streams_IBuffer_Dispose(_pNative);
            }

            Windows_Storage_Streams_IBuffer_ReleaseObject(ref _pNative);

            _pNative = IntPtr.Zero;
        }


        // Public Properties
        [DllImport("KinectUnityAddin", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern uint Windows_Storage_Streams_IBuffer_get_Capacity(IntPtr pNative);
        public uint Capacity
        {
            get
            {
                if (_pNative == IntPtr.Zero)
                {
                    throw new ObjectDisposedException("KinectBuffer");
                }

                uint capacity = Windows_Storage_Streams_IBuffer_get_Capacity(_pNative);
                Helper.ExceptionHelper.CheckLastError();
                return capacity;
            }
        }

        [DllImport("KinectUnityAddin", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern uint Windows_Storage_Streams_IBuffer_get_Length(IntPtr pNative);
        [DllImport("KinectUnityAddin", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern void Windows_Storage_Streams_IBuffer_put_Length(IntPtr pNative, uint value);
        public uint Length
        {
            get
            {
                if (_pNative == IntPtr.Zero)
                {
                    throw new ObjectDisposedException("KinectBuffer");
                }

                uint length = Windows_Storage_Streams_IBuffer_get_Length(_pNative);
                Helper.ExceptionHelper.CheckLastError();
                return length;
            }
            set
            {
                if (_pNative == IntPtr.Zero)
                {
                    throw new ObjectDisposedException("KinectBuffer");
                }

                Windows_Storage_Streams_IBuffer_put_Length(_pNative, value);
                Helper.ExceptionHelper.CheckLastError();
            }
        }

        [DllImport("KinectUnityAddin", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Windows_Storage_Streams_IBuffer_Dispose(IntPtr pNative);
        // Constructors and Finalizers
        public void Dispose()
        {
            if (_pNative == IntPtr.Zero)
            {
                throw new ObjectDisposedException("KinectBuffer");
            }

            Dispose(true);
            GC.SuppressFinalize(this);
        }

        [DllImport("KinectUnityAddin", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern IntPtr Windows_Storage_Streams_IBuffer_get_UnderlyingBuffer(IntPtr pNative);
        public IntPtr UnderlyingBuffer
        {
            get
            {
                if (_pNative == IntPtr.Zero)
                {
                    throw new ObjectDisposedException("KinectBuffer");
                }

                IntPtr value = Windows_Storage_Streams_IBuffer_get_UnderlyingBuffer(_pNative);
                Helper.ExceptionHelper.CheckLastError();
                return value;
            }
        }
    }
}