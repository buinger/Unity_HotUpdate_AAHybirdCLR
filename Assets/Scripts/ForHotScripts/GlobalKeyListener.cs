using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;

public class GlobalKeyListener
{
    public static event Action<VirtualKey> onKeyDown;
    // public static event Action onLeftKeyDown;
    // public static event Action onRightKeyDown;

    private static LowLevelKeyboardProc _proc = HookCallback;
    private static IntPtr _hookID = IntPtr.Zero;

    static Dictionary<VirtualKey, List<Action>> hotKeyDic_Alt = new Dictionary<VirtualKey, List<Action>>();

    static Dictionary<VirtualKey, List<Action>> hotKeyDic_Single = new Dictionary<VirtualKey, List<Action>>();

    public static void Start()
    {
        _hookID = SetHook(_proc);
    }

    public static void Stop()
    {
        UnhookWindowsHookEx(_hookID);
    }
    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
    private static IntPtr SetHook(LowLevelKeyboardProc proc)
    {
        // 使用 IntPtr.Zero 作为模块句柄
        return SetWindowsHookEx(WH_KEYBOARD_LL, proc, IntPtr.Zero, 0);
    }

    public static void RegisterHotKey_Alt(VirtualKey vk, Action action)
    {
        if (!hotKeyDic_Alt.ContainsKey(vk))
        {
            hotKeyDic_Alt.Add(vk, new List<Action>());
        }
        hotKeyDic_Alt[vk].Add(action);
    }

    public static void RegisterHotKey_Single(VirtualKey vk, Action action)
    {
        if (!hotKeyDic_Single.ContainsKey(vk))
        {
            hotKeyDic_Single.Add(vk, new List<Action>());
        }
        hotKeyDic_Single[vk].Add(action);
    }

