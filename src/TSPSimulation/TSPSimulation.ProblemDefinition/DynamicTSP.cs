using TSPSimulation.ProblemDefinition.Static;

namespace TSPSimulation.ProblemDefinition
{
    public class DynamicTSP
    {
        public TSP BaseProblem { get; private set; }
        public IList<(long time, TSPUpdate tspUpdate)> Problems { get; private set; }
        public DynamicTSPConfiguration Configuration { get; private set; }


        public DynamicTSP(TSP baseProblem, IList<(long, TSPUpdate)> problems, DynamicTSPConfiguration configuration)
        {
            BaseProblem = baseProblem;
            Problems = problems;
            Configuration = configuration;
        }

        public TSP GetProblem(long time)
        {
            return GetProblemUpdate(time).Problem;
        }

        public TSPUpdate GetProblemUpdate(long time)
        {
            int i = Problems.Count - 1;
            while(time < Problems[i].time) 
                i--;

            return Problems[i].tspUpdate;
        }

        public Coordinate GetCoordinates(int city)
        {
            return BaseProblem.Coordinates[city];
        }

        public double GetDistance(int city1, int city2)
        {
            return BaseProblem.DistanceMatrix[city1, city2];
        }
    }
}
