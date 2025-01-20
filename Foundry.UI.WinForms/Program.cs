using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Chef.HW1;
using Chef.HW1.Script;
using Chef.Win.UI;

namespace Chef.Win
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
        public static extern bool SetWindowText(nint hWnd, string lpString);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWindowVisible(nint hWnd);

        [STAThread]
        static void Main()
        {
            AllocConsole();
            SetWindowText(GetConsoleWindow(), Properties.Resources.Title);
            ShowConsole(true);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            MainWindow window = new MainWindow();

#if DEBUG
            AssetDatabase.Index("E:\\SteamLibrary\\steamapps\\common\\HaloWarsDE\\extract", window.Assets);
            //AssetDatabase.Index("E:\\repos\\emod", window.Assets);

            //Triggerscript t = new Triggerscript();
            //using (FileStream fs = new FileStream("E:\\Repos\\emod\\data\\triggerscripts\\ammo.triggerscript", FileMode.Open))
            //{
            //    TriggerscriptIO.ReadXml(fs, t);
            //}
            //using (FileStream fs = new FileStream("E:\\Repos\\emod\\data\\triggerscripts\\ammo_temp.triggerscript", FileMode.OpenOrCreate))
            //{
            //    TriggerscriptIO.WriteXml(fs, t);
            //}

            //AssetDatabase.GetOrLoadModel("art\\gorgon_01", window.Assets);

            //ScenarioWindow scn = new ScenarioWindow(window.Assets, window.GpuAssets);
            //scn.Show(window.DockPanel, WeifenLuo.WinFormsUI.Docking.DockState.Document);
            //scn.ScenarioName = "phxscn01";
            //scn.RefreshAssets();

            window.Browser.Update(window.Assets, window.GpuAssets, window.DockPanel);

            TriggerscriptWindow ts = new TriggerscriptWindow(window.Assets, window.GpuAssets);
            ts.ScriptName = "explode_01";
            ts.Show(window.DockPanel, WeifenLuo.WinFormsUI.Docking.DockState.Document);
#endif

            Application.Run(window);

            //when were all done lets just clean this up here.
            D3DViewport.Device3.Dispose();
            D3DViewport.Device2.Dispose();
            D3DViewport.Device1.Dispose();
            D3DViewport.Device.Dispose();
        }
    }
}