﻿using System;
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
        /// <summary>
        /// Returns the nth level in the given file as a Grid
        /// </summary>
        /// <param name="targetLevel">Which level to find in the file.</param>
        /// <param name="path">The file path.</param>
        /// <returns></returns>
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

        /// <summary>
        /// The solution to the last solvable grid that was queried
        /// </summary>
        public static Grid lastSolution;

        /// <summary>
        /// Returns true or false if a grid is solvable. If solvable, stores the solution in lastSolution.
        /// </summary>
        /// <param name="grid">A two dimensional int array representing the level.</param>
        /// <returns></returns>
        public static bool IsItSolvable(int[,] grid)
        {
            SolvingGrid info = new SolvingGrid(grid);

            return Solve(info);
        }

        /// <summary>
        /// Recursive algorithm to solve a level.
        /// </summary>
        /// <param name="info">All the appropriate info stored in a SolvingGrid.</param>
        /// <returns>Returns whether or not the level is solvable.</returns>
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

        /// <summary>
        /// Gets the potential directions of the current active cell, or the last cell on the last path in the given SolvingGrid. 
        /// </summary>
        /// <param name="info">All the appropriate info in the form of a SolvingGrid.</param>
        /// <returns>Direction array with all options.</returns>
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

        /// <summary>
        /// Deep clones an object. Object must be serializable.
        /// </summary>
        /// <returns>Deep cloned object</returns>
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

    /// <summary>
    /// A class similar to flow.Grid, the main differences being its increased editablity and extra information to make solving easier.
    /// </summary>
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
            // find the start nodes, colors, and end nodes
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
                        endNodes.Add(new Point(x, y)); 
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

        /// <summary>
        /// For use with figuring out whether or not a cell is available to move to.
        /// </summary>
        /// <returns>Returns the grid with the paths added to it as just colors, with no actual information on direction</returns>
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

        /// <summary>
        /// Adds the given direction to the currently active path, and starts a new path if that finishes that pipe.
        /// </summary>
        /// <param name="d">The direction to add</param>
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
