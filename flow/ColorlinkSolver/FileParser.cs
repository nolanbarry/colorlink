using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;

namespace Colorlink
{
    public class FileParser
    {
        /// <summary>
        /// All FilerParser objects will automatically put this string infront of the passed filepath parameter.
        /// </summary>
        public static string fileFolder = "C:\\";
        public Grid[] levels;
        public int numberOfLevels { get { return levels.Length; } }
        public string currentFile;
        public FileParser(string filepath)
        {
            if (File.Exists(fileFolder + filepath))
                OpenFile(fileFolder + filepath);
            else throw new Exception($"File \"{filepath}\" does not existin directory {fileFolder}.");
        }
        
        /// <summary>
        /// Opens a file, parses it completely, and makes it available for use with levels. 
        /// Note that this overwrites any previously loaded files. If file does not exist, loads whatever was previously loaded again.
        /// </summary>
        public void OpenFile(string filepath)
        {
            levels = RetrieveFile(filepath);
            if (levels == null) OpenFile(currentFile);
            else currentFile = filepath;
        }

        /// <summary>
        /// Returns the given file as a parsed Grid[]. 
        /// </summary>
        /// <returns>Parsed Grid[] contained in file. Returns null if file does not exist.</returns>
        public Grid[] RetrieveFile(string filepath)
        {
            if (File.Exists(filepath))
            {
                List<Grid> loadingLevels = new List<Grid>();
                currentFile = filepath;
                List<string> lines = File.ReadAllLines(filepath).ToList();
                while (lines[0].Trim() == "") { lines.RemoveAt(0); } // remove blank spaces from top of file
                lines.Add("");
                int currentline = 0;
                do // assemble level
                {
                    List<string> currentGrid = new List<string>();
                    do
                    {
                        currentGrid.Add(lines[currentline]);
                        currentline++;
                    } while (lines[currentline].Trim() != "");
                    if (currentGrid.Count > 1)
                    {
                        while (currentGrid[0].Trim() == "") { currentGrid.RemoveAt(0); } // remove blank lines from top of grid
                        int width = currentGrid[0].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length;
                        int[,] parsedGrid = new int[currentGrid.Count, width]; // parse level into int[,];
                        for (int i = 0; i < currentGrid.Count; i++)
                        {
                            string[] splitLine = currentGrid[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            for (int j = 0; j < splitLine.Length; j++)
                            {
                                if (int.TryParse(splitLine[j], out int parsedInt))
                                    parsedGrid[i, j] = parsedInt;
                                else
                                    parsedGrid[i, j] = -1;

                            }
                        }
                        loadingLevels.Add(new Grid(parsedGrid));
                    }
                } while (currentline < lines.Count - 2);
                return loadingLevels.ToArray();
            }
            else return null;
        }

        /// <summary>
        /// Retrieves the level and its solution from the currently open file.
        /// </summary>
        /// <returns>Returns the level's grid and solution as a Puzzle object, or null if level does not exist.</returns>
        public Puzzle GetLevel(int levelNumber)
        {
            if (numberOfLevels > levelNumber)
            {
                Grid grid = levels[levelNumber];
                Grid solution = PuzzleSolver.GetSolution(grid.grid);
                return new Puzzle(grid, solution);
            }
            else return null;
        }

        /*
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
                        levelAsIntArr[j, i] = int.Parse(levelStr[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[j]);
                    } catch
                    {
                        levelAsIntArr[j, i] = -1;
                    }
                }

            }
            Grid g = new Grid(levelAsIntArr);
            return g;
        }
        */
    }
}
