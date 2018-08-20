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
using System.IO;

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
        public Grid currentLevel;
        public int mouseX;
        public int mouseY;
        public Image imgGrid { get; private set; }
        public Path currentPath;

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
            MouseMove += new MouseEventHandler(OnMouseMove);
            MouseDown += new MouseEventHandler(OnMouseDown);
            MouseUp += new MouseEventHandler(OnMouseUp);
            KeyPress += new KeyPressEventHandler(OnKeyPress);

            OnResize(this, new EventArgs());

            do
            {
                currentLevel = new Grid(Generator.GenerateLevel(5, 5)); //FileParsing.ParseFileIntoGrid(0, "Levels1.txt");
            } while (!FileParsing.IsItSolvable(currentLevel.grid));
            mouseX = 0;
            mouseY = 0;
        }

        private void OnTick(object sender, EventArgs e)
        {
            lblMousePosition.Text = "";
            horizontalMargin = (int)(0.05f * ClientRectangle.Width);
            verticalMargin = (int)(0.05f * ClientRectangle.Height);
            Refresh();
        }

        private void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == 'n')
            {
                do
                {
                    currentLevel = new Grid(Generator.GenerateLevel(5, 5));
                } while (!FileParsing.IsItSolvable(currentLevel.grid));
                currentPath = null;
            }
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (mouseX >= 0 && mouseX < currentLevel.gridWidth 
                && mouseY >= 0 && mouseY < currentLevel.gridHeight
                && currentLevel.grid[mouseX, mouseY] >= 0)
              currentPath = new Path(new Point(mouseX, mouseY), currentLevel.grid[mouseX, mouseY]);
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (currentPath != null)
            {
                //if (mouseX >= 0 && mouseX < currentLevel.gridWidth && mouseY >= 0 && mouseY < currentLevel.gridHeight)
                currentLevel.EditPathOfColor(currentPath);
                currentPath = null;
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            // calculate where the mouse is on the grid
            if (imgGrid != null)
            {
                int pMouseX = mouseX;
                int pMouseY = mouseY;
                Point gridOrigin = new Point(ClientRectangle.Width / 2 - imgGrid.Width / 2, ClientRectangle.Height / 2 - imgGrid.Height / 2);
                Point mouseOnClient = PointToClient(MousePosition);
                Point mouseOffset = new Point(mouseOnClient.X - gridOrigin.X, mouseOnClient.Y - gridOrigin.Y);
                double squareLength = CalculateMaximumSquareSize(currentLevel.gridWidth, currentLevel.gridHeight);
                mouseX = (int) Math.Floor(mouseOffset.X / squareLength);
                mouseY = (int) Math.Floor(mouseOffset.Y / squareLength);

                // add mouse position to path if applicable
                if (currentPath != null)
                {
                    if (new Point(pMouseX, pMouseY) == currentPath.lastPoint)
                    {
                        List<Point> arr = currentPath.asCoordinateArray.ToList<Point>();
                        if (arr.Count > 1)
                        arr.RemoveAt(arr.Count - 2);

                        if (!arr.Contains(new Point(mouseX, mouseY)))
                            AddNewPointToPath(pMouseX, pMouseY, mouseX, mouseY);
                    }
                }
            }
        }

        /// <summary>
        /// If the mouse position has changed since the last frame, add it to the path. When called, assumes currentPath is active and the mouse is down.
        /// </summary>
        /// <param name="xold">Pre-frame mouse x.</param>
        /// <param name="yold">Pre-frame mouse y.</param>
        /// <param name="xnew">Post-frame mouse x.</param>
        /// <param name="ynew">Post-frame mouse y<./param>
        private void AddNewPointToPath(int xold, int yold, int xnew, int ynew)
        {
            if (xnew >= 0 && xnew < currentLevel.gridWidth // new position is within grid x bounds
                && ynew >= 0 && ynew < currentLevel.gridHeight) // new position is within grid y bounds
                if ((currentLevel.grid[xold, yold] != currentPath.color || currentPath.path.Count < 2 
                        || new Point(xnew, ynew) == currentPath.asCoordinateArray[currentPath.asCoordinateArray.Length - 2]) // not moving through an endpoint
                    && currentLevel.grid[xnew, ynew] == -1 || currentLevel.grid[xnew, ynew] == currentPath.color) // not moving to a different color's point    
                {
                    if (currentLevel.FlattenGrid(currentPath.color)[xnew, ynew] == -1)
                    {
                        if (xold != xnew)
                        {
                            if (xnew > xold) currentPath.Add(Path.Direction.Right);
                            else currentPath.Add(Path.Direction.Left);
                        }
                        else if (yold != ynew)
                        {
                            if (ynew > yold) currentPath.Add(Path.Direction.Down);
                            else currentPath.Add(Path.Direction.Up);
                        }
                    }
                }
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

            DrawGridTo(g, new Point(ClientRectangle.Width / 2, ClientRectangle.Height / 2), currentLevel);
        }

        private Image DrawGrid(int height, int width)
        {
            return DrawGrid(height, width, CalculateMaximumSquareSize(width, height));
        }

        private int horizontalMargin;
        private int verticalMargin;
        /// <summary>
        /// Calculates the maximum length each square can be in a grid given the grid's column and row count.
        /// </summary>
        /// <param name="height">Number of rows in grid.</param>
        /// <param name="width">Number of columns in grid.</param>
        /// <returns></returns>
        private int CalculateMaximumSquareSize(int width, int height)
        {
            int x, y, sizeByYAxis, sizeByXAxis;
            x = ClientRectangle.Width - horizontalMargin * 2;
            y = ClientRectangle.Height - verticalMargin * 2;
            sizeByXAxis = x / width;
            sizeByYAxis = y / height;
            if (sizeByXAxis < sizeByYAxis) return sizeByXAxis;
            else return sizeByYAxis;
        }

        public static Image DrawGrid(int height, int width, int squareLength)
        {
            gridOutline.Width = 1;
            Bitmap grid = new Bitmap(width * squareLength + 1, height * squareLength + 1);
            Graphics g = Graphics.FromImage(grid);
            for (int i = 0; i <= width; i++)
            {
                Point p1 = new Point(i * squareLength, 0);
                Point p2 = new Point(i * squareLength, grid.Height - 1);
                g.DrawLine(gridOutline, p1, p2);
            }
            for (int i = 0; i <= height; i++)
            {
                Point p1 = new Point(0, i * squareLength);
                Point p2 = new Point(grid.Width - 1, i * squareLength);
                g.DrawLine(gridOutline, p1, p2);
            }
            return grid;
        }

        private float circleMargin = 0.3f;
        private float pathSize = 0.3f;
        private void DrawGridTo(Graphics drawTo, Point center, Grid gridData)
        {
            int height = gridData.gridHeight;
            int width = gridData.gridWidth;
            int squareLength = CalculateMaximumSquareSize(width, height);
            Bitmap image = (Bitmap)DrawGrid(height, width, squareLength);
            Graphics g = Graphics.FromImage(image);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int[,] startPoints = gridData.grid;
            float margin = circleMargin / 2 * squareLength;
            //float shadowMargin = (circleMargin - 0.07f) / 2 * squareLength;
            float reverseMargin = squareLength * (1 - circleMargin);
            //float reverseShadowMargin = squareLength * (1 - (circleMargin - 0.07f));
            for (int i = 0; i < startPoints.GetLength(0); i++)
            {
                for (int j = 0; j < startPoints.GetLength(1); j++)
                {
                    if (startPoints[i, j] != -1)
                    {
                        //g.FillEllipse(new SolidBrush(ControlPaint.Dark(colorPallet[startPoints[i, j]])), i * squareLength + shadowMargin, j * squareLength + shadowMargin, reverseShadowMargin, reverseShadowMargin);
                        g.FillEllipse(new SolidBrush(colorPallet[startPoints[i, j]]), i * squareLength + margin, j * squareLength + margin, reverseMargin, reverseMargin);
                    }
                }
            }
            if (currentPath != null)
            {
                DrawPathTo(g, squareLength, currentPath, pathSize);
            }
            foreach(Path p in currentLevel.pathsOfColors)
            {
                if (p != null)
                {
                    if (currentPath != null)
                    {
                        if (currentPath.color != p.color) DrawPathTo(g, squareLength, p, pathSize);
                    }
                    else DrawPathTo(g, squareLength, p, pathSize);
                }

            }
            drawTo.DrawImage(image, new Point(center.X - image.Width / 2, center.Y - image.Height / 2));
            imgGrid = image;
        }

        public static void DrawPathTo(Graphics g, int squareLength, Path path, float pathSize)
        {
            int x = path.firstPoint.X;
            int y = path.firstPoint.Y;
            Pen draw = new Pen(colorPallet[path.color], squareLength * pathSize);
            draw.EndCap = LineCap.Round;
            draw.StartCap = LineCap.Round;

            foreach (Path.Direction d in path.path)
            {
                int newx = x;
                int newy = y;
                if ((int)d % 2 == 0)
                    newy += (1 - (int)d) * -1;
                else
                    newx += 2 - (int)d;
                Point p1 = new Point(squareLength * x + squareLength / 2, squareLength * y + squareLength / 2);
                Point p2 = new Point(squareLength * newx + squareLength / 2, squareLength * newy + squareLength / 2);
                g.DrawLine(draw, p1, p2);
                x = newx;
                y = newy;
            }
        }
    }
}
