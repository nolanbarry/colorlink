using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace flow
{
    public class Grid
    {
        public int[,] grid { get; private set; }
        public int[] colorsPresent { get; private set; }
        public Path[] pathsOfColors { get; private set;  } // should line up with colors present array - the 0th color holds the 0th path, etc.
        public bool solved { get; private set; }
        public int gridWidth
        {
            get
            {
                return grid.GetLength(0);
            }
        }
        public int gridHeight
        {
            get
            {
                return grid.GetLength(1);
            }
        }

        /// <param name="grid">A rectangular array where -1 represents empty space and a positive number represents a color.</param>
        public Grid(int[,] grid)
        {
            if (!VerifyValidity(grid)) throw new Exception("The passed grid is not valid.");
            this.grid = grid;

            List<int> colorsPresentAssembly = new List<int>();
            foreach(int i in grid)
            {
                if (i >= 0)
                {
                    if (!colorsPresentAssembly.Contains(i))
                    {
                        colorsPresentAssembly.Add(i);
                    }
                }
            }
            colorsPresent = colorsPresentAssembly.ToArray();
            pathsOfColors = new Path[colorsPresent.Length];
            solved = false;
        }

        public Grid(SolvingGrid g)
        {
            grid = g.grid;
            colorsPresent = g.colors;
            pathsOfColors = g.pathsOfColors.ToArray();
            RotateFlipPaths(pathsOfColors); // for some weird reason SolvingGrid flips x and y axis (and some other funky stuff, either way this fixes it)
            solved = false;
        }

        private void RotateFlipPaths(Path[] patharry)
        {
            foreach(Path p in patharry)
            {
                for(int i = 0; i < p.path.Count; i++)
                {
                    p.path[i] = (Path.Direction)(((int)p.path[i] + 1) % 4);
                    if ((int) p.path[i] % 2 == 1) p.path[i] = (Path.Direction)(((int)p.path[i] + 2) % 4);
                }
                p.firstPoint = new Point(p.firstPoint.Y, p.firstPoint.X);
            }
        }

        public void EditPathOfColor(Path newPath)
        {
            int index = Array.IndexOf(colorsPresent, newPath.color);
            pathsOfColors[index] = newPath;
            CheckIfSolved();
        }

        /// <summary>
        /// Sets the solved bool to true or false if all paths exist and end on an appropriate node.
        /// </summary>
        public void CheckIfSolved()
        {
            if (pathsOfColors.Contains(null)) { solved = false; return; }
            List<int> oneDimensionalGrid = new List<int>();
            foreach (int i in FlattenGrid(int.MaxValue)) oneDimensionalGrid.Add(i);
            if (oneDimensionalGrid.Contains(-1)) { solved = false; return; }
            solved = true;
            foreach (Path p in pathsOfColors)
            {
                if (grid[p.lastPoint.X, p.lastPoint.Y] != p.color) solved = false;
            }
        }

        public int[,] FlattenGrid(int colorToNotInclude)
        {
            int[,] flat = new int[grid.GetLength(0), grid.GetLength(1)];
            for (int i = 0; i < flat.GetLength(0); i++) for (int j = 0; j < flat.GetLength(1); j++) flat[i, j] = -1;
            int num = 0;
            foreach(Path p in pathsOfColors)
            {
                if (p != null && colorsPresent[num] != colorToNotInclude)
                {
                    foreach(Point point in p.asCoordinateArray)
                    {
                        flat[point.X, point.Y] = colorsPresent[num];
                    }
                }
                num++;
            }
            return flat;
        }

        /// <summary>
        /// Checks to make sure every dot on the grid has one matching counterpart and that there are no non-existent colors.
        /// </summary>
        private static bool VerifyValidity(int[,] grid)
        {
            List<int> tally = new List<int>();
            foreach(int i in grid)
            {
                if (i >= 0)
                {
                    if (i > flowindow.colorPallet.Length - 1)
                        return false;
                    while (tally.Count - 1 < i)
                    {
                        tally.Add(0);
                    }
                    tally[i]++;
                }
            }
            foreach(int j in tally)
            {
                if (j != 0 && j != 2) return false;
            }
            return true;
        }
    }
}
