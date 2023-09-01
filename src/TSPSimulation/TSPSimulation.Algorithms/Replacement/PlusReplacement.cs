using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSPSimulation.ProblemDefinition.Static;

namespace TSPSimulation.Algorithms.Replacement
{
    public class PlusReplacement : IReplacement
    {
        public TSPSolution[] Replace(TSPSolution[] parents, TSPSolution[] children)
        {
            return parents.Concat(children).OrderBy(s => s.Fitness).Take(parents.Length).ToArray();
        }
    }
}
