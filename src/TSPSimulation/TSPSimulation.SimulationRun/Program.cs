using TSPSimulation.Algorithms;
using TSPSimulation.Algorithms.Configuration;
using TSPSimulation.Commons;
using TSPSimulation.ProblemDefinition;

namespace TSPSimulation.SimulationRun
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = new DynamicTSPConfiguration
            (
                "berlin52",
                7542,
                20000
            );
            var random = new ThreadSafeRandom();
            var tspCreator = new DynamicTSPCreator(configuration, random);
            var dynamicTSP = tspCreator.CreateDynamicTSP();
            var simulation = new DynamicTSPRun(dynamicTSP, "Test", new GeneticAlgorithmConfiguration());
            var solution = simulation.ExecuteSimulation();

            Console.WriteLine("\n\nFinal Path:");
            foreach((int city, long time, double distance, int generation) in solution.FinalPath)
            {
                Console.Write(city + "\t");
            }
        }
    }
}


