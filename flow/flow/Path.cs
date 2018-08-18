using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace flow
{
    public class Path
    { 
        public enum Direction { Up, Right, Down, Left };

        public Point beginPoint;
        public List<Direction> path { get; private set; }
        public Path() : this(new Point(0, 0)) { }

        public Path(Point start)
        {
            beginPoint = start;
            path = new List<Direction>();
        }

        public void Add(Direction d)
        {
            if ((int)path.Last() % 2 == (int)d % 2) path.RemoveAt(path.Count);
            else path.Add(d);
        }
    }
}
