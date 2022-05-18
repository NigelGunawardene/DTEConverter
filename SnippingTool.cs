using System;
using System.Drawing;
using System.Windows.Forms;

namespace DTEConverter
{

    internal partial class DTEContext
    {
        public partial class SnippingTool : Form
        {
            public static Image Snip()
            {
                var screenDetails = Screen.PrimaryScreen.Bounds;
                using (Bitmap bitmap = new Bitmap(screenDetails.Width, screenDetails.Height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb))
                {
                    using (Graphics graphics = Graphics.FromImage(bitmap))
                        graphics.CopyFromScreen(0, 0, 0, 0, bitmap.Size);
                    using (var snipper = new SnippingTool(bitmap))
                    {
                        if (snipper.ShowDialog() == DialogResult.OK)
                        {
                            return snipper.Image;
                        }
                    }
                    return null;
                }
            }

            public SnippingTool(Image screenShot)
            {
                //InitializeComponent();
                this.BackgroundImage = screenShot;
                this.ShowInTaskbar = false;
                this.FormBorderStyle = FormBorderStyle.None;
                this.WindowState = FormWindowState.Maximized;
                this.DoubleBuffered = true;
            }
            public Image Image { get; set; }

            private Rectangle rcSelect = new Rectangle();
            private Point pntStart;

            protected override void OnMouseDown(MouseEventArgs e)
            {
                // Start the snip on mouse down
                if (e.Button != MouseButtons.Left) return;
                pntStart = e.Location;
                rcSelect = new Rectangle(e.Location, new Size(0, 0));
                this.Invalidate();
            }
            protected override void OnMouseMove(MouseEventArgs e)
            {
                // Modify the selection on mouse move
                if (e.Button != MouseButtons.Left) return;
                int x1 = Math.Min(e.X, pntStart.X);
                int y1 = Math.Min(e.Y, pntStart.Y);
                int x2 = Math.Max(e.X, pntStart.X);
                int y2 = Math.Max(e.Y, pntStart.Y);
                rcSelect = new Rectangle(x1, y1, x2 - x1, y2 - y1);
                this.Invalidate();
            }
            protected override void OnMouseUp(MouseEventArgs e)
            {
                // Complete the snip on mouse-up
                if (rcSelect.Width <= 0 || rcSelect.Height <= 0) return;
                Image = new Bitmap(rcSelect.Width, rcSelect.Height);
                using (Graphics graphics = Graphics.FromImage(Image))
                {
                    graphics.DrawImage(this.BackgroundImage, new Rectangle(0, 0, Image.Width, Image.Height),
                        rcSelect, GraphicsUnit.Pixel);
                }
                DialogResult = DialogResult.OK;
            }
            protected override void OnPaint(PaintEventArgs e)
            {
                // Draw the current selection
                using (Brush br = new SolidBrush(Color.FromArgb(120, Color.White)))
                {
                    int x1 = rcSelect.X; int x2 = rcSelect.X + rcSelect.Width;
                    int y1 = rcSelect.Y; int y2 = rcSelect.Y + rcSelect.Height;
                    e.Graphics.FillRectangle(br, new Rectangle(0, 0, x1, this.Height));
                    e.Graphics.FillRectangle(br, new Rectangle(x2, 0, this.Width - x2, this.Height));
                    e.Graphics.FillRectangle(br, new Rectangle(x1, 0, x2 - x1, y1));
                    e.Graphics.FillRectangle(br, new Rectangle(x1, y2, x2 - x1, this.Height - y2));
                }
                using (Pen pen = new Pen(Color.Red, 3))
                {
                    e.Graphics.DrawRectangle(pen, rcSelect);
                }
            }
            protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
            {
                // Allow canceling the snip with the Escape key
                if (keyData == Keys.Escape) this.DialogResult = DialogResult.Cancel;
                return base.ProcessCmdKey(ref msg, keyData);
            }
        }
    }
}


//  multiple monitor reference code - https://github.com/thepirat000/Snipping-Ocr/tree/master/Snipping%20OCR/SnippingTool

//public sealed partial class SnippingTool : Form
//{
//    public static event EventHandler Cancel;
//    public static event EventHandler AreaSelected;
//    public static Image Image { get; set; }

//    private static SnippingTool[] _forms;
//    private Rectangle _rectSelection;
//    private Point _pointStart;

