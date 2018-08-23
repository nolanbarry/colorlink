using System;
using System.Drawing;

namespace Colorlink
{
    public class PuzzleRetriever
    {
        public enum RetrievalMethod { Generate, FromFile };
        public enum RetrievalStatus { Generating, Ready, OutOfLevels };

        public RetrievalMethod mode { get; private set; }
        public RetrievalStatus status
        {
            get
            {
                if (mode == RetrievalMethod.FromFile)
                {
                    if (level < puzzleparser.numberOfLevels) return RetrievalStatus.Ready;
                    else return RetrievalStatus.OutOfLevels;
                }
                else if (puzzlegen.completedLevels.Count > 0) return RetrievalStatus.Ready;
                else return RetrievalStatus.Generating;
            }
        }

        private PuzzleGenerator puzzlegen;
        private static Size genSize;
        public int levelsToGenerateAtOnce = 10;
        private int maxColor;

        private FileParser puzzleparser;
        public int level;

        public PuzzleRetriever(string defaultFile) : this(defaultFile, 0) { }
        public PuzzleRetriever(int level) : this("4x4.txt", level) { }
        public PuzzleRetriever(string defaultFile, int level) : this(defaultFile, level, RetrievalMethod.FromFile) { }
        public PuzzleRetriever(string defaultFile, RetrievalMethod mode) : this(defaultFile, 0, mode) { }
        public PuzzleRetriever(string defaultFile, RetrievalMethod mode, int maxColor) : this(defaultFile, 0, mode, new Size(5, 5), maxColor) { }
        public PuzzleRetriever(string defaultFile, int level, RetrievalMethod mode) : this(defaultFile, level, mode, new Size(5, 5)) { }
        public PuzzleRetriever(string defaultFile, int level, RetrievalMethod mode, Size generationSize) : this(defaultFile, level, mode, generationSize, 10) { }

        public PuzzleRetriever(string defaultFile, int level, RetrievalMethod mode, Size generationSize, int maxColor)
        {
            this.mode = mode;
            this.level = level - 1; // the -1 is because .Next() immediately increments level
            puzzlegen = new PuzzleGenerator();
            puzzleparser = new FileParser(defaultFile);
            genSize = generationSize;
            this.maxColor = maxColor;
            puzzlegen.QueueLevels(levelsToGenerateAtOnce, genSize.Width, genSize.Height, maxColor);
        }

        public Puzzle Next()
        {
            if (mode == RetrievalMethod.Generate)
            {
                if (status == RetrievalStatus.Ready)
                    return puzzlegen.RetrieveAndRemoveLevel(true);
                else return null;
            } else
            {
                level++;
                if (status == RetrievalStatus.Ready)
                    return puzzleparser.GetLevel(level);
                else return null;
            }
        }

        public void SetMethod(RetrievalMethod m)
        {
            mode = m;
        }

        public void NewGenerationParameters(Size generationSize) { NewGenerationParameters(generationSize, maxColor); }
        public void NewGenerationParameters(Size generationSize, int maxColor)
        {
            puzzlegen.CancelAll();
            puzzlegen.completedLevels.Clear();
            genSize = generationSize;
            this.maxColor = maxColor;
            puzzlegen.QueueLevels(10, genSize.Width, genSize.Height, maxColor);
        }
        
    }
}

