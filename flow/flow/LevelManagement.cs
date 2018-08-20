using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Runtime.Serialization.Formatters.Binary;

namespace flow
{
    public static class LevelManagement
    {
        public static Grid ParseFileIntoGrid(int targetLevel, string path)
        {
            string[] file = File.ReadAllLines("Assets\\Levels\\" + path);
            List<string> listFile = file.ToList();
            listFile.Add("");
            while (listFile[0].Trim() == "") listFile.RemoveAt(0);
            file = listFile.ToArray();
            int currentLevel = 0;
            int line = 0;
            while (currentLevel < targetLevel)
            {
                if (file[line].Trim() == "" && file[line+1].Trim() != "") currentLevel++;
                line++;
            }
            List<string> levelStr = new List<string>();
            do
            {
                levelStr.Add(file[line]);
                line++;
            } while (file[line].Trim() != "");
            int[,] levelAsIntArr = new int[levelStr[0].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length, levelStr.Count];
            for(int i = 0; i < levelAsIntArr.GetLength(1); i++)
            {
                for(int j = 0; j < levelAsIntArr.GetLength(0); j++)
                {
                    try
                    {
                        levelAsIntArr[i, j] = int.Parse(levelStr[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[j]);
                    } catch
                    {
                        levelAsIntArr[i, j] = -1;
                    }
                }
            }
            Grid g = new Grid(levelAsIntArr);
            return g;
        }

        public static Grid lastSolution;
        public static bool IsItSolvable(int[,] grid)
        {
            SolvingGrid info = new SolvingGrid(grid);

            return Solve(info);
        }

        private static bool Solve(SolvingGrid info)
        {
            Path.Direction[] potentials = GetMoveOptions(info);
            bool solved = false;
            foreach(Path.Direction d in potentials)
            {
                SolvingGrid cloned = (SolvingGrid)DeepClone(info);
                cloned.AddDirectionToCurrentPath(d);
                if (cloned.state == SolvingGrid.SolveState.Success)
                {
                    lastSolution = new Grid(cloned);
                    return true;
                }
                else if (cloned.state == SolvingGrid.SolveState.Solving)
                    if (Solve(cloned)) return true;
            }
            
            return solved;
        }

        // TODO - MAKE IT SO PATH CANNOT TRACE OVER ITSELF, BUT CAN STILL GO TO ENDPOINT
        private static Path.Direction[] GetMoveOptions(SolvingGrid info)
        {
            int Y, X;
            Path currentPath = info.pathsOfColors.Last();
            Y = currentPath.lastPoint.Y;
            X = currentPath.lastPoint.X;
            List<Path.Direction> potentials = new List<Path.Direction>();
            int[,] grid = info.FlattenGrid();
            try { if (grid[Y - 1, X] == -1  // direction is valid if new square is not visited or new square is the endpoint for the color of the current path
                    || new Point(X, Y - 1) == info.endNodes[Array.IndexOf(info.colors, info.pathsOfColors.Last().color)])
                        potentials.Add(Path.Direction.Up); } catch { } // try catch is for catching array out of bounds errors, where nothing should be done.
            try { if (grid[Y + 1, X] == -1
                     || new Point(X, Y + 1) == info.endNodes[Array.IndexOf(info.colors, info.pathsOfColors.Last().color)])
                        potentials.Add(Path.Direction.Down); } catch { }
            try { if (grid[Y, X - 1] == -1
                     || new Point(X - 1, Y) == info.endNodes[Array.IndexOf(info.colors, info.pathsOfColors.Last().color)])
                        potentials.Add(Path.Direction.Left); } catch { }
            try { if (grid[Y, X + 1] == -1
                     || new Point(X + 1, Y) == info.endNodes[Array.IndexOf(info.colors, info.pathsOfColors.Last().color)])
                        potentials.Add(Path.Direction.Right); } catch { }
            return potentials.ToArray();
        }

        public static object DeepClone(object obj)
        {
            object objResult = null;
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, obj);

                ms.Position = 0;
                objResult = bf.Deserialize(ms);
            }
            return objResult;
        }

    }

    [Serializable()]
    public class SolvingGrid
    {
        public enum SolveState { Solving, Failed, Success }
        public int[,] grid { get; private set; }
        public List<Point> startNodes { get; private set; }
        public List<Point> endNodes { get; private set; }
        public int[] colors { get; private set; }
        public List<Path> pathsOfColors;
        public SolveState state;

        public SolvingGrid(int[,] grid)
        {
            this.grid = grid;
            startNodes = new List<Point>();
            endNodes = new List<Point>();
            List<int> colorsList = new List<int>();
            for (int y = 0; y < grid.GetLength(0); y++)
            {
                for (int x = 0; x < grid.GetLength(1); x++)
                {
                    if (grid[y, x] != -1 && !colorsList.Contains(grid[y, x]))
                    {
                        startNodes.Add(new Point(x, y));
                        colorsList.Add(grid[y, x]);
                    } else if (colorsList.Contains(grid[y, x]))
                    {
                        endNodes.Add(new Point(x, y)); // reversed so that calling point.X actually gives you the X value, not the Y value. BE CAREFUL OF THIS WHEN CALLING LATER
                        // REMEMBER: int[,] calls for int[y,x]
                    }
                }
            }
            state = SolveState.Solving;
            colors = colorsList.ToArray();
            pathsOfColors = new List<Path>();
            pathsOfColors.Add(new Path(startNodes[0], colors[0]));

            // sort endNodes to correspond correctly with startNodes
            List<Point> endNodesSorted = new List<Point>();
            for(int i = 0; i < startNodes.Count; i++)
            {
                for(int j = 0; j < endNodes.Count; j++)
                {
                    if (grid[endNodes[j].Y, endNodes[j].X] == colors[i])
                    {
                        endNodesSorted.Add(endNodes[j]);
                        j = endNodes.Count;                     
                    }
                }
            }
            endNodes = endNodesSorted;
        }

        public SolvingGrid(SolvingGrid old)
        {
            grid = (int[,]) old.grid.Clone();

            startNodes = (List<Point>)LevelManagement.DeepClone(old.startNodes);
            endNodes = (List<Point>)LevelManagement.DeepClone(old.endNodes);

            colors = (int[]) old.colors.Clone();

            pathsOfColors = (List<Path>)LevelManagement.DeepClone(old.pathsOfColors);

            state = SolveState.Solving;
        }

        public int[,] FlattenGrid()
        {
            int[,] flat = new int[grid.GetLength(0), grid.GetLength(1)];
            for (int i = 0; i < flat.GetLength(0); i++) for (int j = 0; j < flat.GetLength(1); j++) flat[i, j] = -1;
            int num = 0;
            foreach (Point p in startNodes)
            {
                flat[p.Y, p.X] = colors[num];
                num++;
            }
            num = 0;
            foreach (Point p in endNodes)
            {
                flat[p.Y, p.X] = colors[num];
                num++;
            }
            num = 0;
            foreach (Path p in pathsOfColors)
            {
                if (p != null)
                {
                    foreach (Point point in p.asCoordinateArray)
                    {
                        flat[point.Y, point.X] = colors[num];
                    }
                }
                num++;
            }
            return flat;
        }

        public int[,] FlattenGridWithoutCurrentPath()
        {
            int[,] flat = new int[grid.GetLength(0), grid.GetLength(1)];
            for (int i = 0; i < flat.GetLength(0); i++) for (int j = 0; j < flat.GetLength(1); j++) flat[i, j] = -1;
            int num = 0;
            foreach (Path p in pathsOfColors)
            {
                if (p != null && p != pathsOfColors.Last())
                {
                    foreach (Point point in p.asCoordinateArray)
                    {
                        flat[point.X, point.Y] = colors[num];
                    }
                }
                num++;
            }
            return flat;
        }

        public void AddDirectionToCurrentPath(Path.Direction d)
        {
            pathsOfColors.Last().Add(d);
            if (grid[pathsOfColors.Last().lastPoint.Y, pathsOfColors.Last().lastPoint.X] == pathsOfColors.Last().color)
            {
                startNodes.RemoveAt(0);
                try
                {
                    pathsOfColors.Add(new Path(startNodes[0], colors[1 + Array.IndexOf(colors, pathsOfColors.Last().color)]));
                } catch
                {
                    bool successful = true;
                    foreach(int i in FlattenGrid())
                    {
                        if (i == -1) successful = false;
                    }
                    if (successful) state = SolveState.Success;
                    else state = SolveState.Failed;
                }
            }
        }
    }

}
