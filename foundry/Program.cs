using Foundry;
using Foundry.HW1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Foundry
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            WorkspaceUIWinforms window = new WorkspaceUIWinforms();
            Application.Run(window);
        }
    }
}