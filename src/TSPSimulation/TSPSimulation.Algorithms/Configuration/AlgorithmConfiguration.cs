using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using TSPSimulation.Algorithms.Crossovers;
using TSPSimulation.Algorithms.Mutators;
using TSPSimulation.Algorithms.Replacement;
using TSPSimulation.Algorithms.Selectors;
using TSPSimulation.ProblemDefinition.Static;

namespace TSPSimulation.Algorithms.Configuration
{
    public abstract class AlgorithmConfiguration
    {
        public string? Name { get; set; }
        public int Group { get; set; }

        public bool PerformDiversityCalculations { get; set; } = true;
        public bool PerformEdgeCalculations { get; set; } = true;

        public int? RandomState { get; set; }

        public int PopulationSize { get; set; } = 100;
        public double MutationRate { get; set; } = 0.05;
        public MutationOperators Mutator { get; set; } = MutationOperators.InversionMutation;
        public CrossoverOperators Crossover { get; set; } = CrossoverOperators.OrderCrossover;
        public SelectionOperators Selector { get; set; } = SelectionOperators.TournamentSelector;
        public ReplacementStrategies Replacement { get; set; } = ReplacementStrategies.GenerationalReplacement;
        public int TournamentSize { get; set; } = 2;
        public int Pressure { get; set; } = 4;
        public int Elites { get; set; } = 0;

        public IMutator GetMutator()
        {
            switch(Mutator)
            {
                case MutationOperators.InversionMutation: return new InversionMutator();
                case MutationOperators.ExchangeMutation: return new ExchangeMutator();
                case MutationOperators.TranslocationMutation: return new TranslocationMutator();
                default: throw new AlgorithmConfigurationException($"mutation operator {Mutator} does not exist");
            }
            
        }

        public ICrossover GetCrossover()
        {
            switch (Crossover)
            {
                case CrossoverOperators.OrderCrossover: return new OrderCrossover();
                case CrossoverOperators.PartiallyMappedCrossover: return new PartiallyMappedCrossover();
                case CrossoverOperators.EdgeRecombinationCrossover: return new EdgeRecombinationCrossover();
                default: throw new AlgorithmConfigurationException($"crossover operator {Crossover} does not exist");
            }

        }

        public ISelector GetSelector()
        {
            switch(Selector)
            {
                case SelectionOperators.RandomSelector: return new RandomSelector();
                case SelectionOperators.TournamentSelector: return new TournamentSelector(TournamentSize);
                case SelectionOperators.RankSelector: return new RankSelector();
                case SelectionOperators.GeneralizedRankSelector: return new GeneralizedRankSelector(Pressure);
                case SelectionOperators.ProportionalSelector: return new ProportionalSelector();
                default: throw new AlgorithmConfigurationException($"selection operator {Selector} does not exist");
            }
            
        }

        public IReplacement GetReplacement()
        {
            switch (Replacement)
            {
                case ReplacementStrategies.GenerationalReplacement: return new GenerationalReplacement(Elites);
                case ReplacementStrategies.ParentCompetingReplacement: return new ParentCompetingReplacement();
                case ReplacementStrategies.PlusReplacement: return new PlusReplacement();
                default: throw new AlgorithmConfigurationException($"replacement strategy {Replacement} does not exist");
            }

        }

        public abstract string GetAlgorithmName();

        public abstract IAlgorithm CreateAlgorithm(TSP tsp);
    }



}
