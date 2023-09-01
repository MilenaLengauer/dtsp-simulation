using Newtonsoft.Json;
using System.Data;
using TSPSimulation.Algorithms;
using TSPSimulation.Algorithms.Configuration;
using TSPSimulation.ProblemDefinition.Static;
using TSPSimulation.Reporting;

namespace TSPSimulation.ProblemDefinition
{
    public class DynamicTSPSolution
    {
        public string Name { get; private set; }

        [JsonIgnore]
        public DynamicTSP DynamicTSP { get; private set; }

        public IList<ReachedCity> FinalPath { get; private set; }

        public AlgorithmReport AlgorithmReport { get; private set; }

        public DynamicTSPSolution(string name, DynamicTSP dynamicTSP, AlgorithmConfiguration algorithmConfiguration)
        {
            Name = name;
            DynamicTSP = dynamicTSP;
            AlgorithmReport = new AlgorithmReport(name, algorithmConfiguration);
            FinalPath = new List<ReachedCity>();
            FinalPath.Add(new ReachedCity(0, 0, 0, 1)); // add the start (depot)
        }

        public void AddToFinalPath(int city, double distance, int generation)
        {
            double distanceLastCity = FinalPath[^1].Distance;
            long timeLastCity = FinalPath[^1].Time;
            FinalPath.Add(
                new ReachedCity(city, 
                    (long)(timeLastCity + distance * DynamicTSP.Configuration.TimeForDistanceUnit), 
                    distanceLastCity + distance, 
                    generation
                ));
        }

        public IList<int> GetReachedCities(long time)
        {
            return FinalPath.Where(p => p.Time <= time).Select(p => p.City).ToList();
        }

        public double GetCurrentDistance(long time)
        {
            return FinalPath.Where(p => p.Time <= time).Max(p => p.Distance);
        }

        public double GetCurrentDistance(int generation)
        {
            if (generation == 0) return 0;
            return FinalPath.Where(p => p.Generation <= generation).Max(p => p.Distance);
        }

        public long GetFinalTime()
        {
            return FinalPath[^1].Time;
        }

        public double GetFinalDistance()
        {
            return FinalPath[^1].Distance;
        }

        public void RepairAfterImport(DynamicTSP dynamicTSP)
        {
            DynamicTSP = dynamicTSP;
            foreach(var period in AlgorithmReport.Periods)
            {
                period.AlgorithmReport = AlgorithmReport;
                var solution = period.Problem.OptimalSolution;
                if(solution != null)
                {
                    solution.Problem = period.Problem;
                }
                foreach(var generation in period.Generations)
                {
                    generation.Period = period;
                }
            }
        }
    }
}
