using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSPSimulation.Commons;
using TSPSimulation.ProblemDefinition.Static;

namespace TSPSimulation.Algorithms.Selectors
{
    public class RankSelector : ISelector
    {
        public int Select(ThreadSafeRandom random, TSPSolution[] population)
        {
            int lotSum = population.Length * (population.Length + 1) / 2;

            int selectedLot = random.Next(lotSum);
            int i = 0;
            int currentLot = 1;
            while(currentLot <= selectedLot)
            {
                i++;
                currentLot += (i + 1);
            }
            return i;            
        }
    }
}
