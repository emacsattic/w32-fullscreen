using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace toggleTitle
{
    class Program
    {
        // Constants from WinUser.h
        const int GWL_STYLE = -16;
        const int GWL_EXSTYLE = -20;

        const uint WS_CAPTION = 0x00C00000;

        // Imports from user32.dll
        [DllImport("User32", CharSet = CharSet.Auto)]
        private static extern int SetWindowLong(IntPtr hWnd, int Index, int Value);

        [DllImport("User32", CharSet = CharSet.Auto)]
        private static extern int GetWindowLong(IntPtr hWnd, int Index);

        // -- main functions
        static int GetWindowStyle(int hwnd) {
            return GetWindowLong(new IntPtr(hwnd), GWL_STYLE);
        }

        static void ToggleWindowCaption(int hwnd) {
            int currentStyle = GetWindowStyle(hwnd);
            int newStyle = currentStyle ^ (int) WS_CAPTION;
            SetWindowLong(new IntPtr(hwnd), GWL_STYLE, newStyle);
        }

        static List<Process> FindWindows(Regex regexpToMatch) {
            List<Process> results = new List<Process>();
            foreach (Process win in Process.GetProcesses()) {
                if (regexpToMatch.IsMatch(win.MainWindowTitle)) {
                    results.Add(win);
                }
            }
            return results;
        }

        static void Main(string[] args) {
            System.Console.WriteLine("== toggle windows ==");
            if (args.Length < 1) {
                Console.WriteLine("Usage: togglecaption <hwnd>");
                return;
            }
            int windowHwnd = Int32.Parse(args[0]);

            foreach (Process proc in Process.GetProcesses()) {
                if (proc.MainWindowHandle == new IntPtr(windowHwnd)) {
                    System.Console.WriteLine(proc.MainWindowTitle);
                    Console.WriteLine("Toggled WS_CAPTION on: " + proc.MainWindowTitle);
                    ToggleWindowCaption(windowHwnd);
                    return;
                }
            }

            Console.WriteLine("hwnd not found. Exiting.");
        }
    }
}
