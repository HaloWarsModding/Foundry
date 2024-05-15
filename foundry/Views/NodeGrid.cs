using Foundry.util;
using System.Drawing.Drawing2D;

namespace Foundry.Views
{
    public class NodeGridDragArgs
    {
        public Point From { get; set; }
        public Point To { get; set; }
    }

    public class NodeGrid : BaseView
    {
        public event EventHandler<NodeGridDragArgs> GridDrag;


        public Size GridSize { get; set; } = new Size(130, 35);
        public Size GridSpacing { get; set; } = new Size(30, 0);
        private Matrix ViewMatrix { get; set; } = new Matrix();

        private Brush BackgroundBrush { get; set; } = new SolidBrush(Color.Gray);
        private Pen GridPen { get; set; } = new Pen(Color.DarkGray, 1);

        public NodeGrid(FoundryInstance instance) : base(instance)
        {
            ViewTick += (s, e) => { OnTick(); };
            ViewDraw += (s, e) => { OnDraw(e.Graphics, e.ClipRectangle); };
        }

        public Point GridToPixel(Point grid)
        {
            return new Point(
                    grid.X * (GridSize.Width + GridSpacing.Width),
                    grid.Y * (GridSize.Height + GridSpacing.Height)
                );
        }
        public Size GridToPixel(Size grid)
        {
            return new Size(
                    (grid.Width * (GridSize.Width + GridSpacing.Width)) - GridSpacing.Width,
                    (grid.Height * (GridSize.Height + GridSpacing.Height)) - GridSpacing.Height
                );
        }
        public Rectangle GridToPixel(Rectangle grid)
        {
            return new Rectangle(
                GridToPixel(new Point(grid.X, grid.Y)), 
                GridToPixel(new Size(grid.Width, grid.Height))
                );
        }

        public Point PixelToGrid(Point pixel)
        {
            pixel += GridSpacing / 2;
            int gridX = pixel.X / (GridSize.Width + GridSpacing.Width);
            int gridY = pixel.Y / (GridSize.Height + GridSpacing.Height);
            return new Point(gridX, gridY);
        }

        public Point TransformedMousePos(int x, int y)
        {
            Point[] mouse = new Point[1] { new Point(x, y) };
            ViewMatrix.Inverted().TransformPoints(mouse);
            return mouse[0];
        }

        private void OnTick()
        {
            MouseState mouse = GetMouseState();
            Point mouseTransformed = TransformedMousePos(mouse.X, mouse.Y);

            if (mouse.leftDown)
            {
                Console.WriteLine(PixelToGrid(new Point(mouseTransformed.X, mouseTransformed.Y)));
            }
            else if (mouse.middleDown)
            {
                ViewMatrix.Translate(mouse.deltaX, mouse.deltaY);
            }
        }
        private void OnDraw(Graphics g, Rectangle clip)
        {
            g.Transform = ViewMatrix;

            Point[] transformedClip = new Point[1] { new Point(clip.X, clip.Y) };
            ViewMatrix.Inverted().TransformPoints(transformedClip);
            Rectangle viewClip = new Rectangle(transformedClip[0].X, transformedClip[0].Y, clip.Width, clip.Height);

            g.FillRectangle(BackgroundBrush, viewClip);

            int xGrid = viewClip.X - (viewClip.X % (GridSize.Width + GridSpacing.Width)) - (GridSize.Width + GridSpacing.Width);
            int yGrid = viewClip.Y - (viewClip.Y % (GridSize.Height + GridSpacing.Height)) - (GridSize.Height + GridSpacing.Height);
            for (int x = xGrid; x < viewClip.Right; x += GridSize.Width + GridSpacing.Width)
            {
                for (int y = yGrid; y < viewClip.Bottom; y += GridSize.Height + GridSpacing.Height)
                {
                    //vertical lines
                    g.DrawLine(GridPen,
                        new Point(x, y),
                        new Point(x, y + GridSize.Height)
                        );
                    if (GridSpacing.Width > 0)
                    {
                        g.DrawLine(GridPen,
                            new Point(x + GridSize.Width, y),
                            new Point(x + GridSize.Width, y + GridSize.Height)
                        );
                    }
                    
                    //horizontal lines
                    g.DrawLine(GridPen,
                        new Point(x, y),
                        new Point(x + GridSize.Width, y)
                        );
                    if (GridSpacing.Height > 0)
                    {
                        g.DrawLine(GridPen,
                            new Point(x,                  y + GridSize.Height),
                            new Point(x + GridSize.Width, y + GridSize.Height));
                    }
                }
            }

            //foreach(NodeGridOccupant o in GetOccupants())
            //{
            //    g.FillRectangle(new SolidBrush(o.HeaderColor), GridRect(o.GridX, o.GridY, 1, 1));
            //    g.FillRectangle(new SolidBrush(o.BodyColor), GridRect(o.GridX, o.GridY + 1, 1, o.BodyHeight));
            //}
        }
    }
}
