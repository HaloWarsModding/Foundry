//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Foundry.Util
//{
//    public struct Point
//    {
//        public Point() { }
//        public Point(int x, int y) { X = x; Y = y; }
//        public int X { get; set; }
//        public int Y { get; set; }
//    }
//    public struct Size
//    {
//        public Size() { }
//        public Size(int width, int height) { Width = width; Height = height; }
//        public int Width { get; set; }
//        public int Height { get; set; }
//    }
//    public struct Bounds
//    {
//        public Point Location
//        {
//            get
//            {
//                return new Point(X, Y);
//            }
//            set
//            {
//                X = value.X;
//                Y = value.Y;
//            }
//        }
//        public Size Size
//        {
//            get
//            {
//                return new Size(Width, Height);
//            }
//            set
//            {
//                Width = value.Width;
//                Height = value.Height;
//            }
//        }

//        public int X { get; set; }
//        public int Y { get; set; }
//        public int Width { get; set; }
//        public int Height { get; set; }
//    }
//}
