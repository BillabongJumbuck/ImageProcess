using System.Runtime.InteropServices;

namespace ImageProcess.Utility;

public static class WindowsMessage
{
    public const int WM_COPYDATA = 0x004A;

    public struct COPYDATASTRUCT
    {
        public IntPtr DwData;    // 用于传递处理索引
        public int CbData;       // 数据大小
        [MarshalAs(UnmanagedType.LPStr)]
        public string LpData;    // 状态信息
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr FindWindow([MarshalAs(UnmanagedType.LPTStr)] string? lpClassName, [MarshalAs(UnmanagedType.LPTStr)] string lpWindowName);

    [DllImport("User32.dll")]
    public static extern int SendMessage(IntPtr hwnd, int msg, IntPtr wParam, ref COPYDATASTRUCT lParam);

    public enum ProcessStatus
    {
        Processing = 0,
        Completed = 1,
        Failed = 2,
        Cancelled = 3
    }

    public static void SendProgressMessage(IntPtr hwnd, int fileIndex, ProcessStatus status)
    {
        if (hwnd != IntPtr.Zero)
        {
            var cds = new COPYDATASTRUCT
            {
                DwData = fileIndex,
                CbData = sizeof(int),
                LpData = ((int)status).ToString()
            };

            SendMessage(hwnd, WM_COPYDATA, IntPtr.Zero, ref cds);
        }
    }
} 