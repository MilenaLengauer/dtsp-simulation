using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSPSimulation.ExternalSolvers
{
    public class ConcordeResultException : Exception
    {
        public ConcordeResultException()
        {
        }

        public ConcordeResultException(string message)
            : base(message)
        {
        }
    }
}
