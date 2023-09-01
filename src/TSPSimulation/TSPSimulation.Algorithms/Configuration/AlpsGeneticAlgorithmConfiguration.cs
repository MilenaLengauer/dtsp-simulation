using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSPSimulation.Commons;
using TSPSimulation.ProblemDefinition.Static;

namespace TSPSimulation.Algorithms.Configuration
{
    public class AlpsGeneticAlgorithmConfiguration : AlgorithmConfiguration
    {
        public int NumberOfLayers { get; set; } = 10;
        public int AgeGap { get; set; } = 20;
        public AgingScheme AgingScheme { get; set; } = AgingScheme.Polynomial;
        public bool PlusSelection { get; set; } = false;

        public override IAlgorithm CreateAlgorithm(TSP tsp)
        {
            return new AlpsGeneticAlgorithm(tsp, this, RandomState.HasValue ? new ThreadSafeRandom(RandomState.Value) : new ThreadSafeRandom());
        }

        public override string GetAlgorithmName()
        {
            return "ALPS Genetic Algorithm";
        }

        public IList<int> LayerStartAges()
        {
            IList<int> layerStartAges = new List<int>();
            switch(AgingScheme)
            {
                case AgingScheme.Linear:
                    for (int i = 0; i < NumberOfLayers; i++)
                    {
                        layerStartAges.Add(i * AgeGap);
                    }
                    break;
                case AgingScheme.Fibonacci:
                    // 1 2 3 5 8 13 21 ...
                    int x1 = 0;
                    int x2 = 1;
                    int next;
                    for(int i = 0; i < NumberOfLayers; i++)
                    {
                        next = x1 + x2;
                        layerStartAges.Add(next * AgeGap);
                        x1 = x2;
                        x2 = next;
                    }
                    break;
                case AgingScheme.Polynomial:
                    layerStartAges.Add(0);
                    layerStartAges.Add(1 * AgeGap);
                    layerStartAges.Add(2 * AgeGap);
                    for (int i = 2; i < NumberOfLayers - 1; i++)
                    {
                        layerStartAges.Add(i * i * AgeGap);
                    }
                    break;
                case AgingScheme.Exponential:
                    layerStartAges.Add(0);
                    for (int i = 1; i < NumberOfLayers; i++)
                    {
                        layerStartAges.Add((int)Math.Pow(2, i) * AgeGap);
                    }
                    break;
            }
            return layerStartAges;
        }
    }

    public enum AgingScheme
    {
        Linear,
        Fibonacci,
        Polynomial,
        Exponential
    }
}