//    public SnippingTool(Image screenShot, int x, int y, int width, int height)
//    {
//        InitializeComponent();
//        BackgroundImage = screenShot;
//        BackgroundImageLayout = ImageLayout.Stretch;
//        ShowInTaskbar = false;
//        FormBorderStyle = FormBorderStyle.None;
//        StartPosition = FormStartPosition.Manual;
//        SetBounds(x, y, width, height);
//        WindowState = FormWindowState.Maximized;
//        DoubleBuffered = true;
//        Cursor = Cursors.Cross;
//        TopMost = true;
//    }

//    private void OnCancel(EventArgs e)
//    {
//        Cancel?.Invoke(this, e);
//    }

//    private void OnAreaSelected(EventArgs e)
//    {
//        AreaSelected?.Invoke(this, e);
//    }

//    private void CloseForms()
//    {
//        for (int i = 0; i < _forms.Length; i++)
//        {
//            _forms[i].Dispose();
//        }
//    }

//    public static void Snip()
//    {
//        var screens = ScreenHelper.GetMonitorsInfo();
//        _forms = new SnippingTool[screens.Count];
//        for (int i = 0; i < screens.Count; i++)
//        {
//            int hRes = screens[i].HorizontalResolution;
//            int vRes = screens[i].VerticalResolution;
//            int top = screens[i].MonitorArea.Top;
//            int left = screens[i].MonitorArea.Left;
//            var bmp = new Bitmap(hRes, vRes, PixelFormat.Format32bppPArgb);
//            using (var g = Graphics.FromImage(bmp))
//            {
//                g.CopyFromScreen(left, top, 0, 0, bmp.Size);
//            }
//            _forms[i] = new SnippingTool(bmp, left, top, hRes, vRes);
//            _forms[i].Show();
//        }
//    }

//    #region Overrides
//    protected override void OnMouseDown(MouseEventArgs e)
//    {
//        // Start the snip on mouse down
//        if (e.Button != MouseButtons.Left)
//        {
//            return;
//        }
//        _pointStart = e.Location;
//        _rectSelection = new Rectangle(e.Location, new Size(0, 0));
//        Invalidate();
//    }

//    protected override void OnMouseMove(MouseEventArgs e)
//    {
//        // Modify the selection on mouse move
//        if (e.Button != MouseButtons.Left)
//        {
//            return;
//        }
//        int x1 = Math.Min(e.X, _pointStart.X);
//        int y1 = Math.Min(e.Y, _pointStart.Y);
//        int x2 = Math.Max(e.X, _pointStart.X);
//        int y2 = Math.Max(e.Y, _pointStart.Y);
//        _rectSelection = new Rectangle(x1, y1, x2 - x1, y2 - y1);
//        Invalidate();
//    }

//    protected override void OnMouseUp(MouseEventArgs e)
//    {
//        // Complete the snip on mouse-up
//        if (_rectSelection.Width <= 0 || _rectSelection.Height <= 0)
//        {
//            CloseForms();
//            OnCancel(new EventArgs());
//            return;
//        }
//        Image = new Bitmap(_rectSelection.Width, _rectSelection.Height);
//        var hScale = BackgroundImage.Width / (double)Width;
//        var vScale = BackgroundImage.Height / (double)Height;
//        using (Graphics gr = Graphics.FromImage(Image))
//        {

//            gr.DrawImage(BackgroundImage,
//                new Rectangle(0, 0, Image.Width, Image.Height),
//                new Rectangle((int)(_rectSelection.X * hScale), (int)(_rectSelection.Y * vScale), (int)(_rectSelection.Width * hScale), (int)(_rectSelection.Height * vScale)),
//                GraphicsUnit.Pixel);
//        }
//        CloseForms();
//        OnAreaSelected(new EventArgs());
//    }

//    protected override void OnPaint(PaintEventArgs e)
//    {
//        // Draw the current selection
//        using (Brush br = new SolidBrush(Color.FromArgb(120, Color.White)))
//        {
//            int x1 = _rectSelection.X;
//            int x2 = _rectSelection.X + _rectSelection.Width;
//            int y1 = _rectSelection.Y;
//            int y2 = _rectSelection.Y + _rectSelection.Height;
//            e.Graphics.FillRectangle(br, new Rectangle(0, 0, x1, Height));
//            e.Graphics.FillRectangle(br, new Rectangle(x2, 0, Width - x2, Height));
//            e.Graphics.FillRectangle(br, new Rectangle(x1, 0, x2 - x1, y1));
//            e.Graphics.FillRectangle(br, new Rectangle(x1, y2, x2 - x1, Height - y2));
//        }
//        using (Pen pen = new Pen(Color.Red, 2))
//        {
//            e.Graphics.DrawRectangle(pen, _rectSelection);
//        }
//    }

