using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Assets.Scripts
{
    /// <summary>
    /// Phát hiện các tool hack dựa trên tiêu đề cửa sổ – tương thích IL2CPP.
    /// </summary>
    internal sealed class HackDetector
    {
        #region ---------- Cấu hình ----------
        // Danh sách từ khoá blacklist (chữ thường)
        private static readonly List<string> blacklistedProcesses = new()
        {
            "nromessagelogger",
            "ida", "ida64",
            "ollydbg", "x32dbg", "x64dbg",
            "hxd", "hxd32", "hxd64", "hxdcmp", "hexeditor"
        };
        #endregion

        #region ---------- Singleton ----------
        private static readonly HackDetector _instance = new();
        public static HackDetector gI() => _instance;
        private HackDetector() { }
        #endregion

        #region ---------- Biến trạng thái ----------
        private long _lastCheck;
        public string processNameCheck = "";
        #endregion

        #region ---------- Win32 API ----------
        // Delegate callback – phải STATIC, đúng StdCall
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        // Giữ delegate để tránh GC & stripping
        private static readonly EnumWindowsProc _enumCb = EnumWindowsCallback;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);
        #endregion

        #region ---------- Vòng lặp cập-nhật ----------
        public void Update()
        {
            if (mSystem.currentTimeMillis() - _lastCheck < 1000) return; // mỗi giây
            _lastCheck = mSystem.currentTimeMillis();
            if (Application.platform is RuntimePlatform.WindowsPlayer
                                   or RuntimePlatform.WindowsEditor)
            {
                CheckProcess();
            }
        }
        #endregion

        #region ---------- Phát hiện hack ----------
        public bool CheckProcess()
        {
            try
            {
                _windowTitles.Clear();
                // Gọi EnumWindows – IL2CPP sẽ reverse-P/Invoke vào hàm static bên dưới
                EnumWindows(_enumCb, IntPtr.Zero);

                foreach (string title in _windowTitles)
                {
                    processNameCheck = title;
                    string lower = title.ToLower();
                    foreach (string bad in blacklistedProcesses)
                    {
                        if (lower.Contains(bad))
                        {
                            Detect(title);
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Lỗi checkProcess: " + ex);
                //   File.AppendAllText("logerr.txt", ex + Environment.NewLine);
            }
            return false;
        }

        // Danh sách tiêu đề thu thập được – static để callback ghi
        private static readonly List<string> _windowTitles = new();

        // Callback được Win32 gọi tới
        [AOT.MonoPInvokeCallback(typeof(EnumWindowsProc))]
        private static bool EnumWindowsCallback(IntPtr hWnd, IntPtr lParam)
        {
            if (!IsWindowVisible(hWnd)) return true;

            var sb = new StringBuilder(256);
            if (GetWindowText(hWnd, sb, sb.Capacity) > 0)
            {
                string title = sb.ToString().Trim();
                if (!string.IsNullOrEmpty(title))
                    _windowTitles.Add(title);
            }
            return true; // tiếp tục liệt kê
        }
        #endregion

        #region ---------- Xử lý khi phát hiện ----------
        private void Detect(string processName)
        {
            Debug.LogError("Detect tool hack: " + processName);
            GameScr.info1.addInfo(processName, 0);

            Controller.isDisconnected = true;
            if (Session_ME.gI().isConnected()) Session_ME.gI().close();
            if (Session_ME2.gI().isConnected()) Session_ME2.gI().close();

            Application.Quit();
        }
        #endregion
    }
}
