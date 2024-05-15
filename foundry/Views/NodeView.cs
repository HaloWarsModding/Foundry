using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.IO;
using Newtonsoft.Json;
using Foundry;
using System.Numerics;
using Foundry.Util;
using Foundry.util;
using static Foundry.BaseView;
using System.Windows.Forms.VisualStyles;
using System.Text.RegularExpressions;

namespace Foundry
{
	public enum BaseNodeSocketDirection
	{
		In,
		Out,
	}
	public enum BaseNodeSocketFlags
	{
		Restrict = 1,
		MultiConnect = 2,
	}
	public class BaseNodeSocket
	{
		public BaseNodeSocket(BaseNode node)
		{
			Node = node;
			Text = "Socket";
			Offset = 0;
			Color = Color.Black;

			_Connections = new List<BaseNodeSocket>();
		}

		public BaseNode Node { get; private set; }

		//behavior
		public string Text { get; set; }
		public string Restriction { get; set; }
		public Color Color { get; set; }
		public BaseNodeSocketDirection Direction { get; set; }
		public bool HasFlag(BaseNodeSocketFlags flag)
		{
			return (Flags & flag) > 0;
		}
		public BaseNodeSocketFlags Flags { get; set; }

		//connections
		public IReadOnlyList<BaseNodeSocket> Connections { get { return _Connections; } }
		private List<BaseNodeSocket> _Connections { get; set; }
		public bool Connect(BaseNodeSocket socket)
		{
			if (this._Connections.Contains(socket)) return false;
			if (!this.HasFlag(BaseNodeSocketFlags.MultiConnect) && this._Connections.Count > 0) return false;

			if (socket._Connections.Contains(socket)) return false;
			if (!socket.HasFlag(BaseNodeSocketFlags.MultiConnect) && socket._Connections.Count > 0) return false;

			if (this.Node == socket.Node) return false;
			if (this.Direction == socket.Direction) return false;
			if (this.Restriction != socket.Restriction) return false;

			this._Connections.Add(socket);
			this.SocketConnected?.Invoke(this, new ConnectionArgs() { FromSocket = this, ToSocket = socket });

			socket._Connections.Add(this);
			socket.SocketConnected?.Invoke(this, new ConnectionArgs() { FromSocket = socket, ToSocket = this });

			return true;
		}
		public bool Disconnect(BaseNodeSocket socket)
		{
			this.SocketDisconnected?.Invoke(this, new ConnectionArgs() { FromSocket = this, ToSocket = socket });
			this._Connections.Remove(socket);

			socket.SocketDisconnected?.Invoke(this, new ConnectionArgs() { FromSocket = socket, ToSocket = this });
			socket._Connections.Remove(this);

			return true;
		}
		public bool DisconnectAll()
		{
			foreach (BaseNodeSocket socket in Connections.ToList())
			{
				Disconnect(socket);
			}
			return true;
		}

		//location
		public float Offset { get; set; }
		public static float HalfExtents { get; private set; } = 15;
		public RectangleF Bounds
		{
			get
			{
				float xoffs = Direction == BaseNodeSocketDirection.Out ? Node.Width : 0;
				return new RectangleF(
					Node.Location.X + xoffs - HalfExtents,
					Node.BodyBounds.Y + Offset - HalfExtents,
					HalfExtents * 2,
					HalfExtents * 2
					);
			}
		}
		public RectangleF MouseBounds
		{
			get
			{
				RectangleF ret = Bounds;
				ret.Inflate(
					HalfExtents,
					HalfExtents
					);
				return ret;
			}
		}
		public PointF Center
		{
			get
			{
				return new PointF(
					Bounds.X + HalfExtents,
					Bounds.Y + HalfExtents
					);
			}
		}