    [MonoPInvokeCallback(typeof(LowLevelKeyboardProc))]
    private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN))
        {
            int vkCode = Marshal.ReadInt32(lParam);
            //    KeyCode keyCode = ConvertToUnityKeyCode(vkCode);


            // Alt+XX
            if ((GetAsyncKeyState((int)VirtualKey.VK_CONTROL) & 0x8000) != 0)
            {
                if (hotKeyDic_Alt.ContainsKey((VirtualKey)vkCode))
                {
                    foreach (var action in hotKeyDic_Alt[(VirtualKey)vkCode])
                    {
                        action?.Invoke();
                    }
                }
            }
            //Single
            if (hotKeyDic_Single.ContainsKey((VirtualKey)vkCode))
            {
                foreach (var action in hotKeyDic_Single[(VirtualKey)vkCode])
                {
                    action?.Invoke();
                }
            }

            // if (GetAsyncKeyState((int)VirtualKey.VK_LEFT))
            // {
            //     onLeftKeyDown?.Invoke();
            // }
            // else if ((GetAsyncKeyState((int)VirtualKey.VK_RIGHT) & 0x8000) != 0)
            // {
            //     onRightKeyDown?.Invoke();
            // }

            // if ((GetAsyncKeyState((int)VirtualKey.VK_UP) & 0x8000) != 0)
            // {
            //     UnityEngine.Debug.Log("VK_UP");
            // }
            // else if ((GetAsyncKeyState((int)VirtualKey.VK_DOWN) & 0x8000) != 0)
            // {
            //     UnityEngine.Debug.Log("VK_DOWN");
            // }

            onKeyDown?.Invoke((VirtualKey)vkCode);
        }
        return CallNextHookEx(_hookID, nCode, wParam, lParam);
    }


    public static void SimulateCopy()
    {
        // 定义键盘按键的虚拟键码
        byte VK_CONTROL = 0x11; // Ctrl键
        byte VK_C = 0x43;      // C键
        uint KEYEVENTF_KEYDOWN = 0x0000; // 按键按下
        uint KEYEVENTF_KEYUP = 0x0002;   // 按键释放
        // 按下Ctrl键
        keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
        // 按下C键
        keybd_event(VK_C, 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
        // 释放C键
        keybd_event(VK_C, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
        // 释放Ctrl键
        keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
        UnityEngine.Debug.Log($"模拟复制:{GUIUtility.systemCopyBuffer}");

    }

    private static KeyCode ConvertToUnityKeyCode(int vkCode)
    {
        switch (vkCode)
        {
            case 0x41: return KeyCode.A;
            case 0x42: return KeyCode.B;
            case 0x43: return KeyCode.C;
            case 0x44: return KeyCode.D;
            case 0x45: return KeyCode.E;
            case 0x46: return KeyCode.F;
            case 0x47: return KeyCode.G;
            case 0x48: return KeyCode.H;
            case 0x49: return KeyCode.I;
            case 0x4A: return KeyCode.J;
            case 0x4B: return KeyCode.K;
            case 0x4C: return KeyCode.L;
            case 0x4D: return KeyCode.M;
            case 0x4E: return KeyCode.N;
            case 0x4F: return KeyCode.O;
            case 0x50: return KeyCode.P;
            case 0x51: return KeyCode.Q;
            case 0x52: return KeyCode.R;
            case 0x53: return KeyCode.S;
            case 0x54: return KeyCode.T;
            case 0x55: return KeyCode.U;
            case 0x56: return KeyCode.V;
            case 0x57: return KeyCode.W;
            case 0x58: return KeyCode.X;
            case 0x59: return KeyCode.Y;
            case 0x5A: return KeyCode.Z;
            case 0x30: return KeyCode.Alpha0;
            case 0x31: return KeyCode.Alpha1;
            case 0x32: return KeyCode.Alpha2;
            case 0x33: return KeyCode.Alpha3;
            case 0x34: return KeyCode.Alpha4;
            case 0x35: return KeyCode.Alpha5;
            case 0x36: return KeyCode.Alpha6;
            case 0x37: return KeyCode.Alpha7;
            case 0x38: return KeyCode.Alpha8;
            case 0x39: return KeyCode.Alpha9;
            case 0x1B: return KeyCode.Escape;
            case 0x0D: return KeyCode.Return;
            case 0x09: return KeyCode.Tab;
            case 0x20: return KeyCode.Space;
            case 0x2E: return KeyCode.Delete;
            // 添加其他键码映射
            default: return KeyCode.None;
        }
    }

    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;
    private const int WM_SYSKEYDOWN = 0x0104;

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    [DllImport("user32.dll")]
    public static extern short GetAsyncKeyState(int vKey);

    // 导入Windows API
    [DllImport("user32.dll")]
    private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

}

public enum VirtualKey
{
    VK_LBUTTON = 0x01,   // Left mouse button
    VK_RBUTTON = 0x02,   // Right mouse button
    VK_CANCEL = 0x03,    // Control-break processing
    VK_MBUTTON = 0x04,   // Middle mouse button (three-button mouse)
    VK_XBUTTON1 = 0x05,  // X1 mouse button
    VK_XBUTTON2 = 0x06,  // X2 mouse button
    VK_BACK = 0x08,      // BACKSPACE key
    VK_TAB = 0x09,       // TAB key
    VK_CLEAR = 0x0C,     // CLEAR key
    VK_RETURN = 0x0D,    // ENTER key
    VK_SHIFT = 0x10,     // SHIFT key
    VK_CONTROL = 0x11,   // CTRL key
    VK_MENU = 0x12,      // ALT key
    VK_PAUSE = 0x13,     // PAUSE key
    VK_CAPITAL = 0x14,   // CAPS LOCK key
    VK_ESCAPE = 0x1B,    // ESC key
    VK_SPACE = 0x20,     // SPACEBAR
    VK_PRIOR = 0x21,     // PAGE UP key
    VK_NEXT = 0x22,      // PAGE DOWN key
    VK_END = 0x23,       // END key
    VK_HOME = 0x24,      // HOME key
    VK_LEFT = 0x25,      // LEFT ARROW key
    VK_UP = 0x26,        // UP ARROW key
    VK_RIGHT = 0x27,     // RIGHT ARROW key
    VK_DOWN = 0x28,      // DOWN ARROW key
    VK_SELECT = 0x29,    // SELECT key
    VK_PRINT = 0x2A,     // PRINT key
    VK_EXECUTE = 0x2B,   // EXECUTE key
    VK_SNAPSHOT = 0x2C,  // PRINT SCREEN key
    VK_INSERT = 0x2D,    // INS key
    VK_DELETE = 0x2E,    // DEL key
    VK_HELP = 0x2F,      // HELP key
    VK_0 = 0x30,         // 0 key
    VK_1 = 0x31,         // 1 key
    VK_2 = 0x32,         // 2 key
    VK_3 = 0x33,         // 3 key
    VK_4 = 0x34,         // 4 key
    VK_5 = 0x35,         // 5 key
    VK_6 = 0x36,         // 6 key
    VK_7 = 0x37,         // 7 key
    VK_8 = 0x38,         // 8 key
    VK_9 = 0x39,         // 9 key
    VK_A = 0x41,         // A key
    VK_B = 0x42,         // B key
    VK_C = 0x43,         // C key
    VK_D = 0x44,         // D key
    VK_E = 0x45,         // E key
    VK_F = 0x46,         // F key
    VK_G = 0x47,         // G key
    VK_H = 0x48,         // H key
    VK_I = 0x49,         // I key
    VK_J = 0x4A,         // J key
    VK_K = 0x4B,         // K key
    VK_L = 0x4C,         // L key
    VK_M = 0x4D,         // M key
    VK_N = 0x4E,         // N key
    VK_O = 0x4F,         // O key
    VK_P = 0x50,         // P key
    VK_Q = 0x51,         // Q key
    VK_R = 0x52,         // R key
    VK_S = 0x53,         // S key
    VK_T = 0x54,         // T key
    VK_U = 0x55,         // U key
    VK_V = 0x56,         // V key
    VK_W = 0x57,         // W key
    VK_X = 0x58,         // X key
    VK_Y = 0x59,         // Y key
    VK_Z = 0x5A,         // Z key
    VK_LWIN = 0x5B,      // Left Windows key
    VK_RWIN = 0x5C,      // Right Windows key
    VK_APPS = 0x5D,      // APPLICATION key
    VK_SLEEP = 0x5F,     // SLEEP key
    VK_NUMPAD0 = 0x60,   // Numeric keypad 0 key
    VK_NUMPAD1 = 0x61,   // Numeric keypad 1 key
    VK_NUMPAD2 = 0x62,   // Numeric keypad 2 key
    VK_NUMPAD3 = 0x63,   // Numeric keypad 3 key
    VK_NUMPAD4 = 0x64,   // Numeric keypad 4 key
    VK_NUMPAD5 = 0x65,   // Numeric keypad 5 key
    VK_NUMPAD6 = 0x66,   // Numeric keypad 6 key
    VK_NUMPAD7 = 0x67,   // Numeric keypad 7 key
    VK_NUMPAD8 = 0x68,   // Numeric keypad 8 key
    VK_NUMPAD9 = 0x69,   // Numeric keypad 9 key
    VK_MULTIPLY = 0x6A,  // Multiply key
    VK_ADD = 0x6B,       // Add key
    VK_SEPARATOR = 0x6C, // Separator key
    VK_SUBTRACT = 0x6D,  // Subtract key
    VK_DECIMAL = 0x6E,   // Decimal key
    VK_DIVIDE = 0x6F,    // Divide key
    VK_F1 = 0x70,        // F1 key
    VK_F2 = 0x71,        // F2 key
    VK_F3 = 0x72,        // F3 key
    VK_F4 = 0x73,        // F4 key
    VK_F5 = 0x74,        // F5 key
    VK_F6 = 0x75,        // F6 key
    VK_F7 = 0x76,        // F7 key
    VK_F8 = 0x77,        // F8 key
    VK_F9 = 0x78,        // F9 key
    VK_F10 = 0x79,       // F10 key
    VK_F11 = 0x7A,       // F11 key
    VK_F12 = 0x7B,       // F12 key
    VK_F13 = 0x7C,       // F13 key
    VK_F14 = 0x7D,       // F14 key
    VK_F15 = 0x7E,       // F15 key
    VK_F16 = 0x7F,       // F16 key
    VK_F17 = 0x80,       // F17 key
    VK_F18 = 0x81,       // F18 key
    VK_F19 = 0x82,       // F19 key
    VK_F20 = 0x83,       // F20 key
    VK_F21 = 0x84,       // F21 key
    VK_F22 = 0x85,       // F22 key
    VK_F23 = 0x86,       // F23 key
    VK_F24 = 0x87,       // F24 key
    VK_NUMLOCK = 0x90,   // NUM LOCK key
    VK_SCROLL = 0x91,    // SCROLL LOCK key
    VK_LSHIFT = 0xA0,    // Left SHIFT key
    VK_RSHIFT = 0xA1,    // Right SHIFT key
    VK_LCONTROL = 0xA2,  // Left CONTROL key
    VK_RCONTROL = 0xA3,  // Right CONTROL key
    VK_LMENU = 0xA4,     // Left ALT key
    VK_RMENU = 0xA5,     // Right ALT key
    VK_OEM_1 = 0xBA,     // ';' key
    VK_OEM_PLUS = 0xBB,  // '+' key
    VK_OEM_COMMA = 0xBC, // ',' key
    VK_OEM_MINUS = 0xBD, // '-' key
    VK_OEM_PERIOD = 0xBE,// '.' key
    VK_OEM_2 = 0xBF,     // '/' key
    VK_OEM_3 = 0xC0,     // '`' key
                         // … 你可以根据需要查找更多虚拟键
}
