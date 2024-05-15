using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Foundry
{
    public partial class CreateWorkspaceWizard : Form
    {
        public CreateWorkspaceWizard()
        {
            InitializeComponent();
        }

        public string WorkspaceName { get { return textboxName.Text; } }
        public string WorkspaceLocation { get { return textboxLocation.Text; } }
        public bool WorkspaceUnpackDefault { get { return optionUnpackBaseEras.Checked; } }

        private void buttonOpenFileBrowser_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                textboxLocation.Text = fbd.SelectedPath;
            }
        }
    }
}
