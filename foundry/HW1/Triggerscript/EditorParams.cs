using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.HW1.Triggerscript
{
    public static class EditorParams
    {
        public static int HeaderHeight { get; } = 12;
        public static int FooterHeight { get; } = 4;
        public static int CommentHeight { get; } = 26;

        public static int LogicSpacing { get; } = 3;
        public static int LogicSectionSpacing { get; } = 7;

        public static int VarNameHeight { get; } = 4;
        public static int VarValHeight { get; } = 5;
        public static int VarHeight { get { return VarValHeight + VarNameHeight; } }
        public static int VarSpacing { get; } = 2;

        public static int Margin { get; } = 1;

        public static Font TitleFont { get; } = new Font("Consolas", 2.7f, FontStyle.Regular);
        public static Font SubtitleFont { get; } = new Font("Consolas", 2.55f, FontStyle.Regular);
        public static Font TextFont { get; } = new Font("Consolas", 2.4f, FontStyle.Regular);

        public static Color TextColor { get; } = Color.White;
        public static Color BodyColor { get; } = Color.FromArgb(90, 90, 90);
        public static Color TrimColor { get; } = Color.FromArgb(70, 70, 70);
        public static Color TriggerHeaderColor { get; } = Color.CadetBlue;
        public static Color EffectHeaderColor { get; } = Color.RebeccaPurple;
        public static Color ConditionHeaderColor { get; } = Color.Crimson;
        public static Color ContainerColor { get; } = Color.Black;
    }

}
