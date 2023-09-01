using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSPSimulation.Commons;
using TSPSimulation.ProblemDefinition.Static;

namespace TSPSimulation.Algorithms.Mutators
{
    internal class TranslocationMutator : IMutator
    {
        public void Mutate(ThreadSafeRandom random, TSPSolution tspSolution)
        {
            if (tspSolution.Path.Length < 2) return;

            int breakPoint1, breakPoint2, insertPoint, insertPointLimit;

            breakPoint1 = random.Next(tspSolution.Path.Length - 1);
            breakPoint2 = random.Next(breakPoint1 + 1, tspSolution.Path.Length);
            insertPointLimit = tspSolution.Path.Length - breakPoint2 + breakPoint1 - 1;  // get insertion point in remaining part
            if (insertPointLimit == 0)
                return;
            
            insertPoint = random.Next(insertPointLimit);
            int segmentLength = breakPoint2 - breakPoint1 +  1;

            var original = (int[])tspSolution.Path.Clone();
            int originalIdx = 0;
            int segmentIdx = breakPoint1;
            for(int i = 0; i < tspSolution.Path.Length; i++)
            {
                if(originalIdx == breakPoint1)
                {
                    originalIdx += segmentLength;
                }

                if(i >= insertPoint && i < insertPoint + segmentLength)
                {
                    tspSolution.Path[i] = original[segmentIdx];
                    segmentIdx++;
                } else
                {
                    tspSolution.Path[i] = original[originalIdx];
                    originalIdx++;
                }
            }
        }
    }
}
