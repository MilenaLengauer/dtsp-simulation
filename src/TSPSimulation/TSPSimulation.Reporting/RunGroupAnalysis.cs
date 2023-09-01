using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Google.OrTools.ConstraintSolver.RoutingModel.ResourceGroup;

namespace TSPSimulation.Reporting
{
    public class RunGroupAnalysis
    {
        public string GroupName { get; }
        public int Count { get; }
        public double Min { get; }
        public double Max { get; }
        public double Avg { get; }
        public double StandardDev { get; }
        public double Median { get; }

        public RunGroupAnalysis(string groupName, IEnumerable<double> values)
        {
            GroupName = groupName;
            Count = values.Count();
            Min = values.Min();
            Max = values.Max();
            Avg = values.Average();
            StandardDev = Math.Sqrt(values.Sum(v => (v - Avg) * (v - Avg)) / Count);

            var sortedValues = values.OrderBy(v => v);
            int midpoint = Count / 2;

            if(Count % 2 == 0)
            {
                Median = (sortedValues.ElementAt(midpoint - 1) + sortedValues.ElementAt(midpoint)) / 2.0;
            } else
            {
                Median = sortedValues.ElementAt(midpoint);
            }
        }



    }
}
