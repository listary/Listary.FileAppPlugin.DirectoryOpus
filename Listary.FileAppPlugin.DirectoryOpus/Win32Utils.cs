using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace Listary.FileAppPlugin.DirectoryOpus
{
    internal static class Win32Utils
    {
        internal static string GetClassName(HWND hWnd)
        {
            unsafe
            {
                char* className = stackalloc char[256];
                PInvoke.GetClassName(hWnd, className, 256);
                return new string(className);
            }
        }

        internal static HWND GetFocus(HWND hWnd)
        {
            unsafe
            {
                uint threadId = PInvoke.GetWindowThreadProcessId(hWnd);

                GUITHREADINFO threadInfo = new GUITHREADINFO { };
                threadInfo.cbSize = (uint)Marshal.SizeOf(threadInfo);
                if (PInvoke.GetGUIThreadInfo(threadId, ref threadInfo))
                {
                    return threadInfo.hwndFocus;
                }
            }
            return default;
        }
        
        internal static string GetWindowText(HWND hWnd)
        {
            unsafe
            {
                char* buffer = stackalloc char[260];
                // WM_GETTEXT won't be dropped by UIPI
                PInvoke.SendMessage(hWnd, PInvoke.WM_GETTEXT, 260, (nint)buffer);
                return new(buffer);
            }
        }

        internal static HWND FindWindowExRecursively(HWND hWndParent, HWND hWndChildAfter, string lpszClass, string lpszTitle)
        {
            HWND result = PInvoke.FindWindowEx(hWndParent, hWndChildAfter, lpszClass, lpszTitle);
            if (result != default)
                return result;
            
            HWND child = PInvoke.FindWindowEx(hWndParent, hWndChildAfter, default(PCWSTR), default);
            while (child != default)
            {
                result = FindWindowExRecursively(child, default, lpszClass, lpszTitle);
                if (result != default)
                {
                    return result;
                }
                child = PInvoke.FindWindowEx(hWndParent, child, default(PCWSTR), default);
            }
            
            return default;
        }
    }
}