//    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
//    {
//        // Allow canceling the snip with the Escape key
//        if (keyData == Keys.Escape)
//        {
//            Image = null;
//            CloseForms();
//            OnCancel(new EventArgs());
//        }
//        return base.ProcessCmdKey(ref msg, keyData);
//    }
//    #endregion
//}



//SnippingTool.AreaSelected += OnAreaSelected;
//SnippingTool.Snip();

//private static void OnAreaSelected(object sender, EventArgs e)
//{
//    var bmp = SnippingTool.Image;
//    // Do something with the bitmap
//    //...
//}


//public class DeviceInfo
//{
//    public string DeviceName { get; set; }
//    public int VerticalResolution { get; set; }
//    public int HorizontalResolution { get; set; }
//    public Rectangle MonitorArea { get; set; }
//}
//public static class ScreenHelper
//{
//    private const int DektopVertRes = 117;
//    private const int DesktopHorzRes = 118;
//    [StructLayout(LayoutKind.Sequential)]
//    internal struct Rect
//    {
//        public int left;
//        public int top;
//        public int right;
//        public int bottom;
//    }
//    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
//    internal struct MONITORINFOEX
//    {
//        public int Size;
//        public Rect Monitor;
//        public Rect WorkArea;
//        public uint Flags;
//        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
//        public string DeviceName;
//    }
//    private delegate bool MonitorEnumDelegate(IntPtr hMonitor, IntPtr hdcMonitor, ref Rect lprcMonitor, IntPtr dwData);
//    [DllImport("user32.dll")]
//    private static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumDelegate lpfnEnum, IntPtr dwData);
//    [DllImport("gdi32.dll")]
//    private static extern IntPtr CreateDC(string lpszDriver, string lpszDevice, string lpszOutput, IntPtr lpInitData);
//    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
//    private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFOEX lpmi);
//    [DllImport("User32.dll")]
//    private static extern int ReleaseDC(IntPtr hwnd, IntPtr dc);
//    [DllImport("gdi32.dll")]
//    private static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

//    private static List<DeviceInfo> _result;

//    public static List<DeviceInfo> GetMonitorsInfo()
//    {
//        _result = new List<DeviceInfo>();
//        EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, MonitorEnum, IntPtr.Zero);
//        return _result;
//    }

//    private static bool MonitorEnum(IntPtr hMonitor, IntPtr hdcMonitor, ref Rect lprcMonitor, IntPtr dwData)
//    {
//        var mi = new MONITORINFOEX();
//        mi.Size = Marshal.SizeOf(typeof(MONITORINFOEX));
//        bool success = GetMonitorInfo(hMonitor, ref mi);
//        if (success)
//        {
//            var dc = CreateDC(mi.DeviceName, mi.DeviceName, null, IntPtr.Zero);
//            var di = new DeviceInfo
//            {
//                DeviceName = mi.DeviceName,
//                MonitorArea = new Rectangle(mi.Monitor.left, mi.Monitor.top, mi.Monitor.right - mi.Monitor.right, mi.Monitor.bottom - mi.Monitor.top),
//                VerticalResolution = GetDeviceCaps(dc, DektopVertRes),
//                HorizontalResolution = GetDeviceCaps(dc, DesktopHorzRes)
//            };
//            ReleaseDC(IntPtr.Zero, dc);
//            _result.Add(di);
//        }
//        return true;
//    }
//}

//=================================================================================================================================================================================

//Public Class SnippingTool


//    Private Shared _Screen As Screen

//    Private Shared BitmapSize As Size

//    Private Shared Graph As Graphics


//    Public Shared Function Snip(ByVal screen As Screen) As Image

//        _Screen = screen

//        Dim bmp As New Bitmap(screen.Bounds.Width, screen.Bounds.Height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb)

//        Dim gr As Graphics = Graphics.FromImage(bmp)

//        Graph = gr


