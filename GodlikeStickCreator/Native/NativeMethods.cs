using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace GodlikeStickCreator.Native
{
    internal static class NativeMethods
    {
        [DllImport("user32.dll")]
        internal static extern bool GetWindowRect(IntPtr hWnd, ref Rectangle lpRect);

        [DllImport("user32.dll")]
        internal static extern int MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll")]
        internal static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);

        [DllImport("user32.dll")]
        internal static extern int UnhookWindowsHookEx(IntPtr idHook);

        [DllImport("user32.dll")]
        internal static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        internal static extern IntPtr GetCurrentThreadId();

        [DllImport("Shell32.dll", SetLastError = false)]
        internal static extern Int32 SHGetStockIconInfo(SHSTOCKICONID siid, SHGSI uFlags, ref SHSTOCKICONINFO psii);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool DestroyIcon(IntPtr hIcon);

        [DllImport("user32.dll")]
        internal static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        [DllImport("user32.dll")]
        internal static extern bool SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter, int x, int y, int width,
            int height, uint flags);

        [DllImport("user32.dll")]
        internal static extern int GetWindowLong(IntPtr hwnd, int index);

        internal delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);
    }
}