namespace TSPSimulation.ProblemDefinition.Static
{
    public class Edge
    {
        public int City1 { get; }
        public int City2 { get; }

        public Edge(int city1, int city2)
        {
            City1 = city1;
            City2 = city2;
        }

        public override int GetHashCode()
        {
            //"sort" the cities, so that it does not matter which of the cities is City1 and which is City2
            int firstCity = City1 <= City2 ? City1 : City2;
            int secondCity = firstCity == City1 ? City2 : City1;
            return HashCode.Combine(firstCity, secondCity);
        }

        public override bool Equals(object? obj)
        {
            return obj is Edge edge && ((edge.City1 == City1 && edge.City2 == City2) || (edge.City1 == City2 && edge.City2 == City1));
        }
    }
}
