using System.Runtime.InteropServices;
using System.Text;

namespace FWindSoft.WindowsApi
{
    public class WinAPi
    {
        #region 窗体操作相关
        public delegate bool EnumThreadWndProc(int hwnd, int lParam);
        [DllImport("user32.dll")]
        public static extern int EnumThreadWindows(int dwThreadId, EnumThreadWndProc proc, int lParam);
        [DllImport("user32.dll")]
        public static extern int EnumWindows(EnumThreadWndProc lpFun, int lParam);
        [DllImport("user32.dll")]
        public static extern int IsWindowEnabled(int hwnd);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int GetWindow(int hwnd, int wCmd);
        [DllImport("User32.dll")]
        public static extern int GetWindowText(int hwnd, StringBuilder lpString, int nMaxCount);

        [DllImport("User32.dll")]
        public static extern int GetWindowThreadProcessId(int hwnd, out int nMaxCount);
        [DllImport("User32.dll")]
        public static extern bool IsWindow(int hwnd);
        #endregion

    }
}
