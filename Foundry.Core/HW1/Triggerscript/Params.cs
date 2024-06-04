using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing; //cross platform System.Drawing.Primitives is used.

namespace Foundry.HW1.Triggerscript
{
    public static class Params
    {
        public static int DefaultWidth { get; set; } = 30;
        public static int HeaderHeight { get; } = 6;
        public static int FooterHeight { get; } = 2;
        public static int CommentHeight { get; } = 12;

        public static int LogicSpacing { get; } = 1;
        public static int LogicSectionSpacing { get; } = 3;

        public static int VarNameHeight { get; } = 2;
        public static int VarValHeight { get; } = 3;
        public static int VarHeight { get { return VarValHeight + VarNameHeight; } }
        public static int VarSpacing { get; } = 1;

        public static int Margin { get; } = 1;

        public static int TriggerSpacingMultiplier { get; } = 3;

        public static float ScaleViewMax { get; } = 4.5f;
        public static float ScaleViewMin { get; } = .01f;

        public static Color TextColor { get; } = Color.White;
        public static Color BodyColor { get; } = Color.FromArgb(90, 90, 90);
        public static Color TrimColor { get; } = Color.FromArgb(70, 70, 70);
        public static Color TriggerHeaderColor { get; } = Color.CadetBlue;
        public static Color EffectHeaderColor { get; } = Color.RebeccaPurple;
        public static Color ConditionHeaderColor { get; } = Color.Crimson;
        public static Color ContainerColor { get; } = Color.Black;
    }

}
