using System;

namespace Helper
{
    internal interface INativeWrapper
    {
        IntPtr nativePtr { get; }
    }
}