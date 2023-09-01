namespace TSPSimulation.ProblemDefinition.Static
{
    public class EdgeFrequency
    {
        public Edge Edge { get; }
        public double Frequency { 
            get => (double)Count / populationSize; 
        }
        public bool EdgeOfBestKnownSolution { get; }
        public double AverageSolutionQuality { 
            get => sumSolutionFitness / populationSize;
        }

        public int Count { get; private set; }
        private double sumSolutionFitness = 0;
        private int populationSize;

        /*public EdgeFrequency(Edge edge, double frequency, bool edgeOfBestKnownSolution, double averageSolutionQuality)
        {
            Edge = edge;
            Frequency = frequency;
            EdgeOfBestKnownSolution = edgeOfBestKnownSolution;
            AverageSolutionQuality = averageSolutionQuality;
        }*/

        public EdgeFrequency(Edge edge, bool edgeOfBestKnownSolution, int populationSize)
        {
            Edge = edge;
            Count = 0;
            EdgeOfBestKnownSolution = edgeOfBestKnownSolution;
            this.populationSize = populationSize;
        }

        public void ReportOccurence(double solutionFitness)
        {
            Count++;
            sumSolutionFitness += solutionFitness;
        }
    }
}