//        gr.SmoothingMode = Drawing2D.SmoothingMode.None '###

//        BitmapSize = bmp.Size


//        Using snipper = New SnippingTool(bmp)

//            snipper.Location = New Point(screen.Bounds.Left, screen.Bounds.Top)

//            If snipper.ShowDialog() = DialogResult.OK Then
//                Return snipper.Image
//            End If

//        End Using

//        Return Nothing


//    End Function



//    Public Sub New(ByVal screenShot As Image)
//        InitializeComponent()
//        Me.BackgroundImage = screenShot
//        Me.ShowInTaskbar = False
//        Me.FormBorderStyle = FormBorderStyle.None


//        'Me.WindowState = FormWindowState.Maximized

//        Me.DoubleBuffered = True
//    End Sub
//    Public Property Image() As Image
//        Get
//            Return m_Image
//        End Get
//        Set(ByVal value As Image)
//            m_Image = Value
//        End Set
//    End Property
//    Private m_Image As Image


//    Private rcSelect As New Rectangle()
//    Private pntStart As Point

//    Protected Overrides Sub OnMouseDown(ByVal e As MouseEventArgs)
//        ' Start the snip on mouse down
//        If e.Button<> MouseButtons.Left Then
//            Return
//        End If
//        pntStart = e.Location
//        rcSelect = New Rectangle(e.Location, New Size(0, 0))
//        Me.Invalidate()
//    End Sub
//    Protected Overrides Sub OnMouseMove(ByVal e As MouseEventArgs)
//        ' Modify the selection on mouse move
//        If e.Button<> MouseButtons.Left Then
//            Return
//        End If
//        Dim x1 As Integer = Math.Min(e.X, pntStart.X)
//        Dim y1 As Integer = Math.Min(e.Y, pntStart.Y)
//        Dim x2 As Integer = Math.Max(e.X, pntStart.X)
//        Dim y2 As Integer = Math.Max(e.Y, pntStart.Y)
//        rcSelect = New Rectangle(x1, y1, x2 - x1, y2 - y1)
//        Me.Invalidate()
//    End Sub


//    Protected Overrides Sub OnMouseUp(ByVal e As MouseEventArgs)
//        ' Complete the snip on mouse-up
//        If rcSelect.Width <= 0 OrElse rcSelect.Height <= 0 Then
//            Return
//        End If
//        Image = New Bitmap(rcSelect.Width, rcSelect.Height)
//        Using gr As Graphics = Graphics.FromImage(Image)
//            gr.DrawImage(Me.BackgroundImage, New Rectangle(0, 0, Image.Width, Image.Height), rcSelect, GraphicsUnit.Pixel)
//        End Using
//        DialogResult = DialogResult.OK
//    End Sub
//    Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)
//        ' Draw the current selection
//        Using br As Brush = New SolidBrush(Color.FromArgb(120, Color.White))
//            Dim x1 As Integer = rcSelect.X
//            Dim x2 As Integer = rcSelect.X + rcSelect.Width
//            Dim y1 As Integer = rcSelect.Y
//            Dim y2 As Integer = rcSelect.Y + rcSelect.Height
//            e.Graphics.FillRectangle(br, New Rectangle(0, 0, x1, Me.Height))
//            e.Graphics.FillRectangle(br, New Rectangle(x2, 0, Me.Width - x2, Me.Height))
//            e.Graphics.FillRectangle(br, New Rectangle(x1, 0, x2 - x1, y1))
//            e.Graphics.FillRectangle(br, New Rectangle(x1, y2, x2 - x1, Me.Height - y2))
//        End Using
//        Using pen As New Pen(Color.Red, 3)
//            e.Graphics.DrawRectangle(pen, rcSelect)
//        End Using
//    End Sub
//    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, ByVal keyData As Keys) As Boolean
//        ' Allow canceling the snip with the Escape key
//        If keyData = Keys.Escape Then
//            Me.DialogResult = DialogResult.Cancel
//        End If
//        Return MyBase.ProcessCmdKey(msg, keyData)
//    End Function

//    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
//        MyBase.OnLoad(e)
//        Me.Size = New Size(_Screen.Bounds.Width, _Screen.Bounds.Height)
//        Dim area = _Screen.WorkingArea
//        Graph.CopyFromScreen(area.X, area.Y, area.Y, area.Y, BitmapSize)
//    End Sub

//End Class