using Docker.DotNet.Models;
using Serilog;
using SimSharp;
using TSPSimulation.Commons;
using TSPSimulation.ProblemDefinition;
using TSPSimulation.ProblemDefinition.Static;

namespace TSPSimulation.SimulationRun
{
    public class DynamicTSPCreator
    {
        
        public DynamicTSPConfiguration Configuration { get; }

        private readonly TSPLoader tspLoader;
        private readonly ThreadSafeRandom random;

        private int countRevealCities;
        private int countChangeDistanceMatrix;
        private int nextCityToReveal;

        private int numberOfCitiesToReveal;
        private int numberOfInitiallyKnownCities;

        private TSP tspBaseProblem;
        private IList<int> randomCityOrder;
        private IList<(long time, TSPUpdate tspUpdate)> problems;

        private double minX;
        private double minY;
        private double maxX;
        private double maxY;
        private double diffCoordinatesX;
        private double diffCoordinatesY;

        public DynamicTSPCreator(DynamicTSPConfiguration config, ThreadSafeRandom random)
        {
            Configuration = config;
            tspLoader = new TSPLoader();
            this.random = random;
        }

        public DynamicTSP CreateDynamicTSP()
        {
            Log.Logger.Information("Creating DTSP...");
            countRevealCities = 0;
            countChangeDistanceMatrix = 0;

            tspBaseProblem = tspLoader.LoadProblem(Configuration.ProblemFilePath);
            problems = new List<(long, TSPUpdate)>();

            DetermineCoordinateSpace();

            numberOfInitiallyKnownCities = (int)Math.Ceiling(tspBaseProblem.ProblemSize * Configuration.InitiallyKnownCities);
            numberOfCitiesToReveal = (int)Math.Ceiling(
                (tspBaseProblem.ProblemSize - numberOfInitiallyKnownCities) / (double)Configuration.RevealCitiesXTimes);

            GenerateInitialProblem();

            var env = new Simulation();
            if(Configuration.FrequencyDistanceMatrixChange > 0)
            {
                env.Process(ChangeDistanceMatrix(env));
            }
            if(Configuration.FrequencyRevealCities > 0)
            {
                env.Process(RevealCities(env));
            }

            env.Run();

            Log.Logger.Information("Successfully created DTSP");

            return new DynamicTSP(tspBaseProblem, problems, Configuration);
        }

        private void DetermineCoordinateSpace()
        {
            minX = tspBaseProblem.Coordinates.Select(c => c.X).Min();
            maxX = tspBaseProblem.Coordinates.Select(c => c.X).Max();
            diffCoordinatesX = maxX - minX;
            minY = tspBaseProblem.Coordinates.Select(c => c.Y).Min();
            maxY = tspBaseProblem.Coordinates.Select(c => c.Y).Max();
            diffCoordinatesY = maxY - minY;
        }

        private void GenerateInitialProblem()
        {
            randomCityOrder = Enumerable.Range(1, tspBaseProblem.ProblemSize).ToList();
            randomCityOrder.Shuffle<int>(random);
            nextCityToReveal = (int)Math.Ceiling(tspBaseProblem.ProblemSize * Configuration.InitiallyKnownCities);
            int[] cities = new int[nextCityToReveal];
            for (int i = 0; i < nextCityToReveal; i++)
            {
                cities[i] = randomCityOrder[i];
            }
            problems.Add((0, new TSPUpdate(TSPUpdateType.Initial, Array.Empty<int>(), 
                new TSP(tspBaseProblem.DistanceMatrix, tspBaseProblem.Start, tspBaseProblem.End, cities, tspBaseProblem.Coordinates))));
        }

        private IEnumerable<Event> RevealCities(Simulation env)
        {
            while (countRevealCities < Configuration.RevealCitiesXTimes && !AllCitiesRevealed())
            {
                yield return env.TimeoutD(Configuration.FrequencyRevealCities);

                TSP lastProblem = problems[^1].tspUpdate.Problem;
                int previousNextCityToReveal = nextCityToReveal;
                nextCityToReveal += numberOfCitiesToReveal;
                if (nextCityToReveal > tspBaseProblem.ProblemSize)
                {
                    nextCityToReveal = tspBaseProblem.ProblemSize;
                }
                IList<int> addedCities = new List<int>();
                IList<int> cities = new List<int>();
                for (int i = 0; i < nextCityToReveal; i++)
                {
                    cities.Add(randomCityOrder[i]);
                    if (i >= previousNextCityToReveal)
                    {
                        addedCities.Add(randomCityOrder[i]);
                    }
                }
                var tsp = new TSP(lastProblem.DistanceMatrix, tspBaseProblem.Start, tspBaseProblem.End, cities.ToArray(), lastProblem.Coordinates);
                if (problems[^1].time == (long)env.NowD)
                {
                    problems[^1] = ((long)env.NowD, new TSPUpdate(TSPUpdateType.BothChanged, addedCities.ToArray(), tsp));
                }
                else
                {
                    problems.Add(((long)env.NowD, new TSPUpdate(TSPUpdateType.NewCities, addedCities.ToArray(), tsp)));
                }
                countRevealCities++;
            }
        }

