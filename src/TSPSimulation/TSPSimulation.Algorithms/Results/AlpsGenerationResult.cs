using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSPSimulation.ProblemDefinition.Static;

namespace TSPSimulation.Algorithms.Results
{
    public class AlpsGenerationResult : GenerationResult
    {
        public List<List<AlpsSolution>> AgeLayers { get; }

        public AlpsGenerationResult(int generation, int evaluations, TSPSolution[] population, TSP problem, List<List<AlpsSolution>> ageLayers)
            : base(generation, evaluations, population, problem)
        {
            AgeLayers = ageLayers;
        }
    }
}
