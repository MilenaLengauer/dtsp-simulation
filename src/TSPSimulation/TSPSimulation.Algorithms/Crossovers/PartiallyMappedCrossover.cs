using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSPSimulation.Commons;
using TSPSimulation.ProblemDefinition.Static;

namespace TSPSimulation.Algorithms.Crossovers
{
    public class PartiallyMappedCrossover : ICrossover
    {
        public TSPSolution Crossover(ThreadSafeRandom random, TSPSolution s1, TSPSolution s2)
        {
            int length = s1.Path.Length;
            int[] result = new int[length];
            int[] invResult = new int[s1.Problem.DistanceMatrix.GetLength(0)];

            int breakPoint1, breakPoint2;
            do
            {
                breakPoint1 = random.Next(length - 1);
                breakPoint2 = random.Next(breakPoint1 + 1, length);
            } while (breakPoint2 - breakPoint1 >= length - 2); // prevent the case [0,length-1) -> clone of parent1

            // clone parent2 and calculate inverse permutation (number -> index)
            for (int j = 0; j < length; j++)
            {
                result[j] = s2.Path[j];
                invResult[result[j]] = j;
            }

            for (int j = breakPoint1; j <= breakPoint2; j++)
            {
                int orig = result[j]; // save the former value
                result[j] = s1.Path[j]; // overwrite the former value with the new value
                int index = invResult[result[j]]; // look where the new value is in the offspring
                result[index] = orig; // write the former value to this position
                invResult[orig] = index; // update the inverse mapping
                                         // it's not necessary to do 'invResult[result[j]] = j' as this will not be needed again
            }
            return new TSPSolution(result, s1.Problem);
        }
    }
}
