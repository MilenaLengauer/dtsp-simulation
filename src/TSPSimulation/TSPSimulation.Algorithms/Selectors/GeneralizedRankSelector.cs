using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSPSimulation.Commons;
using TSPSimulation.ProblemDefinition.Static;

namespace TSPSimulation.Algorithms.Selectors
{
    public class GeneralizedRankSelector : ISelector
    {
        private readonly int pressure;

        public GeneralizedRankSelector(int pressure)
        {
            this.pressure = pressure;
        }

        public int Select(ThreadSafeRandom random, TSPSolution[] population)
        {
            double rand = 1 + random.NextDouble() * (Math.Pow(population.Length, 1.0 / pressure) - 1);
            int index = (int)Math.Floor(Math.Pow(rand, pressure) - 1);
            return index;
        }
    }
}
