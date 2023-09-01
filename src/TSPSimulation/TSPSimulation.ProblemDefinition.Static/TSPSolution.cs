using Newtonsoft.Json;
using System.Runtime.Intrinsics;

namespace TSPSimulation.ProblemDefinition.Static
{
    public class TSPSolution
    {
        public int[] Path { get; }

        [JsonIgnore]
        public TSP Problem { get; set; }

        public double Fitness { get; private set; }

        private ISet<Edge>? edgeSet;

        public TSPSolution(int[] path, TSP problem)
        {
            Path = path;
            Problem = problem;
        }

        private TSPSolution(int[] path, TSP problem, double fitness)
        {
            Path = path;
            Problem = problem;
            Fitness = fitness;
        }

        public TSPSolution Clone()
        {
            return new TSPSolution(Path.ToArray(), Problem, Fitness);
        }
        public void Evaluate()
        {
            if(Path.Length == 0)
            {
                Fitness = Problem.DistanceMatrix[Problem.Start, Problem.End];
                return;
            }
            double length = Problem.DistanceMatrix[Problem.Start, Path[0]];
            for(int i = 0; i < Path.Length-1; i++)
            {
                length += Problem.DistanceMatrix[Path[i], Path[i + 1]];
            }
            length += Problem.DistanceMatrix[Path[Path.Length-1], Problem.End];
            Fitness = length;
        }

        /*public IEnumerable<Edge> Edges()
        {
            for (int i = 0; i <= Problem.ProblemSize; i++)
            {
                yield return GetEdge(i);
            }
        }*/

        public ISet<Edge> EdgesHashSet()
        {
            if(edgeSet != null)
            {
                return edgeSet;
            }
            edgeSet = new HashSet<Edge>();
            if(Path.Length == 0)
            {
                edgeSet.Add(new Edge(Problem.Start, Problem.End));
                return edgeSet;
            }
            edgeSet.Add(new Edge(Problem.Start, Path[0]));
            edgeSet.Add(new Edge(Path[Path.Length - 1], Problem.End));
            for(int i = 1; i < Path.Length; i++)
            {
                edgeSet.Add(new Edge(Path[i-1], Path[i]));
            }
            return edgeSet;
        }

        public Edge GetEdge(int idx)
        {
            int city1, city2;
            if(Path.Length == 0)
            {
                city1 = Problem.Start;
                city2 = Problem.End;
            } else
            {
                if (idx == 0)
                {
                    city1 = Problem.Start;
                    city2 = Path[idx];
                }
                else if (idx == Path.Length)
                {
                    city1 = Path[idx - 1];
                    city2 = Problem.End;
                }
                else
                {
                    city1 = Path[idx - 1];
                    city2 = Path[idx];
                }
            }
            
            return new Edge(city1, city2);
        }

        public bool ContainsEdge(Edge edge)
        {
            return EdgesHashSet().Contains(edge);
        }

        public int Distance(TSPSolution solution)
        {
            return EdgesHashSet().Except(solution.EdgesHashSet()).Count();

            /*if (Path.Length == 0) return 0;
           
            int countUnique = 0;

            if(Problem.Start == Problem.End)
            {
                if (Path[0] != solution.Path[0] && Path[0] != solution.Path[^1]) countUnique++;
                if (Path[^1] != solution.Path[0] && Path[^1] != solution.Path[^1]) countUnique++;
            } else
            {
                if (Path[0] != solution.Path[0]) countUnique++;
                if (Path[^1] != solution.Path[^1]) countUnique++;
            }

            if (Path.Length == 1) return countUnique;

            int v1, v2;
            for(int i = 1; i < Path.Length; i++)
            {
                v1 = Path[i - 1];
                v2 = Path[i];
                for (int j = 0; j < solution.Path.Length; j++)
                {
                    if (solution.Path[j] == v1)
                    {
                        if ((j == 0 && solution.Path[j + 1] != v2) ||
                            (j == solution.Path.Length - 1 && solution.Path[j - 1] != v2) ||
                            (j > 0 && j < solution.Path.Length - 1 && solution.Path[j - 1] != v2 && solution.Path[j + 1] != v2))
                        {
                            countUnique++;
                        }
                        break;
                    }
                }
            }
            return countUnique;*/
        }

        public override string ToString()
        {
            return $"Solution ({Fitness}): {Problem.Start} - {string.Join(" - ", Path)} - {Problem.End}";
        }

    }
}
