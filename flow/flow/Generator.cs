using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flow
{
    public class Generator
    {
        public static int[,] GenerateLevel(int width, int height)
        {
            Random gen = new Random();
            int minus = 0; // gen.Next(3);
            int colors = (int)(Math.Sqrt(width * height) - minus);
            int[] colorsToUse = new int[colors];
            for(int i = 0; i < colors; i++)
            {
                int chosenColor;
                do
                {
                    chosenColor = gen.Next(flowindow.colorPallet.Length);
                } while (colorsToUse.Contains(chosenColor));
                colorsToUse[i] = chosenColor;
            }

            int[,] generated = new int[height, width]; // this is backward (not width, height) because it's really asksing for rows, columns
            for (int i = 0; i < height; i++) 
            {
                for(int j = 0; j < width; j++)
                {
                    generated[i, j] = -1;
                }
            }
            for(int i = 0; i < colors; i++)
            {
                for(int j = 0; j < 2; j++)
                {
                    int col, row;
                    do
                    {
                        col = gen.Next(width);
                        row = gen.Next(height);
                    } while (generated[row, col] != -1);
                    generated[row, col] = colorsToUse[i];
                }
            }
            return generated;
        }
    }
}
