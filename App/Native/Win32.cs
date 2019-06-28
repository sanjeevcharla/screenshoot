using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;

namespace ScreenShoot
{
    public class Win32
    {
        public const int SRCCOPY = 0x00CC0020;
        const int KEY_BOARD = 13;
        const int KEY_DOWN = 0x0100;
        const int SYS_KEY_DOWN = 0x0104;

        #region Interop
        [DllImport("gdi32.dll")]
        public static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest,
            int nWidth, int nHeight, IntPtr hObjectSource,
            int nXSrc, int nYSrc, int dwRop);
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth,
            int nHeight);
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hDC);
        [DllImport("gdi32.dll")]
        public static extern bool DeleteDC(IntPtr hDC);
        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
        [DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rectangle rect);

        [DllImport("user32.dll")]
        static extern int CallNextHookEx(IntPtr hhk, int code, int wParam, ref KeyInfo lParam);
        [DllImport("user32.dll")]
        static extern IntPtr SetWindowsHookEx(int idHook, HookProcedure callback, IntPtr hInstance, uint theardID);
        [DllImport("user32.dll")]
        static extern bool UnhookWindowsHookEx(IntPtr hInstance);
        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string lpFileName); 
        #endregion

        [StructLayout(LayoutKind.Sequential)]
        public struct Rectangle
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
        public struct KeyInfo
        {
            public int keyCode;
        }

        HookProcedure hookProcedure;
        IntPtr Hook = IntPtr.Zero;
        public event KeyEventHandler KeyDown;
        public delegate int HookProcedure(int Code, int wParam, ref KeyInfo lParam);
        public Win32()
        {
            hookProcedure = new HookProcedure(ProcessHook);
        }
        ~Win32()
        {
            RemoveListener();
        }

        public void AddListener()
        {
            IntPtr hInstance = LoadLibrary("User32");
            Hook = SetWindowsHookEx(KEY_BOARD, hookProcedure, hInstance, 0);
        }

        public void RemoveListener()
        {
            UnhookWindowsHookEx(Hook);
        }

        public int ProcessHook(int Code, int wParam, ref KeyInfo keyInfo)
        {
            if (Code >= 0)
            {
                Keys key = (Keys)keyInfo.keyCode;
                if (key == Keys.PrintScreen)
                {
                    if ((wParam == KEY_DOWN || wParam == SYS_KEY_DOWN) && (KeyDown != null))
                        KeyDown(this, new KeyEventArgs(key));
                }
            }
            return CallNextHookEx(Hook, Code, wParam, ref keyInfo);
        }
        public static Image ScreenShot()
        {
            var desktopWindowHandle = GetDesktopWindow();
            IntPtr hdcSrc = GetWindowDC(desktopWindowHandle);
            Rectangle windowRect = new Rectangle();
            GetWindowRect(desktopWindowHandle, ref windowRect);
            int width = windowRect.right - windowRect.left;
            int height = windowRect.bottom - windowRect.top;
            IntPtr hdcDest = CreateCompatibleDC(hdcSrc);
            IntPtr hBitmap = CreateCompatibleBitmap(hdcSrc, width, height);
            IntPtr hOld = SelectObject(hdcDest, hBitmap);
            BitBlt(hdcDest, 0, 0, width, height, hdcSrc, 0, 0, SRCCOPY);
            SelectObject(hdcDest, hOld);
            DeleteDC(hdcDest);
            ReleaseDC(desktopWindowHandle, hdcSrc);
            Image image = Image.FromHbitmap(hBitmap);
            DeleteObject(hBitmap);
            return image;
        }
    }
}
