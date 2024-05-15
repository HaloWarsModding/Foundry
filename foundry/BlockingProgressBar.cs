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
    public partial class BlockingProgressBar : Form
    {
        public BlockingProgressBar()
        {
            InitializeComponent();
        }

        public string TaskDisplayText
        {
            get
            {
                return groupBox1.Text;
            }
            set
            {
                groupBox1.Text = value;
            }
        }
    }
}
