using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSPSimulation.Commons
{
    public class ThreadSafeRandom
    {
        private readonly Random _global;// = new Random(); must be static???
        [ThreadStatic] private static Random? _local;

        public ThreadSafeRandom()
        {
            _global = new Random();
        }

        public ThreadSafeRandom(int seed)
        {
            _global = new Random(seed);
        }

        public int Next()
        {
            if (_local == null)
            {
                int seed;
                lock (_global)
                {
                    seed = _global.Next();
                }
                _local = new Random(seed);
            }

            return _local.Next();
        }

        public int Next(int n)
        {
            if (_local == null)
            {
                int seed;
                lock (_global)
                {
                    seed = _global.Next();
                }
                _local = new Random(seed);
            }

            return _local.Next(n);
        }

        public int Next(int minValue, int maxValue)
        {
            if (_local == null)
            {
                int seed;
                lock (_global)
                {
                    seed = _global.Next();
                }
                _local = new Random(seed);
            }

            return _local.Next(minValue, maxValue);
        }

        public double NextDouble()
        {
            if (_local == null)
            {
                int seed;
                lock (_global)
                {
                    seed = _global.Next();
                }
                _local = new Random(seed);
            }

            return _local.NextDouble();
        }
    }
}
