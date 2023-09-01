using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSPSimulation.Commons;
using TSPSimulation.ProblemDefinition.Static;

namespace TSPSimulation.Algorithms.Configuration
{
    public class ImmigrantsGeneticAlgorithmConfiguration : AlgorithmConfiguration
    {
        public bool PerformAfterProblemUpdate { get; set; } = false;
        public int ImmigrantsInterval { get; set; } = 1;
        public int Immigrants { get; set; } = 10;
        public ImmigrantsType ImmigrantsType { get; set; } = ImmigrantsType.Random;

        public override IAlgorithm CreateAlgorithm(TSP tsp)
        {
            return new ImmigrantsGeneticAlgorithm(tsp, this, RandomState.HasValue ? new ThreadSafeRandom(RandomState.Value) : new ThreadSafeRandom());
        }

        public override string GetAlgorithmName()
        {
            return "Elitism-based Immigrants Genetic Algorithm";
        }


    }

    public enum ImmigrantsType
    {
        ElitismBased,
        Random
    }
}
