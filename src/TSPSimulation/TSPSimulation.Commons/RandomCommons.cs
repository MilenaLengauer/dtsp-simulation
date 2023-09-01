using System.Collections.Generic;

namespace TSPSimulation.Commons
{
    public static class RandomCommons
    {
        public static void Shuffle<T>(this IList<T> list, ThreadSafeRandom random)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static T RandomChoice<T>(this IList<T> list, ThreadSafeRandom random)
        {
            int index = random.Next(list.Count);
            return list[index];
        }

        public static IList<T> RandomChoice<T>(this IList<T> list, int count, ThreadSafeRandom random)
        {
            IList<T> copy = new List<T>(list);
            IList<T> result = new List<T>();
            for (int i = 0; i < count; i++)
            {
                var selected = copy.RandomChoice(random);
                result.Add(selected);
                copy.Remove(selected);

            }
            return result;
        }

        public static IList<IList<T>> GetPermutations<T>(this T[] array)
        {
            IList<IList<T>> permutations = new List<IList<T>>();
            return GetPermutations(array, 0, array.Length - 1, permutations);
        }

        private static IList<IList<T>> GetPermutations<T>(T[] array, int start, int end, IList<IList<T>> list)
        {
            if (start == end)
            {
                list.Add(new List<T>(array));
            }
            else
            {
                for (var i = start; i <= end; i++)
                {
                    var temp = array[start];
                    array[start] = array[i];
                    array[i] = temp;
                    GetPermutations(array, start + 1, end, list);
                    temp = array[start];
                    array[start] = array[i];
                    array[i] = temp;
                }
            }

            return list;
        }

    }
}
