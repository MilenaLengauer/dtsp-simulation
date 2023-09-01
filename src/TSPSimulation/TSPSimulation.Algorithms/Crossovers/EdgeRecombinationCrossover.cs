using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSPSimulation.Commons;
using TSPSimulation.ProblemDefinition.Static;

namespace TSPSimulation.Algorithms.Crossovers
{
    public class EdgeRecombinationCrossover : ICrossover
    {
        public TSPSolution Crossover(ThreadSafeRandom random, TSPSolution parent1, TSPSolution parent2)
        {
            TSP problem = parent1.Problem;
            int length = parent1.Path.Length;
            if (problem.Start == problem.End)
            {
                length += 1;
            } else
            {
                length += 2;
            }
            
            int[] parent1Arr = new int[length];
            int idxParent = 0;
            parent1Arr[idxParent] = problem.End;
            idxParent++;
            if(problem.Start != problem.End)
            {
                parent1Arr[idxParent] = problem.Start;
                idxParent++;
            }
            for(int i = idxParent; i < parent1Arr.Length; i++)
            {
                parent1Arr[i] = parent1.Path[i - idxParent];
            }

            int[] parent2Arr = new int[length];
            idxParent = 0;
            parent2Arr[idxParent] = problem.End;
            idxParent++;
            if(problem.Start != problem.End)
            {
                parent2Arr[idxParent] = problem.Start;
                idxParent++;
            }
            for (int i = idxParent; i < parent2Arr.Length; i++)
            {
                parent2Arr[i] = parent2.Path[i - idxParent];
            }

            int problemSize = parent1.Problem.Coordinates.Length;
            int[] result = new int[length];
            int[,] edgeList = new int[problemSize, 4];
            bool[] remainingNumbers = new bool[problemSize];
            int index, currentEdge, currentNumber, nextNumber, currentEdgeCount, minEdgeCount;

            for (int i = 0; i < problemSize; i++)
            {
                // generate edge list for every number
                remainingNumbers[i] = true;

                index = 0;
                while ((index < length) && (parent1Arr[index] != i))
                {  // search edges in parent1
                    index++;
                }
                if (index == length)
                {
                    //city does not exist for this dynamic problem
                    remainingNumbers[i] = false;
                    continue;
                }
                edgeList[i, 0] = parent1Arr[(index - 1 + length) % length];
                edgeList[i, 1] = parent1Arr[(index + 1) % length];

                index = 0;
                while ((index < length) && (parent2Arr[index] != i))
                {  // search edges in parent2
                    index++;
                }
                currentEdge = parent2Arr[(index - 1 + length) % length];
                if ((edgeList[i, 0] != currentEdge) && (edgeList[i, 1] != currentEdge))
                {  // new edge found ?
                    edgeList[i, 2] = currentEdge;
                }
                else
                {
                    edgeList[i, 2] = -1;
                }
                currentEdge = parent2Arr[(index + 1) % length];
                if ((edgeList[i, 0] != currentEdge) && (edgeList[i, 1] != currentEdge))
                {  // new edge found ?
                    edgeList[i, 3] = currentEdge;
                }
                else
                {
                    edgeList[i, 3] = -1;
                }
            }

            currentNumber = parent1Arr[random.Next(length)];  // get number to start
            bool startAndEndInserted = false;
            for (int i = 0; i < length; i++)
            {
                result[i] = currentNumber;
                remainingNumbers[currentNumber] = false;

                for (int j = 0; j < 4; j++)
                {  // remove all edges to / from currentNumber
                    if (edgeList[currentNumber, j] != -1)
                    {
                        for (int k = 0; k < 4; k++)
                        {
                            if (edgeList[edgeList[currentNumber, j], k] == currentNumber)
                            {
                                edgeList[edgeList[currentNumber, j], k] = -1;
                            }
                        }
                    }
                }

                nextNumber = -1;

                if(problem.Start != problem.End && currentNumber == problem.Start && !startAndEndInserted)
                {
                    nextNumber = problem.End;
                    startAndEndInserted = true;
                } else if (problem.Start != problem.End && currentNumber == problem.End && !startAndEndInserted)
                {
                    nextNumber = problem.Start;
                    startAndEndInserted = true;
                } else
                {
                    minEdgeCount = 5;  // every number hasn't more than 4 edges
                    for (int j = 0; j < 4; j++)
                    {  // find next number with least edges
                        if (edgeList[currentNumber, j] != -1)
                        {  // next number found
                            currentEdgeCount = 0;
                            for (int k = 0; k < 4; k++)
                            {  // count edges of next number
                                if (edgeList[edgeList[currentNumber, j], k] != -1)
                                {
                                    currentEdgeCount++;
                                }
                            }
                            if ((currentEdgeCount < minEdgeCount) ||
                              ((currentEdgeCount == minEdgeCount) && (random.NextDouble() < 0.5)))
                            {
                                nextNumber = edgeList[currentNumber, j];
                                minEdgeCount = currentEdgeCount;
                            }
                        }
                    }
                }
                
                currentNumber = nextNumber;
                if (currentNumber == -1)
                {  // current number has no more edge
                    index = 0;
                    while ((index < problemSize) && (!remainingNumbers[index]))
                    {  // choose next remaining number
                        index++;
                    }
                    if (index < problemSize)
                    {
                        currentNumber = index;
                    }
                }
            }

            //prepare path
            int[] resultPath = new int[parent1.Path.Length];
            int idxStart = Array.IndexOf(result, problem.Start);
            int idxEnd = Array.IndexOf(result, problem.End);
            if(idxStart < idxEnd)
            {
                Array.Reverse(result);
                idxStart = Array.IndexOf(result, problem.Start);
            }
            int idx = (idxStart + 1) % length;
            for(int i = 0; i < resultPath.Length; i++)
            {
                resultPath[i] = result[idx];
                idx = (idx + 1) % length;
            }

            return new TSPSolution(resultPath, parent1.Problem);
        }
    
    }
}