		public class ConnectionArgs
		{
			public BaseNodeSocket FromSocket { get; set; }
			public BaseNodeSocket ToSocket { get; set; }
		}
		public event EventHandler<ConnectionArgs> SocketConnected;
		public event EventHandler<ConnectionArgs> SocketDisconnected;
	}
	public class BaseNodeControl
	{
		public static float Padding = 12;
		public BaseNodeControl(BaseNode node)
		{
			Node = node;

			Height = Padding * 3;

			BackgroundBrush = new SolidBrush(Color.FromArgb(40, 40, 40));
			BackgroundPen = new Pen(BackgroundBrush, 3);

			ForegroundBrush = new SolidBrush(Color.White);
			ForegroundPen = new Pen(ForegroundBrush, 3);

			TextBrush = new SolidBrush(Color.White);
		}

		public BaseNode Node { get; private set; }
		public float Offset { get; set; }
		public float Height { get; set; }
		public PointF Location
		{
			get
			{
				return Node.BodyBounds.Location + new SizeF(Padding, Offset);
			}
		}
		public RectangleF Bounds
        {
			get
            {
				return new RectangleF(
					Location.X,
					Location.Y,
					Node.BodyBounds.Width - (Padding * 2),
					Height);
            }
        }

		public SolidBrush BackgroundBrush { get; private set; }
		public Pen BackgroundPen { get; private set; }
		public SolidBrush ForegroundBrush { get; private set; }
		public Pen ForegroundPen { get; private set; }
		public SolidBrush TextBrush { get; private set; }

		//public void Draw(Graphics g, BaseNode.DrawFlags flags)
		//{
		//	OnDraw?.Invoke(this, new OnDrawArgs() { Graphics = g, DrawFlags = flags });
		//}
		public void Keyboard(char c)
		{
			OnKeyboard?.Invoke(this, c);
		}
		public void Clicked(Point pos)
		{
			OnClicked?.Invoke(this, pos);
		}
		public void ClickedOff(Point pos)
		{
			OnClickedOff?.Invoke(this, pos);
		}

		//public class OnDrawArgs
		//      {
		//	public Graphics Graphics { get; set; }
		//	//public BaseNode.DrawFlags DrawFlags { get; set; }
		//      }
		//public event EventHandler<OnDrawArgs> OnDraw;
		public event EventHandler<char> OnKeyboard;
		public event EventHandler<Point> OnClicked;
		public event EventHandler<Point> OnClickedOff;
	}
    public class BaseNode
	{
		//Public
		public virtual string Text { get; set; } = "";
		public virtual string Subtext { get; set; } = "";
		public virtual PointF Location { get; set; } = new PointF(0, 0);

		public float Width { get; protected set; } = 500;
		public float HeaderHeight { get; protected set; } = 100;
		public float BodyHeight { get; protected set; } = 200;
		public RectangleF Bounds
		{
			get
			{
				return new RectangleF(
					Location.X,
					Location.Y,
					Width,
					HeaderHeight + BodyHeight);
			}
		}
		public RectangleF HeaderBounds
		{
			get
			{
				return new RectangleF(
				Location.X,
				Location.Y,
				Width,
				HeaderHeight);
			}
		}
		public RectangleF BodyBounds
		{
			get
			{
				return new RectangleF(
				Location.X,
				HeaderBounds.Y + HeaderHeight,
				Width,
                BodyHeight);
			}
		}


  //      //Drawing
  //      public Color HeaderColor
		//{
		//	get { return HeaderBrush.Color; }
		//	set { HeaderBrush.Color = value; }
		//}
		//public Color TextColor
		//{
		//	get { return TextBrush.Color; }
		//	set { TextBrush.Color = value; }
		//}
		//public Color BackgroundColor 
		//{
		//	get 
		//	{
		//		return BodyBrush.Color; 
		//	}
		//}
		//private Pen BorderDefaultPen;
		//private Pen BorderSelectedPen;
		//private Pen ConnectionPen;
		//private Pen ConnectionBackingPen;
		//private SolidBrush BodyBrush;
		//private SolidBrush HeaderBrush;
		//private SolidBrush TextBrush;

		//[Flags]
		//public enum DrawFlags
		//{
		//	Border = 1,
		//	Fast = 2,
		//	Hover = 4,
		//}
		//public virtual void DrawBase(Graphics g, DrawFlags flags)
		//{
		//	bool border = (flags & DrawFlags.Border) > 0;
		//	bool fast = (flags & DrawFlags.Fast) > 0;

