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
