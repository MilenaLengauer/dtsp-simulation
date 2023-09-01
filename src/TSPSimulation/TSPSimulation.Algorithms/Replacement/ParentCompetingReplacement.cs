using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSPSimulation.ProblemDefinition.Static;

namespace TSPSimulation.Algorithms.Replacement
{
    public class ParentCompetingReplacement : IReplacement
    {
        public TSPSolution[] Replace(TSPSolution[] parents, TSPSolution[] children)
        {
            IList<TSPSolution> parentsCopy = new List<TSPSolution>(parents);
            IList<TSPSolution> newGeneration = new List<TSPSolution>();
            for (int i = 0; i < children.Length; i++)
            {
                // find closest parent
                int minDist = Int32.MaxValue;
                int idx = -1;
                for (int j = 0; j < parentsCopy.Count; j++)
                {
                    int dist = children[i].Distance(parentsCopy[j]);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        idx = j;
                    }
                }
                if (children[i].Fitness < parentsCopy[idx].Fitness)
                {
                    newGeneration.Add(children[i]);
                }
                else
                {
                    newGeneration.Add(parentsCopy[idx]);
                    parentsCopy.RemoveAt(idx); // cant use same parent twice
                }
            }
            return newGeneration.ToArray();
        }
    }
}
