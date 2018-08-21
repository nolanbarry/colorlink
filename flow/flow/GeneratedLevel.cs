using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flow
{
    /// <summary>
    /// An all in one object for passing a blank level and solved level from BackgroundWorker to the UI
    /// </summary>
    public class GeneratedLevel
    {
        public Grid blankLevel { get; private set; }
        public Grid solution { get; private set; }

        public GeneratedLevel(Grid level, Grid solution)
        {
            blankLevel = level;
            this.solution = solution;
        }
    }
}
