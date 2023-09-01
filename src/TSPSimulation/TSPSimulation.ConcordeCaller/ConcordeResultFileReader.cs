using TSPSimulation.ProblemDefinition.Static;

namespace TSPSimulation.ExternalSolvers
{
    public class ConcordeResultFileReader
    {
        public static int[] ReadResult(string filename, TSP problem)
        {
            string[] lines = File.ReadAllLines(filename);
            IList<int> path = new List<int>();

            for (int i=1; i < lines.Length; i++) // omit first line - only contains dimension
            {
                var cityStrings = lines[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                foreach(string cityString in cityStrings)
                {
                    int city = int.Parse(cityString);
                    path.Add(city);
                }
            }
            // translate path to correct city indices
            if (problem.Start == problem.End)
            {
                // in case of roundtrip: depot has index 0, does not need to be translated
                for(int i = 1; i < path.Count; i++)
                {
                    path[i] = problem.Cities[path[i]-1];
                }
            } else
            {
                // not a roundtrip: start = 0, end = path.Length-2, dummy = path.Length-1
                path[0] = problem.Start;
                for (int i = 1; i < path.Count; i++)
                {
                    if (path[i] == path.Count - 2)
                    {
                        path[i] = problem.End;
                        if(i != 2 && i != path.Count - 2)
                        {
                            throw new ConcordeResultException();
                        }
                    } else if (path[i] == path.Count - 1)
                    {
                        path[i] = -1;
                    } else
                    {
                        path[i] = problem.Cities[path[i]-1];
                    }
                }
                path.Remove(-1);
                path.Remove(problem.Start);
                if (path[0] == problem.End)
                {
                    path = path.Reverse().ToList();
                }
            }
            path.Remove(problem.End);
            return path.ToArray();
        }
    }
}
