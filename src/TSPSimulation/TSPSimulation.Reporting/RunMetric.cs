using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSPSimulation.Reporting
{
    public enum RunMetric
    {
        Error,
        ErrorInitializingExcluded,
        FinalTourLength,
        Stability,
        RecoveryRate,
        AbsoluteRecoveryRate
    }
}
