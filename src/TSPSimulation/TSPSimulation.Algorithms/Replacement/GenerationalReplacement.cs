using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSPSimulation.ProblemDefinition.Static;

namespace TSPSimulation.Algorithms.Replacement
{
    public class GenerationalReplacement : IReplacement
    {
        private readonly int elites;

        public GenerationalReplacement(int elites)
        {
            this.elites = elites;
        }

        public TSPSolution[] Replace(TSPSolution[] parents, TSPSolution[] children)
        {
            for(int i = 0; i < elites && i < parents.Length; i++)
            {
                children[i] = parents[i];
            }
            return children;
        }
    }
}
