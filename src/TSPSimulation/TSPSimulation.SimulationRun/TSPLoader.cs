using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System.Globalization;
using TSPSimulation.ProblemDefinition.Static;

namespace TSPSimulation.SimulationRun
{
    internal class TSPLoader
    {

        private readonly CsvConfiguration csvConfig;

        public TSPLoader()
        {
            csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = " ",
                ShouldSkipRecord = args =>
                {
                    var record = args.Row.Parser.Record;
                    return record.Length < 3 || !record[0].All(char.IsDigit);
                },
                HasHeaderRecord = false,
                TrimOptions = TrimOptions.None,
                WhiteSpaceChars = Array.Empty<char>()
            };
        }


        public TSP LoadProblem(string problemName)
        {
            using var reader = new StreamReader($".\\TSPs\\{problemName}.tsp");
            using var csv = new CsvReader(reader, csvConfig);
            csv.Context.RegisterClassMap<CsvCityMap>();
            var records = csv.GetRecords<CsvCity>();
            var cities = records.ToList<CsvCity>();

            int dimension = cities.Count;
            double[,] distanceMatrix = new double[dimension, dimension];
            Coordinate[] coordinates = new Coordinate[dimension];
            for (int i = 0; i < dimension; i++)
            {
                coordinates[i] = new Coordinate(cities[i].X, cities[i].Y);
                for (int j = 0; j < dimension; j++)
                {
                    distanceMatrix[i, j] = Math.Sqrt(Math.Pow(cities[i].X - cities[j].X, 2) + Math.Pow(cities[i].Y - cities[j].Y, 2));
                }
            }
            return new TSP(distanceMatrix, 0, 0, Enumerable.Range(1, dimension - 1).ToArray(), coordinates);

        }

        class CsvCity
        {
            public int City { get; set; }
            public double X { get; set; }
            public double Y { get; set; }
        }

        class CsvCityMap : ClassMap<CsvCity>
        {
            public CsvCityMap()
            {
                Map(m => m.City).Index(0).TypeConverter<Int32Converter>();
                Map(m => m.X).Index(1).TypeConverter<DoubleConverter>();
                Map(m => m.Y).Index(2).TypeConverter<DoubleConverter>();
            }
        }

    }
}
