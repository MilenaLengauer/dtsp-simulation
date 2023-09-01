using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSPSimulation.ProblemDefinition.Static;

namespace TSPSimulation.Algorithms.Results
{
    public class NgaGenerationResult : GenerationResult
    {
        public TSPSolution[][] Neighborhoods { get; }

        public NgaGenerationResult(int generation, int evaluations, TSPSolution[] population, TSP problem, TSPSolution[][] neighborhoods)
            : base(generation, evaluations, population, problem)
        {
            Neighborhoods = neighborhoods;
        }
    }
}