		//	//node bounds
		//	g.FillRectangle(HeaderBrush, Rectangle.Round(HeaderBounds));
		//	g.FillRectangle(BodyBrush, Rectangle.Round(BodyBounds));

		//	//border
		//	if (border) g.DrawRectangle(BorderSelectedPen, Rectangle.Round(Bounds));
		//	else g.DrawRectangle(BorderDefaultPen, Rectangle.Round(Bounds));

		//}
		//public virtual void DrawSockets(Graphics g, DrawFlags flags)
		//{
		//	bool border = (flags & DrawFlags.Border) > 0;
		//	bool fast = (flags & DrawFlags.Fast) > 0;

		//	if (!fast)
		//	{
		//		foreach (BaseNodeSocket socket in _Sockets)
		//		{
		//			g.FillRectangle(new SolidBrush(socket.Color), socket.Bounds);

		//			if (border) g.DrawRectangle(BorderSelectedPen, Rectangle.Round(socket.Bounds));
		//			else g.DrawRectangle(BorderDefaultPen, Rectangle.Round(socket.Bounds));
		//		}
		//	}
		//}
		//public virtual void DrawConnections(Graphics g, DrawFlags flags)
		//{
		//	bool border = (flags & DrawFlags.Border) > 0;
		//	bool fast = (flags & DrawFlags.Fast) > 0;

		//	foreach (BaseNodeSocket socket in _Sockets)
		//	{
		//		//draw output socket connections
		//		if (socket.Direction == BaseNodeSocketDirection.Out)
		//		{
		//			foreach (BaseNodeSocket connection in socket.Connections)
		//			{
		//				//backing
		//				//g.DrawLine(ConnectionBackingPen, socket.Center, connection.Center);

		//				//colored line
		//				ConnectionPen.Color = socket.Color;
		//				g.DrawLine(ConnectionPen, socket.Center, connection.Center);
		//			}
		//		}
		//	}
		//}
		//public virtual void DrawText(Graphics g, DrawFlags flags)
		//{
		//	bool border = (flags & DrawFlags.Border) > 0;
		//	bool fast = (flags & DrawFlags.Fast) > 0;

		//	//node text
		//	if (!fast)
		//	{
		//		g.DrawString(Text,
		//				   new Font("Consolas", HeaderHeight / 3.5f, FontStyle.Regular),
		//				   TextBrush,
		//				   Location.X + 7,
		//				   Location.Y + (HeaderHeight / 3.5f),
		//				   new StringFormat()
		//				   {
		//					   LineAlignment = StringAlignment.Center,
		//				   });
		//		g.DrawString(Subtext,
		//				   new Font("Consolas", HeaderHeight / 4.5f, FontStyle.Regular),
		//				   TextBrush,
		//				   Location.X + 10,
		//				   Location.Y + (HeaderHeight / 3.5f) * 2.35f,
		//				   new StringFormat()
		//				   {
		//					   LineAlignment = StringAlignment.Center,
		//				   });
		//	}

		//	//socket text
		//	foreach (BaseNodeSocket socket in _Sockets)
		//	{
		//		//draw socket text
		//		if (!fast)
		//		{
		//			float xoffs = socket.Direction == BaseNodeSocketDirection.Out ? -BaseNodeSocket.HalfExtents : BaseNodeSocket.HalfExtents;
		//			g.DrawString(
		//				socket.Text,
		//				new Font("Consolas", socket.Bounds.Height * .75f, FontStyle.Regular),
		//				TextBrush,
		//				socket.Center.X + (xoffs * 1.5f),
		//				socket.Center.Y,
		//				new StringFormat()
		//				{
		//					FormatFlags = socket.Direction == BaseNodeSocketDirection.Out ? StringFormatFlags.DirectionRightToLeft : 0,
		//					LineAlignment = StringAlignment.Center,
		//				});
		//		}
		//	}
		//}


