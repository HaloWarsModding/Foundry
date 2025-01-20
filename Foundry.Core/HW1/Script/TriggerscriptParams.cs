using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Chef.HW1.Script
{
    public static class TriggerscriptParams
    {
        public static int DefaultWidth { get; set; } = 42;
        public static int HeaderHeight { get; } = 11;
        public static int FooterHeight { get; } = 2;
        public static int CommentHeight { get; } = 18;

        public static int LogicSpacing { get; } = 4;
        public static int LogicSectionSpacing { get; } = 8;

        public static int VarNameHeight { get; } = 4;
        public static int VarValHeight { get; } = 5;
        public static int VarHeight { get { return VarValHeight + VarNameHeight; } }
        public static int VarSpacing { get; } = 2;

        public static int Margin { get; } = 1;

        public static int TriggerSpacingMultiplier { get; } = 5;

        public static float ScaleViewMax { get; } = 6.5f;
        public static float ScaleViewMin { get; } = .01f;

        public static Color TextColor { get; } = Color.White;
        public static Color BodyColor { get; } = Color.FromArgb(90, 90, 90);
        public static Color TrimColor { get; } = Color.FromArgb(70, 70, 70);
        public static Color UnitColor { get; } = Color.FromArgb(60, 60, 60);
        public static Color TriggerHeaderColor { get; } = Color.CadetBlue;
        public static Color TriggerActiveColor { get; } = Color.Yellow;
        public static Color EffectHeaderColor { get; } = Color.RebeccaPurple;
        public static Color ConditionHeaderColor { get; } = Color.Crimson;
        public static Color BackgroundColor { get; } = Color.FromArgb(50, 50, 50);
    }

}