        private IEnumerable<Event> ChangeDistanceMatrix(Simulation env)
        {
            while (countChangeDistanceMatrix < Configuration.ChangeDistanceMatrixXTimes)
            {
                yield return env.TimeoutD(Configuration.FrequencyDistanceMatrixChange);

                TSP lastProblem = problems[^1].tspUpdate.Problem;

                Coordinate[] newCoordinates = new Coordinate[tspBaseProblem.ProblemSize + 1];
                TSP baseTspForChange;
                if(Configuration.UseLastAsChangeBase)
                {
                    baseTspForChange = lastProblem;
                } 
                else
                {
                    baseTspForChange = tspBaseProblem;
                }

                // moving cities
                for(int i = 0; i < newCoordinates.Length; i++)
                {
                    if(random.NextDouble() < Configuration.MagnitudeOfChange)
                    {
                        double randomX = random.NextDouble() * diffCoordinatesX;
                        double randomY = random.NextDouble() * diffCoordinatesY;

                        double newX = baseTspForChange.Coordinates[i].X * (1 - Configuration.CoordinateChangeCoefficient) +
                            randomX * Configuration.CoordinateChangeCoefficient;
                        double newY = baseTspForChange.Coordinates[i].Y * (1 - Configuration.CoordinateChangeCoefficient) +
                            randomY * Configuration.CoordinateChangeCoefficient;

                        newCoordinates[i] = new Coordinate(newX, newY);
                    } else
                    {
                        Coordinate coordinate = baseTspForChange.Coordinates[i];
                        newCoordinates[i] = new Coordinate(coordinate.X, coordinate.Y);
                    }
                }

                // randomly placing cities
                if(Configuration.FractionRandomCities > 0)
                {
                    for (int i = 0; i < newCoordinates.Length; i++)
                    {
                        if (random.NextDouble() < Configuration.FractionRandomCities)
                        {
                            double newX = random.NextDouble() * diffCoordinatesX + minX;
                            double newY = random.NextDouble() * diffCoordinatesY + minY;
                            newCoordinates[i] = new Coordinate(newX, newY);
                        }
                        else
                        {
                            Coordinate coordinate = baseTspForChange.Coordinates[i];
                            newCoordinates[i] = new Coordinate(coordinate.X, coordinate.Y);
                        }
                    }
                }

                // re-evaluate distance matrix
                double[,] distanceMatrix = new double[tspBaseProblem.ProblemSize + 1, tspBaseProblem.ProblemSize + 1];
                int changedEdges = 0;
                for(int i = 0; i < tspBaseProblem.ProblemSize + 1; i++)
                {
                    for(int j = i + 1; j < tspBaseProblem.ProblemSize + 1; j++)
                    {
                        double newVal = Math.Sqrt(Math.Pow(newCoordinates[i].X - newCoordinates[j].X, 2) + Math.Pow(newCoordinates[i].Y - newCoordinates[j].Y, 2));
                        double oldVal = baseTspForChange.DistanceMatrix[i, j];
                        if(Math.Abs(newVal - oldVal) > 0.001)
                        {
                            changedEdges++;
                        }
                        distanceMatrix[i, j] = Math.Sqrt(Math.Pow(newCoordinates[i].X - newCoordinates[j].X, 2) + Math.Pow(newCoordinates[i].Y - newCoordinates[j].Y, 2));
                        distanceMatrix[j, i] = distanceMatrix[i, j];
                    }
                }

                var tsp = new TSP(distanceMatrix, tspBaseProblem.Start, tspBaseProblem.End, lastProblem.Cities, newCoordinates);
                if (problems[^1].time == (long)env.NowD)
                {
                    problems[^1] = ((long)env.NowD, new TSPUpdate(TSPUpdateType.BothChanged, problems[^1].tspUpdate.AddedCities, tsp));
                }
                else
                {
                    problems.Add(((long)env.NowD, new TSPUpdate(TSPUpdateType.DistanceMatrixChange, Array.Empty<int>(), tsp)));
                }
                countChangeDistanceMatrix++;
            }
        }

        private bool AllCitiesRevealed()
        {
            return nextCityToReveal >= tspBaseProblem.Cities.Length;
        }


    }
}
