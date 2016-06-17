using System;
using System.Runtime.InteropServices;

namespace GodlikeStickCreator.Native
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct SHSTOCKICONINFO
    {
        public UInt32 cbSize;
        public IntPtr hIcon;
        public Int32 iSysIconIndex;
        public Int32 iIcon;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260 /*MAX_PATH*/)] public string szPath;
    }
}