		////Events
		//public class SocketConnectedArgs
		//{
		//	public BaseNodeSocket FromSocket { get; set; }
		//	public BaseNodeSocket ToSocket { get; set; }
		//}
		//public event EventHandler<SocketConnectedArgs> OnSocketConnected;
		//public event EventHandler<SocketConnectedArgs> OnSocketDisconnected;
		//public class MovedArgs
		//{
		//	public BaseNode Node { get; set; }
		//	public PointF LastLocation { get; set; }
		//	public PointF CurrentLocation { get; set; }
		//}
		//public event EventHandler<MovedArgs> NodeMoved;

		//public class NameChangedArgs
		//{
		//	public BaseNode Node { get; set; }
		//	public string Name { get; set; }
		//}
		//public event EventHandler<NameChangedArgs> OnNameChanged;

		//public class MouseArgs
		//{
		//	public MouseState Mouse { get; set; }
		//}
		//public event EventHandler<MouseArgs> OnMouse;
	}


	public class BaseNodeEditorData
	{
		public BaseNodeEditorData(FoundryInstance i)
		{
			_Nodes = new List<BaseNode>();
			_SelectedNodes = new List<BaseNode>();
		}

		public IReadOnlyList<BaseNode> Nodes
		{
			get
			{
				return _Nodes;
			}
		}
		private List<BaseNode> _Nodes { get; set; }
		public IReadOnlyList<BaseNode> SelectedNodes
		{
			get
			{
				return _SelectedNodes;
			}
		}
		private List<BaseNode> _SelectedNodes { get; set; }
		public void AddNode(BaseNode node)
		{
			if (_Nodes.Contains(node)) return;
			_Nodes.Add(node);
			NodeAdded?.Invoke(this, node);
		}
		public void RemoveNode(BaseNode node)
		{
			if (_Nodes.Contains(node))
			{
				//node.ClearConnections();
				_Nodes.Remove(node);
				if (_SelectedNodes.Contains(node)) _SelectedNodes.Remove(node);
				NodeRemoved?.Invoke(this, node);
			}
		}
		public void ClearNodes()
		{
			foreach (BaseNode node in Nodes)
			{
				NodeRemoved?.Invoke(this, node);
			}

			_Nodes.Clear();
			_SelectedNodes.Clear();
		}

		public void SelectNode(BaseNode node)
		{
			if (_SelectedNodes.Contains(node)) return;
			_SelectedNodes.Add(node);
		}
		public void DeselectNode(BaseNode node)
		{
			if (!_SelectedNodes.Contains(node)) return;
			_SelectedNodes.Remove(node);
		}
		public void DeselectAllNodes()
		{
			_SelectedNodes.Clear();
		}
		public BaseNode GetFirstNodeAtPoint(int mx, int my)
		{
			var nodesReversed = Nodes.ToList();
			nodesReversed.Reverse();

			foreach (BaseNode n in nodesReversed)
			{
				if (n.Bounds.Contains(mx, my))
				{
					return n;
				}
			}

			return null;
		}

		public event EventHandler<BaseNode> NodeAdded;
		public event EventHandler<BaseNode> NodeRemoved;
	}


	public class NodeView : BaseView
	{
		public NodeView(FoundryInstance i) : base(i)
		{
			SelectedSocket = null;
			SelectedControl = null;

			RenderIntervalMs = 0;


            ViewTick += (sender, e) => { OnTick(); };
			//OnPageClickL += (sender, e) => { OnClickL(); };
			//OnPageDragL += (sender, e) => { OnDragL(); };
			//OnPageReleaseL += (sender, e) => { OnReleaseL(); };
			ViewTextInput += (sender, e) => { OnTextInput(e); };

			Form.Paint += OnPaint;
		}

		public BaseNodeEditorData NodeData { get; set; }

