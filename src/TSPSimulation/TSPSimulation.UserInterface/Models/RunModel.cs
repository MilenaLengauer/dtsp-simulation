using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSPSimulation.ProblemDefinition;

namespace TSPSimulation.UserInterface.Models
{
    public class RunModel
    {
        public DynamicTSP DynamicTSP { get; set; }
        public ObservableCollection<DynamicTSPSolution> Solutions { get; private set; }

        public RunModel(DynamicTSP dynamicTSP)
        {
            DynamicTSP = dynamicTSP;
            Solutions = new ObservableCollection<DynamicTSPSolution>();
        }

        public void RepairAfterImport()
        {
            foreach(var solution in Solutions)
            {
                solution.RepairAfterImport(DynamicTSP);
            }
        }
    }
}
