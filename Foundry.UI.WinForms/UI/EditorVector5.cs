using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chef.Win.UI
{
    public partial class EditorVector5 : UserControl
    {
        public EditorVector5()
        {
            InitializeComponent();

            xval.ValueChanged += (s, e) => { ValueChanged?.Invoke(this, EventArgs.Empty); };
            yval.ValueChanged += (s, e) => { ValueChanged?.Invoke(this, EventArgs.Empty); };
            zval.ValueChanged += (s, e) => { ValueChanged?.Invoke(this, EventArgs.Empty); };
            wval.ValueChanged += (s, e) => { ValueChanged?.Invoke(this, EventArgs.Empty); };
        }

        event EventHandler? ValueChanged;
        public Vector4 Value 
        {
            get
            {
                return new Vector4(
                    (float)xval.Value,
                    (float)yval.Value,
                    (float)zval.Value,
                    (float)wval.Value);
            }
            set
            {
                xval.Value = (decimal)value.X;
                yval.Value = (decimal)value.Y;
                zval.Value = (decimal)value.Z;
                wval.Value = (decimal)value.W;
            }
        }
        public string TextX { 
            get { return xlabel.Text; } 
            set { xlabel.Text = value; }
        }
        public string TextY
        {
            get { return ylabel.Text; }
            set { ylabel.Text = value; }
        }
        public string TextZ
        {
            get { return zlabel.Text; }
            set { zlabel.Text = value; }
        }
        public string TextW
        {
            get { return wlabel.Text; }
            set { wlabel.Text = value; }
        }
    }
}
