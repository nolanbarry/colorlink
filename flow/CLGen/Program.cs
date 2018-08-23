using System;
using System.IO;
using Colorlink;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLGen
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            int num = int.Parse(args[0]);
            int width = int.Parse(args[1]);
            int height = int.Parse(args[2]);
            int maxColor = int.Parse(args[3]);
            Console.WriteLine($"Generating {num} {width}x{height} colorlink level(s), with {(maxColor + 1)} possible colors");
            int startCursor = Console.CursorTop;
            string filename = $"{width}x{height}.txt";
            Int64 i = 0;
            while (File.Exists("Generated Colorlink Levels\\" + filename))
            {
                filename = $"{width}x{height}_{i}.txt";
                i++;
            }
            string path = Directory.GetCurrentDirectory() + $"\\Generated Colorlink Levels";
            Directory.CreateDirectory(path);
            string filepath = path + $"\\{filename}";
            Console.WriteLine("I'll output here:");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(filepath);
            Console.ForegroundColor = ConsoleColor.White;

            List<Puzzle> levels = new List<Puzzle>();
            Console.Write("Progess: ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("0");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(" of " + num);
            for (int j = 0; j < num; j++)
            {
                levels.Add(PuzzleGenerator.GenerateSolvableLevel(width, height, maxColor, false));
                Console.CursorTop = 2 + startCursor;
                Console.CursorLeft = 9;
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write(j + 1);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($" of {num}");
            }
            Console.WriteLine();
            List<string> outputList = new List<string>();
            foreach (Puzzle g in levels)
            {
                string[] arr = PuzzleGenerator.GridToString(g.level.grid);
                foreach (string s in arr)
                {
                    outputList.Add(s);
                }
                outputList.Add("");
            }
            string[] outputArr = outputList.ToArray();
            string output = "";
            for (int j = 0; j < outputArr.Length; j++)
            {
                output += outputArr[j] + "\n";
            }
            File.AppendAllText(filepath, output);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("All done.");
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}
