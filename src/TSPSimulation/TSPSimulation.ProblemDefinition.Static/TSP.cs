using System.Text;

namespace TSPSimulation.ProblemDefinition.Static
{
    public class TSP
    {
        public int Start { get; }
        public int End { get; }
        public int ProblemSize { get; }
        public double[,] DistanceMatrix { get; }
        public int[] Cities { get; }
        public Coordinate[] Coordinates { get; }
        public TSPSolution? OptimalSolution { get; set; }

        public TSP(double[,] distanceMatrix, int start, int end,  int[] cities, Coordinate[] coordinates)
        {
            DistanceMatrix = distanceMatrix;
            Start = start;
            End = end;
            Cities = cities;
            Coordinates = coordinates;
            ProblemSize = Cities.Length;
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder($"TSP: {Start} - {string.Join(",", Cities)} - {End}");
            sb.AppendLine();
            for(int i = 0; i < DistanceMatrix.GetLength(0); i++)
            {
                for(int j = 0; j < DistanceMatrix.GetLength(1); j++)
                {
                    sb.Append((int)DistanceMatrix[i, j]);
                    sb.Append("\t");
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        public override bool Equals(object? obj)
        {
            return obj is TSP tSP &&
                   Start == tSP.Start &&
                   End == tSP.End &&
                   ProblemSize == tSP.ProblemSize &&
                   EqualityComparer<double[,]>.Default.Equals(DistanceMatrix, tSP.DistanceMatrix) &&
                   EqualityComparer<int[]>.Default.Equals(Cities, tSP.Cities) &&
                   EqualityComparer<Coordinate[]>.Default.Equals(Coordinates, tSP.Coordinates);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Start, End, ProblemSize, DistanceMatrix, Cities, Coordinates);
        }
    }

    public record Coordinate(double X, double Y);
}
