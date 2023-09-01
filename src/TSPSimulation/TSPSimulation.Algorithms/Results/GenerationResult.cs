using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSPSimulation.ProblemDefinition.Static;

namespace TSPSimulation.Algorithms.Results
{
    public class GenerationResult
    {
        public int Generation { get; }
        public int Evaluations { get; }
        public TSPSolution[] Population { get; }
        public TSP Problem { get; }

        public GenerationResult(int generation, int evaluations, TSPSolution[] population, TSP problem)
        {
            Generation = generation;
            Evaluations = evaluations;
            Population = population;
            Problem = problem;
        }
    }
}
