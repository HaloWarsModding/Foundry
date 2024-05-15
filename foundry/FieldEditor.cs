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
    public interface IField
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Group { get; set; }
        public IEnumerable<object> Options { get; set; }
        public bool Valid(object value);
        public object Value { get; set; }
    }

    public partial class FieldEditor : Form
    {
        public FieldEditor(IEnumerable<IField> fields)
        {
            InitializeComponent();

            if (fields.Count() == 0) return;
            Controls.Clear();

            List<IField> sorted = fields.ToList();
            sorted.Sort((l, r) => l.Group.CompareTo(r.Group));

            string currentGroup = "";
            int currentGroupY = 0;
            int intergroupY = 0;

            foreach (IField field in sorted)
            {
                if (field.Group != currentGroup)
                {
                    currentGroupY += intergroupY + 20;

                    GroupBox gb = new GroupBox();
                    gb.Location = new Point(gb.Location.X, currentGroupY);
                    gb.Text = currentGroup;
                    gb.Size = new Size(Size.Width, 5);

                    intergroupY = 0;
                }
            }
        }
    }
}
