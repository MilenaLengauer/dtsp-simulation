using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSPSimulation.ProblemDefinition.Static;

namespace TSPSimulation.Algorithms.Replacement
{
    public interface IReplacement
    {
        public TSPSolution[] Replace(TSPSolution[] parents, TSPSolution[] children);

    }
}
