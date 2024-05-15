using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Foundry.FoundryInstance;

namespace Foundry
{
    public partial class ConfigPrompt : Form
    {
        public ConfigPrompt(Config config, bool allowCancel)
        {
            InitializeComponent();
            DialogResult = DialogResult.Cancel;

            LocalConfig = new Config();

            Width = 550;

            int heightTracker = 12;
            foreach (Config.Param p in Enum.GetValues<Config.Param>())
            {
                LocalConfig.GetParamData(p).Value = config.GetParamData(p).Value;

                PictureBox statusimg = new PictureBox();
                statusimg.Dock = DockStyle.Fill;
                statusimg.Image = config.ParamValid(p) ? Properties.Resources.bullet_green : Properties.Resources.bullet_red;
                statusimg.SizeMode = PictureBoxSizeMode.StretchImage;


                TextBox textbox = new TextBox();
                textbox.Dock = DockStyle.Fill;
                textbox.TextChanged += (sender, e) =>
                {
                    LocalConfig.GetParamData(p).Value = textbox.Text;
                    if (LocalConfig.ParamValid(p))
                    {
                        statusimg.Image = Properties.Resources.bullet_green;
                    }
                    else
                    {
                        statusimg.Image = Properties.Resources.bullet_red;
                    }
                };
                textbox.Text = LocalConfig.GetParamData(p).Value;


                SplitContainer splitter = new SplitContainer();
                splitter.Location = new Point(6, 19);
                splitter.Size = new Size(500, 23);
                splitter.SplitterDistance = 480;
                splitter.Margin = new Padding(0, 0, 0, 0);
                splitter.IsSplitterFixed = true;
                splitter.Panel1.Controls.Add(textbox);
                splitter.Panel2.Controls.Add(statusimg);


                GroupBox group = new GroupBox();
                group.Text = LocalConfig.GetParamData(p).DisplayName;
                group.Location = new Point(12, heightTracker);
                group.Size = new Size(510, 48);
                group.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
                group.Controls.Add(splitter);


                Controls.Add(group);
                heightTracker += 50;
            }

            Height = heightTracker + 100;


            buttonApply.Click += (sender, e) =>
            {
                foreach (Config.Param p in Enum.GetValues<Config.Param>())
                {
                    if (!LocalConfig.ParamValid(p))
                    {
                        DialogResult = DialogResult.None;
                        return;
                    }
                }

                DialogResult = DialogResult.OK;
                Close();
            };
            buttonCancel.Visible = allowCancel;
            buttonCancel.Enabled = allowCancel;
            buttonCancel.Click += (sender, e) =>
            {
                DialogResult = DialogResult.Cancel;
                Close();
            };
        }

        public Config LocalConfig { get; private set; }
    }
}
