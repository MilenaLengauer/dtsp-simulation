using TSPSimulation.ProblemDefinition.Static;

namespace TSPSimulation.ExternalSolvers
{
    public class GoogleOrTspModel
    {
        public long[,] DistanceMatrix { get; private set; }
        public int VehicleNumber { get; private set; } = 1;
        public int Depot { get; private set; }

        public GoogleOrTspModel(TSP problem)
        {
            DistanceMatrix = CreateDistanceMatrix(problem);
            Depot = problem.End;
        }

        private long[,] CreateDistanceMatrix(TSP problem)
        {
            var cities = problem.Cities.ToList();
            if(problem.Start == problem.End)
            {
                cities.Add(problem.Start);
            }else
            {
                cities.Add(problem.Start);
                cities.Add(problem.End);
            }
            var distanceMatrix = new long[cities.Count, cities.Count];

            for (int i = 0; i < cities.Count; i++)
            {
                for(int j = i + 1; j < cities.Count; j++)
                {
                    distanceMatrix[i, j] = (long)Math.Round(problem.DistanceMatrix[cities[i], cities[j]] * 1000);
                    distanceMatrix[j, i] = distanceMatrix[i, j];
                }
            }
            return distanceMatrix;
        }
    }
}
