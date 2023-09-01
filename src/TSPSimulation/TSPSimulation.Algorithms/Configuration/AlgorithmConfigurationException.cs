using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TSPSimulation.Algorithms.Configuration
{
    public class AlgorithmConfigurationException : Exception
    {
        public AlgorithmConfigurationException()
        {
        }

        public AlgorithmConfigurationException(string? message) : base(message)
        {
        }

        public AlgorithmConfigurationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected AlgorithmConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