		private enum CurrentMouseState
		{
			None,
			DraggingNodes,
			DraggingMarquee,
			DraggingSocket,
			EditingControl
		}
		private CurrentMouseState mouseState;
		private BaseNodeSocket SelectedSocket;
		private BaseNodeControl SelectedControl;
		private float zoomMax = 8f, zoomMin = .1f;
		public float currentViewX = 0, currentViewY = 0, currentViewZoom = .25f;
		private Matrix viewMatrix = new Matrix();
		private Point markedMousePos = new Point();
		public Point GetTransformedMousePos()
		{
			var m = GetMouseState();
			Point[] p = new Point[] { new Point(m.X, m.Y) };
			viewMatrix.Inverted().TransformPoints(p);
			return p[0];
		}
		public MouseState GetTransformedMouse()
		{
			MouseState ret = new MouseState();
			var m = GetMouseState();
			ret.deltaX = (int)(m.deltaX * (1 / currentViewZoom));
			ret.deltaY = (int)(m.deltaY * (1 / currentViewZoom));
			ret.deltaScroll = m.deltaScroll;
			ret.leftDownLast = m.leftDownLast;
			ret.rightDownLast = m.rightDownLast;
			ret.middleDownLast = m.middleDownLast;
			ret.leftDown = m.leftDown;
			ret.rightDown = m.rightDown;
			ret.middleDown = m.middleDown;
            Point[] p = new Point[] { new Point(m.X, m.Y) };
            viewMatrix.Inverted().TransformPoints(p);
			ret.X = p[0].X;
			ret.Y = p[0].Y;
			return ret;
		}

