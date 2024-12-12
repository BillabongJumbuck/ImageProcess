using System.Runtime.InteropServices;

namespace ImageProcess.Utility;

public static class WindowsMessage
{
    // 定义自定义消息ID（WM_USER + 100 避免与系统消息冲突）
    public const int WM_PROGRESS_UPDATE = 0x400 + 100;  // 0x400 是 WM_USER

    // Win32 API 导入
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr FindWindow([MarshalAs(UnmanagedType.LPTStr)] string? lpClassName, [MarshalAs(UnmanagedType.LPTStr)] string lpWindowName);

    [DllImport("user32.dll")]
    public static extern bool PostMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
} 