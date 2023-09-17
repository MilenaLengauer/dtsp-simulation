using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSPSimulation.UserInterface.Models
{
    public record CsvExportAlgorithmResult(string Algorithm, double Error, double Stability, double RecoveryRate, double AbsoluteRecoveryRate, double Accuracy);

    public class CsvExportResult
    {
        public IList<CsvExportAlgorithmResult> AlgorithmResults { get; set; }

        public CsvExportResult(RunModel runModel)
        {
            AlgorithmResults = new List<CsvExportAlgorithmResult>();
            foreach(var solution in runModel.Solutions)
            {
                AlgorithmResults.Add(new CsvExportAlgorithmResult(
                    solution.AlgorithmReport.Configuration.Name,
                    solution.AlgorithmReport.ErrorInPercentInitializingExcluded,
                    solution.AlgorithmReport.Stability,
                    solution.AlgorithmReport.RecoveryRate,
                    solution.AlgorithmReport.AbsoluteRecoveryRate,
                    solution.AlgorithmReport.AccuracyInitializingExcluded
                    ));
            }
        }
    }
}
