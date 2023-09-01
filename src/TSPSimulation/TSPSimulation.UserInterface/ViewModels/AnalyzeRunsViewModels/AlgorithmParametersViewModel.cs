using SimSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSPSimulation.Algorithms.Configuration;
using TSPSimulation.Commons;
using TSPSimulation.ProblemDefinition;

namespace TSPSimulation.UserInterface.ViewModels.AnalyzeRunsViewModels
{
    public class AlgorithmParametersViewModel
    {
        public string AlgorithmName { get; }
        public IEnumerable<string> Properties { get; }

        public AlgorithmParametersViewModel(DynamicTSPSolution solution)
        {
            var config = solution.AlgorithmReport.Configuration;

            AlgorithmName = config.GetAlgorithmName();
            Properties = config.GetType().GetProperties()
                .Select(propertyInfo => $"{propertyInfo.Name}: {propertyInfo.GetValue(config, null) ?? "(null)"}");
        }

    }
}
