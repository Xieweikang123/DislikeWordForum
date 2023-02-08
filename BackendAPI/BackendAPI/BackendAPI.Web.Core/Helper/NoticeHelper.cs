using Microsoft.Extensions.Caching.Memory;
using System;
using System.Runtime.InteropServices;

namespace BackendAPI.Web.Core.Helper
{

    /// <summary>
    /// windows 右下角通知
    /// </summary>
    public class NoticeHelper
    {


        [DllImport("shell32.dll")]
        public static extern uint Shell_NotifyIcon(uint dwMessage, [In] ref NOTIFYICONDATA pnid);


        public static void ShowNotification(string title, string message)
        {
            NOTIFYICONDATA data = new NOTIFYICONDATA();
            data.cbSize = Marshal.SizeOf(data);
            data.hWnd = IntPtr.Zero;
            data.uID = 0;
            data.uFlags = 0x10;
            data.szTip = message;
            data.szInfo = message;
            data.szInfoTitle = title;
            data.uTimeoutOrVersion = 5000;
            data.dwInfoFlags = 0;
            //先删除气泡，再添加
            Shell_NotifyIcon(0x00000002, ref data);
            Shell_NotifyIcon(0x00000000, ref data);
        }


        public struct NOTIFYICONDATA
        {
            public int cbSize;
            public IntPtr hWnd;
            public uint uID;
            public uint uFlags;
            public uint uCallbackMessage;
            public IntPtr hIcon;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string szTip;
            public uint dwState;
            public uint dwStateMask;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szInfo;
            public uint uTimeoutOrVersion;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string szInfoTitle;
            public uint dwInfoFlags;
        }
    }
}
