using KSoft;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using static Foundry.FoundryInstance;

namespace Foundry
{
    public class DoubleBufferedDockContent : DockContent
    {
        public DoubleBufferedDockContent() : base()
        {
            DoubleBuffered = true;
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.DoubleBuffer,
                true);
        }
    }

    public class ViewDrawArgs
    {
        public IntPtr HWND { get; set; }
        public Rectangle ClipRectangle { get; set; }
        public Graphics Graphics { get; set; }
    }

    public class MouseState
    {
        public bool leftDown, rightDown, middleDown;
        public bool leftDownLast, rightDownLast, middleDownLast;
        public int X, Y;
        public int deltaX, deltaY, deltaScroll;
    }

    public abstract class BaseView
    {
        //TODO: Get rid of these
        public event EventHandler OnPageClickL;
        public event EventHandler OnPageDragL;
        public event EventHandler OnPageReleaseL;


        ///////////////////////////////////////////////////////////////////////////////////////
        //// public
        public BaseView(FoundryInstance i)
        {
            Instance = i;
            Form = new DoubleBufferedDockContent();

            CurMouseState = new MouseState();
            CurDownKeys = new List<Keys>();
            CurDownKeysLast = new List<Keys>();

            Form.ControlAdded += Internal_ControlAdded;
            Form.Resize       += Internal_Resize;
            Form.Paint        += Internal_Draw;

            Form.MouseMove    += Internal_MouseMoved;
            Form.MouseWheel   += Internal_MouseWheelMoved;
            Form.MouseDown    += Internal_MouseButtonDown;
            Form.MouseUp      += Internal_MouseButtonUp;

            Form.KeyDown      += Internal_KeyDown;
            Form.KeyUp        += Internal_KeyUp;
            Form.KeyPress     += Internal_KeyPress;

            Form.GotFocus += (s, e) => { ViewGotFocus?.Invoke(this, EventArgs.Empty); };
            Form.LostFocus += (s, e) => { ViewLostFocus?.Invoke(this, EventArgs.Empty); };

            RenderTimer = new Stopwatch();
            RenderIntervalMs = 16;
            RenderTimer.Start();
        }

        public event EventHandler ViewTick;
        public event EventHandler<ViewDrawArgs> ViewDraw;
        public event EventHandler ViewResized;

        public event EventHandler ViewMouseInput;
        public event EventHandler ViewKeyboardInput;
        public event EventHandler<char> ViewTextInput;

        public event EventHandler ViewShown;
        public event EventHandler ViewClosed;

        public event EventHandler ViewGotFocus;
        public event EventHandler ViewLostFocus;

        public event EventHandler ViewSaved;

        public FoundryInstance Instance {  get;  private set;  }
        public long RenderIntervalMs { get; set; }
        public DockContent Form { get; private set; }

        public void Show(FoundryInstance i, DockState state)
        {
            Form.Show(i.MainDockPanel, state);
            ViewShown?.Invoke(this, EventArgs.Empty);
        }
        public void Show(BaseView page, DockState state)
        {
            Form.Show(page.Form.DockPanel, state);
            ViewShown?.Invoke(this, EventArgs.Empty);
        }
        public void Close(bool force = false)
        {
            Form.Close();
            ViewClosed?.Invoke(this, EventArgs.Empty);
        }
        public void AddElement(object element)
        {
            if (element == null) return;
            if (element is Control)
            {
                Form.Controls.Add(element as Control);
            }
        }
        public void Redraw()
        {
            Form.Invalidate();
        }

        public MouseState GetMouseState()
        {
            return CurMouseState;
        }
        public IReadOnlyList<Keys> GetKeysState()
        {
            return CurDownKeys;
        }
        public bool GetKeyIsDown(Keys k)
        {
            return CurDownKeys.Contains(k);
        }
        public bool GetKeyWasDown(Keys k)
        {
            return CurDownKeysLast.Contains(k);
        }

        public void Save()
        {
            ViewSaved?.Invoke(this, EventArgs.Empty);
        }


        ///////////////////////////////////////////////////////////////////////////////////////
        //// private
        private MouseState CurMouseState { get; set; }
        private List<Keys> CurDownKeys { get; set; }
        private List<Keys> CurDownKeysLast { get; set; }
        private Stopwatch RenderTimer { get; set; }

        private void Internal_Shown(object o, EventArgs e)
        {
            Instance.RegisterPage(this);
            ViewShown?.Invoke(this, EventArgs.Empty);
        }
        private void Internal_Closed(object o, FormClosingEventArgs e)
        {
            Instance.UnregisterPage(this);
            ViewClosed?.Invoke(this, EventArgs.Empty);
        }
        private void Internal_ControlAdded(object o, ControlEventArgs e)
        {
            e.Control.ControlAdded += Internal_ControlAdded;

            e.Control.MouseMove += Internal_MouseMoved;
            e.Control.MouseWheel += Internal_MouseWheelMoved;
            e.Control.MouseDown += Internal_MouseButtonDown;
            e.Control.MouseUp += Internal_MouseButtonUp;

            e.Control.KeyDown += Internal_KeyDown;
            e.Control.KeyUp += Internal_KeyUp;

            e.Control.KeyPress += Internal_KeyPress;
        }
        private void Internal_Resize(object o, EventArgs e)
        {
            if (!Form.Disposing)
            {
                ViewResized?.Invoke(this, null);
            }
        }
        private void Internal_Tick()
        {
            //Main Tick
             ViewTick?.Invoke(this, null);
            //catch (Exception e)
            //{
            //    Instance.AppendLog(LogEntryType.Error, "Internal_Tick(): OnTick() encountered an error. See console for details.", true,
            //        string.Format("--Error info:\n--Editor type: {0}\n--Exception information: {1}'\n'--Stacktrace:{2}", GetType().Name, e.Message, e.StackTrace));
            //}

            //Dragging
            //if (CurMouseState.leftDown && !CurMouseState.leftDownLast)
            //{
            //    try { OnPageClickL?.Invoke(this, null); }
            //    catch (Exception e)
            //    {
            //        Instance.AppendLog(LogEntryType.Error, "Internal_Tick(): OnDragStart() encountered an error. See console for details.", true,
            //            string.Format("--Error info:\n--Editor type: {0}\n--Exception information: {1}'\n'--Stacktrace:{2}", GetType().Name, e.Message, e.StackTrace));
            //    }
            //}
            //if (CurMouseState.leftDown && CurMouseState.leftDownLast)
            //{
            //    try { OnPageDragL?.Invoke(this, null); }
            //    catch (Exception e)
            //    {
            //        Instance.AppendLog(LogEntryType.Error, "Internal_Tick(): OnDragging() encountered an error. See console for details.", true,
            //            string.Format("--Error info:\n--Editor type: {0}\n--Exception information: {1}'\n'--Stacktrace:{2}", GetType().Name, e.Message, e.StackTrace));
            //    }
            //}
            //if (!CurMouseState.leftDown && CurMouseState.leftDownLast)
            //{
            //    try { OnPageReleaseL?.Invoke(this, null); }
            //    catch (Exception e)
            //    {
            //        Instance.AppendLog(LogEntryType.Error, "Internal_Tick(): OnDragStop() encountered an error. See console for details.", true,
            //            string.Format("--Error info:\n--Editor type: {0}\n--Exception information: {1}'\n'--Stacktrace:{2}", GetType().Name, e.Message, e.StackTrace));
            //    }
            //}

            //Draw
            if (RenderTimer.ElapsedMilliseconds > RenderIntervalMs)
            {
                Redraw();
                RenderTimer.Restart();
            }
        }
        private void Internal_Draw(object o, PaintEventArgs e)
        {
            ViewDraw?.Invoke(this, new ViewDrawArgs()
            {
                HWND = Form.Handle,
                ClipRectangle = e.ClipRectangle,
                Graphics = e.Graphics
            });
        }

        private void Internal_MouseMoved(object o, MouseEventArgs e)
        {
            CurMouseState.deltaX = e.X - CurMouseState.X;
            CurMouseState.deltaY = e.Y - CurMouseState.Y;
            CurMouseState.X = e.X;
            CurMouseState.Y = e.Y;

            Internal_Tick();

            CurMouseState.deltaX = 0;
            CurMouseState.deltaY = 0;
        }
        private void Internal_MouseWheelMoved(object o, MouseEventArgs e)
        {
            CurMouseState.deltaScroll = e.Delta;

            Internal_Tick();

            CurMouseState.deltaScroll = 0;
        }
        private void Internal_MouseButtonDown(object o, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                CurMouseState.leftDown = true;
            }
            if (e.Button == MouseButtons.Right)
            {
                CurMouseState.rightDown = true;
            }
            if (e.Button == MouseButtons.Middle)
            {
                CurMouseState.middleDown = true;
            }

            Internal_Tick();

            CurMouseState.leftDownLast = CurMouseState.leftDown;
            CurMouseState.rightDownLast = CurMouseState.rightDown;
            CurMouseState.middleDownLast = CurMouseState.middleDown;
        }
        private void Internal_MouseButtonUp(object o, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                CurMouseState.leftDown = false;
            }
            if (e.Button == MouseButtons.Right)
            {
                CurMouseState.rightDown = false;
            }
            if (e.Button == MouseButtons.Middle)
            {
                CurMouseState.middleDown = false;
            }

            Internal_Tick();

            CurMouseState.leftDownLast = CurMouseState.leftDown;
            CurMouseState.rightDownLast = CurMouseState.rightDown;
            CurMouseState.middleDownLast = CurMouseState.middleDown;
        }

        private void Internal_KeyDown(object o, KeyEventArgs e)
        {
            if (!CurDownKeys.Contains(e.KeyCode))
            {
                CurDownKeys.Add(e.KeyCode);
            }

            Internal_Tick();

            CurDownKeysLast = CurDownKeys.ToList();
        }
        private void Internal_KeyUp(object o, KeyEventArgs e)
        {
            if (CurDownKeys.Contains(e.KeyCode))
            {
                CurDownKeys.RemoveAll(x => x == e.KeyCode);
            }

            Internal_Tick();

            CurDownKeysLast = CurDownKeys.ToList();
        }
        private void Internal_KeyPress(object o, KeyPressEventArgs e)
        {
            ViewTextInput?.Invoke(this, e.KeyChar);
            Internal_Tick();
        }
    }
}
