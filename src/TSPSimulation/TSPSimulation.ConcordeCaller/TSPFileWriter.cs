using TSPSimulation.ProblemDefinition.Static;

namespace TSPSimulation.ExternalSolvers
{
    public class TSPFileWriter
    {
        public static void WriteFile(string filename, string id, TSP problem)
        {
            using (StreamWriter sw = File.CreateText(filename))
            {
                sw.WriteLine($"NAME: {id}");
                sw.WriteLine("TYPE: TSP");
                sw.WriteLine($"DIMENSION: {(problem.Start == problem.End ? problem.ProblemSize + 1 : problem.ProblemSize + 3)}");
                sw.WriteLine("EDGE_WEIGHT_TYPE: EXPLICIT");
                sw.WriteLine("EDGE_WEIGHT_FORMAT:UPPER_ROW");
                sw.WriteLine("EDGE_WEIGHT_SECTION");
                var cities = problem.Cities.ToList();
                if(problem.Start == problem.End)
                {
                    // this is a roundtrip
                    cities.Insert(0, problem.Start);
                    for (int i = 0; i < cities.Count; i++)
                    {
                        for (int j = i + 1; j < cities.Count; j++)
                        {
                            sw.Write($"{Math.Round(problem.DistanceMatrix[cities[i], cities[j]])} ");
                        }
                        sw.WriteLine();
                    }
                } else
                {
                    // not a roundtrip
                    cities.Insert(0, problem.Start);
                    cities.Add(problem.End);
                    cities.Add(-1); // dummy node

                    // add a dummy node between start and end
                    // this dummy node has very high distances to all the cities except for start and end
                    for (int i = 0; i < cities.Count; i++)
                    {
                        for (int j = i + 1; j < cities.Count; j++)
                        {
                            if (cities[i] == -1 || cities[j] == -1)
                            {
                                if (cities[i] == problem.Start || cities[j] == problem.Start || cities[i] == problem.End || cities[j] == problem.End)
                                {
                                    sw.Write($"{Math.Round(problem.DistanceMatrix[problem.Start, problem.End] / 2)} ");
                                } else
                                {
                                    sw.Write($"{Math.Pow(2, 15) - 1} ");
                                }
                            } else
                            {
                                sw.Write($"{Math.Round(problem.DistanceMatrix[cities[i], cities[j]])} ");
                            }
                        }
                        sw.WriteLine();
                    }
                }
                sw.WriteLine("EOF");
            }
        }
    }
}
