using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace flow
{
    [Serializable()]
    public class Path
    {
        public enum Direction { Up, Right, Down, Left };

        public List<Direction> path { get; private set; }

        public Point firstPoint;
        public Point lastPoint
        {
            get
            {
                return asCoordinateArray.Last();
            }
        }
        public Point[] asCoordinateArray
        {
            get
            {
                List<Point> coords = new List<Point>();
                coords.Add(firstPoint);
                foreach (Path.Direction d in path)
                {
                    int x = coords.Last().X;
                    int y = coords.Last().Y;
                    if ((int)d % 2 == 0)
                        y += (1 - (int)d) * -1; // up, down
                    else
                        x += 2 - (int)d; // right, left
                    coords.Add(new Point(x, y));
                }
                return coords.ToArray();
            }
        }
        public int color; // color of path, corresponds to flowindow.colorPallet
        public Path() : this(new Point(0, 0), 0) { }

        public Path(Point start, int color)
        {
            firstPoint = start;
            this.color = color;
            path = new List<Direction>();
        }

        public void Add(Direction d)
        {
            if (path.Count > 0)
            {
                if ((int)path.Last() % 2 == (int)d % 2 && (int)path.Last() != (int)d)
                    path.RemoveAt(path.Count - 1);
                else path.Add(d);
            }
            else path.Add(d);
        }
    }
}
