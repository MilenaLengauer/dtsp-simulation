using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSPSimulation.Algorithms.Selectors
{
    public enum SelectionOperators
    {
        TournamentSelector,
        RandomSelector,
        RankSelector,
        GeneralizedRankSelector,
        ProportionalSelector
    }
}
