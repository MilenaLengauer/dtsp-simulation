using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSPSimulation.Commons;
using TSPSimulation.ProblemDefinition.Static;

namespace TSPSimulation.Algorithms.Selectors
{
    public class RandomSelector : ISelector
    {
        public int Select(ThreadSafeRandom random, TSPSolution[] population)
        {
            return random.Next(population.Length);
        }
    }
}
