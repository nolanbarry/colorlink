using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace flow
{
    public partial class SolvingVisualizer : Form
    {
        public Grid grid;

        public SolvingVisualizer(SolvingGrid grid)
        {
            InitializeComponent();
            this.grid = new Grid(grid);
            Paint += new PaintEventHandler(OnPaint);
        }

        public void UpdateGrid(Grid grid)
        {
            this.grid = grid;
            Refresh();
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            DrawGridTo(g, grid, new Point(ClientRectangle.Width / 2, ClientRectangle.Height / 2));
        }

        private float circleMargin = 0.3f;
        private float pathSize = 0.3f;
        private void DrawGridTo(Graphics drawTo, Grid gridData, Point center)
        {
            int height = gridData.gridHeight;
            int width = gridData.gridWidth;
            int squareLength = 40;
            Bitmap image = (Bitmap)flowindow.DrawGrid(height, width, squareLength);
            Graphics g = Graphics.FromImage(image);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int[,] startPoints = gridData.grid;
            float margin = circleMargin / 2 * squareLength;
            float reverseMargin = squareLength * (1 - circleMargin);
            for (int i = 0; i < startPoints.GetLength(0); i++)
            {
                for (int j = 0; j < startPoints.GetLength(1); j++)
                {
                    if (startPoints[i, j] != -1)
                    {
                        g.FillEllipse(new SolidBrush(flowindow.colorPallet[startPoints[i, j]]), i * squareLength + margin, j * squareLength + margin, reverseMargin, reverseMargin);
                    }
                }
            }
            foreach (Path p in gridData.pathsOfColors)
            {
                if (p != null)
                {
                    flowindow.DrawPathTo(g, squareLength, p, pathSize);
                }

            }
            drawTo.DrawImage(image, new Point(center.X - image.Width / 2, center.Y - image.Height / 2));
        }
    }
}