		private void OnClickL()
		{
			int mx = GetTransformedMousePos().X;
			int my = GetTransformedMousePos().Y;

			//if were just clicking on already the selected control, do nothing.
			//if(SelectedControl != null)
   //         {
			//	if (SelectedControl.Bounds.Contains(mx, my))
			//	{
			//		SelectedControl.Clicked(new Point(mx, my));
			//		return;
			//	}
			//	else
			//	{
			//		SelectedControl.ClickedOff(new Point(mx, my));
			//		SelectedControl = null;
			//	}
   //         }

			//var node = NodeData.GetFirstNodeAtPoint(mx, my);
			//if (node != null)
			//{
				////select control
				//foreach (BaseNodeControl c in node.Controls)
				//{
				//	if (c.Bounds.Contains(mx, my))
				//	{
				//		NodeData.DeselectAllNodes();
				//		SelectedControl = c;
				//		mouseState = CurrentMouseState.EditingControl;
				//		c.Clicked(new Point(mx, my));
				//		return;
				//	}
				//}

				//select socket
				//foreach (BaseNodeSocket socket in node.Sockets)
				//{
				//	if (socket.MouseBounds.Contains(mx, my))
				//	{
				//		NodeData.DeselectAllNodes();
				//		if (!socket.HasFlag(BaseNodeSocketFlags.MultiConnect) &&
				//			socket.Connections.Count > 0)
				//		{
				//			SelectedSocket = socket.Connections[0];
				//			socket.Disconnect(socket.Connections[0]);
				//		}
				//		else
				//		{
				//			SelectedSocket = socket;
				//		}
				//		mouseState = CurrentMouseState.DraggingSocket;
				//		return;
				//	}
				//}
			//	SelectedSocket = null;

			//	//select node
			//	if (NodeData.SelectedNodes.Contains(node))
			//	{
			//		mouseState = CurrentMouseState.DraggingNodes;
			//		return;
			//	}

			//	if (!GetKeyIsDown(Keys.Shift))
			//	{
			//		NodeData.DeselectAllNodes();
			//	}

			//	NodeData.SelectNode(node);
			//	mouseState = CurrentMouseState.DraggingNodes;
			//	return;
			//}
			//else
			//{
			//	//start dragging marquee
			//	NodeData.DeselectAllNodes();
			//	mouseState = CurrentMouseState.DraggingMarquee;
			//	markedMousePos.X = mx;
			//	markedMousePos.Y = my;
			//	return;
			//}
		}
		private void OnDragL()
		{
			int mx = GetTransformedMousePos().X;
			int my = GetTransformedMousePos().Y;

			var nodesReversed = NodeData.Nodes.ToList();
			nodesReversed.Reverse();

			//nodes are selected, drag them
			if (mouseState == CurrentMouseState.DraggingNodes)
			{
				foreach (BaseNode n in NodeData.SelectedNodes)
				{
					n.Location += new SizeF(GetMouseState().deltaX * (1 / currentViewZoom), GetMouseState().deltaY * (1 / currentViewZoom));
				}
			}

			//marquee is dragging, update it
			if (mouseState == CurrentMouseState.DraggingMarquee)
			{
				foreach (BaseNode n in nodesReversed)
				{
					if (n.Bounds.IntersectsWith(
						new RectangleF(
							Math.Min(markedMousePos.X, mx),
							Math.Min(markedMousePos.Y, my),
							Math.Abs(mx - markedMousePos.X),
							Math.Abs(my - markedMousePos.Y)))
						)
					{
						NodeData.SelectNode(n);
					}
					else
					{
						NodeData.DeselectNode(n);
					}
				}
			}
		}
		private void OnReleaseL()
		{
			int mx = GetTransformedMousePos().X;
			int my = GetTransformedMousePos().Y;

			//socket was dragging
			if (mouseState == CurrentMouseState.DraggingSocket)
			{
				//foreach node
				foreach (BaseNode n in NodeData.Nodes)
				{
					////foreach socket in that node
					//foreach (BaseNodeSocket s in n.Sockets)
					//{
					//	if (s == SelectedSocket) continue;

					//	//if mouse was in that socket
					//	if (s.MouseBounds.Contains(mx, my))
					//	{
					//		s.Connect(SelectedSocket);
					//		//TrySetEdited();
					//		SelectedSocket = null;
					//	}
					//}
				}
				mouseState = CurrentMouseState.None;
			}

			if (mouseState == CurrentMouseState.DraggingMarquee)
            {
				mouseState = CurrentMouseState.None;
            }

		}
		private void OnTick()
		{
			var m = GetMouseState();

			if (mouseState != CurrentMouseState.EditingControl)
			{
				SelectedControl = null;
			}
			if (mouseState != CurrentMouseState.DraggingSocket)
			{
				SelectedSocket = null;
			}


			#region delete
			if (GetKeyIsDown(Keys.Delete))
			{
				foreach (BaseNode n in NodeData.SelectedNodes.ToList())
				{
					//NodeData.RemoveNode(n);
					//TrySetEdited();
				}
			}

			#endregion


			//TODO:
			#region copy/paste
			//if (GetKeyIsDown(Keys.ControlKey) && GetKeyIsDown(Keys.C) && !GetKeyWasDown(Keys.C))
			//{
			//	CopyGraph();
			//}
			//if (GetKeyIsDown(Keys.ControlKey) && GetKeyIsDown(Keys.V) && !GetKeyWasDown(Keys.V))
			//{
			//	PasteGraph();
			//	TrySetEdited();
			//}
			#endregion


			#region mouse
			//scroll wheel (pan)
			if (m.middleDown)
			{
				currentViewX += (m.deltaX) * 1 / currentViewZoom;
				currentViewY += (m.deltaY) * 1 / currentViewZoom;
			}
			//scroll wheel (zoom)
			currentViewZoom += m.deltaScroll * (currentViewZoom / 1000);
			currentViewZoom = currentViewZoom < zoomMin ? zoomMin : currentViewZoom;
			currentViewZoom = currentViewZoom > zoomMax ? zoomMax : currentViewZoom;
			#endregion

			//matrix stuff must be done at the very end.
			#region matrix
			//create the view matrix for the editor's page (order matters).
			viewMatrix.Reset();
			viewMatrix.Translate(Form.Size.Width / 2, Form.Size.Height / 2);
			viewMatrix.Scale(currentViewZoom, currentViewZoom);
			viewMatrix.Translate(currentViewX, currentViewY);
			#endregion
		}
		private void OnTextInput(char c)
		{
			if (mouseState == CurrentMouseState.EditingControl)
			{
				SelectedControl.Keyboard(c);
			}
		}

