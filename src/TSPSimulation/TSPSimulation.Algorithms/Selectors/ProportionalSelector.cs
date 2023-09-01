using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using TSPSimulation.Commons;
using TSPSimulation.ProblemDefinition.Static;

namespace TSPSimulation.Algorithms.Selectors
{
    public class ProportionalSelector : ISelector
    {
        public int Select(ThreadSafeRandom random, TSPSolution[] population)
        {
            var maxQuality = population.Max(s => s.Fitness);
            var qualities = population.Select(s => maxQuality - s.Fitness).ToList();
            var qualitySum = qualities.Sum();

            double selectedQuality = random.NextDouble() * qualitySum;
            int index = 0;
            double currentQuality = qualities[index];
            while (currentQuality < selectedQuality)
            {
                index++;
                currentQuality += qualities[index];
            }
            Log.Logger.Information("Selected index: " + index);
            return index;
        }
    }
}
