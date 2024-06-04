using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Foundry.UI.WinForms
{
    internal static class Program
    {
        public static void ToggleConsole()
        {
            bool visible = IsWindowVisible(GetConsoleWindow());
            ShowConsole(!visible);
        }
        public static void ShowConsole(bool show)
        {
            ShowWindow(GetConsoleWindow(), show ? 1 : 0);
        }

        [DllImport("kernel32.dll")]
        static extern nint GetConsoleWindow();
        [DllImport("user32.dll")]
        static extern bool ShowWindow(nint hWnd, int nCmdShow);
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool SetWindowText(nint hwnd, string lpString);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWindowVisible(IntPtr hWnd);

        [STAThread]
        static void Main()
        {
            AllocConsole();
            SetWindowText(GetConsoleWindow(), Properties.Resources.Title);
            ShowConsole(true);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            MainWindow window = new MainWindow();
            ScenarioWindow scn = new ScenarioWindow();
            scn.Show(window.DockPanel, WeifenLuo.WinFormsUI.Docking.DockState.Document);
            Application.Run(window);
        }
    }
}