		private Pen gridPen = new Pen(Color.FromArgb(255, 150, 150, 150));
		private int majorGridSpace = 350;
		protected void OnPaint(object o, PaintEventArgs e)
		{
			float fastThreshold = .075f;

			int mx = GetTransformedMousePos().X;
			int my = GetTransformedMousePos().Y;

			Graphics g = e.Graphics;
			g.SmoothingMode = SmoothingMode.AntiAlias;
			g.InterpolationMode = InterpolationMode.HighQualityBicubic;
			g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
			g.Transform = viewMatrix;

			//TODO: replace with config (all colors)
			g.Clear(Color.DarkGray);

			//gets and transforms the viewable bounds by the inverted view matrix.
			PointF[] invertedMatrixPoints = new PointF[]
			{
				new PointF(e.ClipRectangle.Left, e.ClipRectangle.Top),
				new PointF(e.ClipRectangle.Right, e.ClipRectangle.Bottom)
			};
			viewMatrix.Inverted().TransformPoints(invertedMatrixPoints);
			float invLeft = invertedMatrixPoints[0].X;
			float invRight = invertedMatrixPoints[1].X;
			float invTop = invertedMatrixPoints[0].Y;
			float invBottom = invertedMatrixPoints[1].Y;

			//draw grid lines from the starting lines' offset.
			//top-most and left-most grid lines.
			if (currentViewZoom < fastThreshold)
			{
				majorGridSpace = 10000;
			}
			else
			{
				majorGridSpace = 500;
			}

			int xOffs = (int)Math.Round(invLeft / majorGridSpace) * majorGridSpace;
			int yOffs = (int)Math.Round(invTop / majorGridSpace) * majorGridSpace;
			for (int x = xOffs; x < invRight; x += majorGridSpace)
			{
				g.DrawLine(gridPen, x, invTop, x, invBottom);
			}
			for (int y = yOffs; y < invBottom; y += majorGridSpace)
			{
				g.DrawLine(gridPen, invLeft, y, invRight, y);
			}

			//draw current temp connection
			if (mouseState == CurrentMouseState.DraggingSocket)
			{
				//just a line. TODO: make this more uniform with the actual socket connections. Perhaps make the socket itself draw this?
				g.DrawLine(
					new Pen(Color.Blue, 5.0f),
					SelectedSocket.Center,
					new PointF(mx, my)
					);
			}

			List<BaseNode> visibleNodes = new List<BaseNode>();
			//draw nodes
			//foreach (BaseNode n in NodeData.Nodes)
			//{
			//	if (n.Bounds.IntersectsWith(new RectangleF(invLeft, invTop, invRight - invLeft, invBottom - invTop)))
			//	{
			//		visibleNodes.Add(n);
			//	}
			//}


			//foreach (BaseNode n in visibleNodes)
			//{
			//	BaseNode.DrawFlags flags = 0;

			//	if (NodeData.SelectedNodes.Contains(n))
			//		flags |= BaseNode.DrawFlags.Border;
			//	if (currentViewZoom < fastThreshold)
			//		flags |= BaseNode.DrawFlags.Fast;

			//	n.DrawBase(g, flags);
			//	n.DrawSockets(g, flags);
			//	n.DrawText(g, flags);

			//	//draw controls
			//	foreach (BaseNodeControl c in n.Controls)
			//	{
			//		BaseNode.DrawFlags cflags = 0;

			//		if (c == SelectedControl)
			//			cflags |= BaseNode.DrawFlags.Border;
			//		if (currentViewZoom < fastThreshold)
			//			cflags |= BaseNode.DrawFlags.Fast;

			//		c.Draw(g, cflags);
			//	}

			//}
			//foreach (BaseNode n in visibleNodes)
			//{
			//	BaseNode.DrawFlags flags = 0;

			//	if (NodeData.SelectedNodes.Contains(n))
			//		flags |= BaseNode.DrawFlags.Border;
			//	if (currentViewZoom < fastThreshold)
			//		flags |= BaseNode.DrawFlags.Fast;

			//	n.DrawConnections(g, flags);
			//}


			//draw marquee (current mouse dragged selection)
			if (mouseState == CurrentMouseState.DraggingMarquee)
			{
				//just a rectangle
				g.FillRectangle(new SolidBrush(Color.FromArgb(100, 10, 10, 10)),
					Math.Min(markedMousePos.X, mx),
					Math.Min(markedMousePos.Y, my),
					Math.Abs(mx - markedMousePos.X),
					Math.Abs(my - markedMousePos.Y));
			}
		}
	}
}