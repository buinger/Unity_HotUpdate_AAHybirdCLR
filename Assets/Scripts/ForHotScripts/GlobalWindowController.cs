using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class GlobalWindowController : MonoBehaviour
{
    public static IntPtr hWnd;
    private const int SW_RESTORE = 9;
    private const int SW_MINIMIZE = 6;
    private const uint SWP_NOSIZE = 0x0001;
    private const uint SWP_NOMOVE = 0x0002;
    private const uint TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;
    private const uint SWP_NOZORDER = 0x0004;

    private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
    private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);

    [DllImport("User32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("User32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("User32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("User32.dll")]
    private static extern IntPtr GetActiveWindow();

    [DllImport("User32.dll")]
    private static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    [DllImport("User32.dll", SetLastError = true)]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("User32.dll")]
    private static extern bool BringWindowToTop(IntPtr hWnd);

    [DllImport("User32.dll")]
    private static extern IntPtr SetFocus(IntPtr hWnd);

    [DllImport("User32.dll")]
    private static extern int GetSystemMetrics(int nIndex);

    [DllImport("User32.dll", SetLastError = true)]
    private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [DllImport("User32.dll", SetLastError = true)]
    private static extern bool SystemParametersInfo(uint uiAction, uint uiParam, ref RECT pvParam, uint fWinIni);

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    private const int SM_CXSCREEN = 0;
    private const int SM_CYSCREEN = 1;
    private const uint SPI_GETWORKAREA = 0x0030;

    public static void Ini()
    {
        hWnd = GetActiveWindow();
    }
    public static void RestoreAndBringToFront()
    {
        if (hWnd == IntPtr.Zero)
        {
            Debug.LogError("Failed to get active window handle.");
            return;
        }

        // Minimize the window
        ShowWindow(hWnd, SW_MINIMIZE);

        // Restore the window
        ShowWindow(hWnd, SW_RESTORE);

        // Bring the window to the foreground
        SetForegroundWindow(hWnd);

        // Bring the window to the top and set focus
        BringWindowToTop(hWnd);
        SetFocus(hWnd);
    }

    public static void SetTopMost()
    {
        if (hWnd == IntPtr.Zero)
        {
            Debug.LogError("Failed to get active window handle.");
            return;
        }
        // Set the window to be always on top
        SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
    }

    public static void RemoveTopMost()
    {
        if (hWnd == IntPtr.Zero)
        {
            Debug.LogError("Failed to get active window handle.");
            return;
        }

        // Remove the always on top attribute
        SetWindowPos(hWnd, HWND_NOTOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
    }

    public static void SetWindowToBottomRight()
    {
        if (hWnd == IntPtr.Zero)
        {
            Debug.LogError("Failed to get active window handle.");
            return;
        }

        // Get screen width and height
        int screenWidth = GetSystemMetrics(SM_CXSCREEN);
        int screenHeight = GetSystemMetrics(SM_CYSCREEN);

        // Get the work area (excluding taskbar)
        RECT workArea = new RECT();
        SystemParametersInfo(SPI_GETWORKAREA, 0, ref workArea, 0);

        // Get the window rectangle
        if (!GetWindowRect(hWnd, out RECT rect))
        {
            Debug.LogError("Failed to get window rectangle.");
            return;
        }

        // Calculate the new position for the window to be at the bottom right corner
        int windowWidth = rect.Right - rect.Left;
        int windowHeight = rect.Bottom - rect.Top;
        int newX = workArea.Right - windowWidth;
        int newY = workArea.Bottom - windowHeight;

        // Set the window position to the bottom right corner
        SetWindowPos(hWnd, IntPtr.Zero, newX, newY, 0, 0, SWP_NOSIZE | SWP_NOZORDER);
    }
}