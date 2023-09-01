using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSPSimulation.Algorithms.Configuration;
using TSPSimulation.Algorithms.Results;
using TSPSimulation.ProblemDefinition.Static;

namespace TSPSimulation.Reporting
{
    public class AlgorithmReport
    {
        public string Name { get; private set; }

        public AlgorithmConfiguration Configuration { get; private set; }

        public IList<PeriodReport> Periods { get; private set; }

        public int GenerationsForInitializing { get; private set; } = 0;
        public int EvaluationsForInitializing { get; private set; } = 0;

        private PeriodReport? currentPeriod;

        private double? stability;
        [JsonIgnore]
        public double Stability
        {
            get => stability ??= GetStability();
        }

        private double? errorInPercent;
        [JsonIgnore]
        public double ErrorInPercent
        {
            get => errorInPercent ??= GetErrorInPercent();
        }

        private double? errorInPercentInitializingExcluded;
        [JsonIgnore]
        public double ErrorInPercentInitializingExcluded
        {
            get => errorInPercentInitializingExcluded ??= GetErrorInPercentInitializingExcluded();
        }

        private double? recoveryRate;
        [JsonIgnore]
        public double RecoveryRate
        {
            get => recoveryRate ??= GetRecoveryRate();
        }

        private double? absoluteRecoveryRate;
        [JsonIgnore]
        public double AbsoluteRecoveryRate
        {
            get => absoluteRecoveryRate ??= GetAbsoluteRecoveryRate();
        }


        public AlgorithmReport(string name, AlgorithmConfiguration configuration)
        {
            Name = name;
            Configuration = configuration;
            Periods = new List<PeriodReport>();
        }

        public void ReportInitializationCompleted()
        {
            if(currentPeriod != null && currentPeriod.Generations.Count > 0)
            {
                GenerationsForInitializing = currentPeriod.Generations.Last().Generation;
                EvaluationsForInitializing = currentPeriod.Generations.Last().Evaluations;
            }
        }

        public void ReportNewPeriod(TSP problem, GenerationResult? generation)
        {
            if(currentPeriod != null)
            {
                if(currentPeriod.Generations.Count == 0)
                {
                    // if the period has no generations, delete it
                    Periods.RemoveAt(Periods.Count - 1);
                    currentPeriod = Periods.LastOrDefault();
                } else
                {
                    currentPeriod.EndGeneration = currentPeriod.Generations.Last().Generation;
                }
            }
            currentPeriod = new PeriodReport(Periods.Count, problem, Name, this);
            currentPeriod.CalculateOptimalSolution();
            Periods.Add(currentPeriod);

            if(generation != null)
            {
                currentPeriod.ReportNewGeneration(generation, Configuration.PerformDiversityCalculations, Configuration.PerformEdgeCalculations);
            }
        }

        public void ReportNewGeneration(GenerationResult generation)
        {
            if (currentPeriod == null) throw new ArgumentNullException("No period set yet.");
            currentPeriod.ReportNewGeneration(generation, false, false);
        }

        public void VisitedCity(Edge edge, double distance)
        {
            currentPeriod.VisitedCity(edge, distance);
        }

        public GenerationReport? GetGenerationReport(int generation)
        {
            return Periods.Where(p => p.StartGeneration <= generation).MaxBy(p => p.StartGeneration)?.GetGenerationReport(generation);
        }

        public IList<GenerationReport> GetGenerationReports()
        {
            return Periods.SelectMany(p => p.Generations).ToList();
        }

        public IList<(int evaluations, double error)> GetErrorsInPercent()
        {
            List<(int, double)> errors = new List<(int, double)>();
            foreach (var period in Periods)
            {
                errors.AddRange(period.ErrorsInPercent);
            }
            return errors;
        }

        public IEnumerable<GenerationReport> GenerationReportsWithDiversity()
        {
            return Periods.SelectMany(p => p.Generations).Where(g => g.PopulationDiversity != null);
        }

        public IEnumerable<GenerationReport> GenerationReportsWithEdgeFrequencies()
        {
            return Periods.SelectMany(p => p.Generations).Where(g => g.UniqueEdgesCount != null);
        }

        public void FinishReport()
        {
            if(Periods.Count > 0)
            {
                var lastPeriod = Periods.Last();
                if(lastPeriod.Generations.Count == 0)
                {
                    Periods.Remove(lastPeriod);
                }
                if(Periods.Count > 0)
                {
                    lastPeriod = Periods.Last();
                    lastPeriod.EndGeneration = lastPeriod.Generations.Last().Generation;
                }
            }
        }

        private double GetErrorInPercent()
        {
            return GetErrorsInPercent().Select(p => p.error).Average();
        }

        private double GetErrorInPercentInitializingExcluded()
        {
            return GetErrorsInPercent().Where(p => p.evaluations > EvaluationsForInitializing).Select(p => p.error).Average();
        }

        private double GetStability()
        {
            return Periods.Where(p => p.Stability.HasValue).Select(p => p.Stability.Value).Average();
        }

        private double GetRecoveryRate()
        {
            return Periods.Select(p => p.RecoveryRate).Average();
        }

        private double GetAbsoluteRecoveryRate()
        {
            return Periods.Select(p => p.AbsoluteRecoveryRate).Average();
        }
    }
}
