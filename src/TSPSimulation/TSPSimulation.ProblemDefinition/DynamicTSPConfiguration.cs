namespace TSPSimulation.ProblemDefinition
{
    public record DynamicTSPConfiguration (
            string ProblemFilePath,
            double DistanceBestKnownTour,
            int RuntimeForBestKnown
        )
    {
        public long TimeForInitialSolution { get; set; } = 1_000_000;
        public long FrequencyDistanceMatrixChange { get; set; } = 100_000;
        public int ChangeDistanceMatrixXTimes { get; set; } = 10;
        public double FractionRandomCities { get; set; } = 0;
        public double MagnitudeOfChange { get; set; } = 0.25;
        public double CoordinateChangeCoefficient { get; set; } = 0.1;
        public bool UseLastAsChangeBase { get; set; } = true;
        public double InitiallyKnownCities { get; set; } = 0.4;
        public long FrequencyRevealCities { get; set; } = 100_000;
        public int RevealCitiesXTimes { get; set; } = 5;
        public int? RandomState { get; set; }

        public double TimeForDistanceUnit { get; } = RuntimeForBestKnown / DistanceBestKnownTour;


        public long TimeLastDistanceMatrixChange()
        {
            return ChangeDistanceMatrixXTimes * FrequencyDistanceMatrixChange;
        }

        public long TimeLastCityReveal()
        {
            return RevealCitiesXTimes * FrequencyRevealCities;
        }
    }
}
