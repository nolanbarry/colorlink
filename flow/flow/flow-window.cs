using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;

namespace flow
{
    public partial class flowindow : Form
    {
        private Timer ticker;
        public static Color[] colorPallet = new Color[]
        {
            ColorTranslator.FromHtml("#ffc3a0"), // pink
            ColorTranslator.FromHtml("#ff7373"), // salmon
            ColorTranslator.FromHtml("#f6b36e"), // orange
            ColorTranslator.FromHtml("#f5f66e"), // yellow
            ColorTranslator.FromHtml("#7fffd4"), // light green
            ColorTranslator.FromHtml("#6ef3f6"), // baby blue
            ColorTranslator.FromHtml("#3399ff"), // dark blue
            ColorTranslator.FromHtml("#9c6ef6")  // purple
        };
        private static readonly Pen gridOutline = new Pen(ColorTranslator.FromHtml("#383838"));

        public flowindow()
        {
            InitializeComponent();

            DoubleBuffered = true;

            // timer setup
            ticker = new Timer();
            ticker.Interval = 17;
            ticker.Tick += new EventHandler(OnTick);
            ticker.Start();

            ReplaceEdgeDockedControls();

            Paint += new PaintEventHandler(OnPaint);
            Resize += new EventHandler(OnResize);
        }

        private void OnTick(object sender, EventArgs e)
        {
            lblMousePosition.Text = PointToClient(MousePosition).ToString();
            Refresh();
        }

        private void OnResize(object sender, EventArgs e)
        {
            ReplaceEdgeDockedControls();
            Refresh();
        }

        private void ReplaceEdgeDockedControls()
        {
            // mouse position
            Point dummyPoint = new Point();
            lblMousePosition.Dock = DockStyle.Bottom;
            dummyPoint.Y = lblMousePosition.Location.Y;
            lblMousePosition.Dock = DockStyle.Left;
            dummyPoint.X = lblMousePosition.Location.X;
            lblMousePosition.Dock = DockStyle.None;
            lblMousePosition.Location = dummyPoint;
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.HighQuality;
            
            for (int i = 0; i < colorPallet.Length; i++)
            {
                g.FillEllipse(new SolidBrush(colorPallet[i]), new Rectangle(i * 25, 0, 25, 25));
            }
            
            DrawGrid(g, new Point(ClientRectangle.Width / 2, ClientRectangle.Height / 2), 5, 5);
        }

        private void DrawGrid(Graphics g, Point center, int height, int width)
        {
            DrawGrid(g, center, height, width, CalculateMaximumSquareSize(height, width));
        }

        private int horizontalMargin = 20;
        private int verticalMargin = 50;
        private int CalculateMaximumSquareSize(int height, int width)
        {
            int x, y, sizeByYAxis, sizeByXAxis;
            x = ClientRectangle.Width - horizontalMargin * 2;
            y = ClientRectangle.Height - verticalMargin * 2;
            sizeByXAxis = x / width;
            sizeByYAxis = y / height;
            if (sizeByXAxis < sizeByYAxis) return sizeByXAxis;
            else return sizeByYAxis;
        }

        private void DrawGrid(Graphics drawTo, Point center, int height, int width, int squareLength)
        {
            gridOutline.Width = 1;
            Bitmap grid = new Bitmap(width * squareLength + 1, height * squareLength + 1);
            Graphics g = Graphics.FromImage(grid);
            for(int i = 0; i <= width; i++)
            {
                Point p1 = new Point(i * squareLength, 0);
                Point p2 = new Point(i * squareLength, grid.Height - 1);
                g.DrawLine(gridOutline, p1, p2);
            }
            for(int i = 0; i <= height; i++)
            {
                Point p1 = new Point(0, i * squareLength);
                Point p2 = new Point(grid.Width - 1, i * squareLength);
                g.DrawLine(gridOutline, p1, p2);
            }
            Point centerOffset = new Point(center.X - grid.Width / 2, center.Y - grid.Height / 2);
            drawTo.DrawImage(grid, centerOffset);            
        }

        private void DrawGridWithDots
    }
}
