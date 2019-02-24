using System;
using System.Collections;

namespace ThesisWebApplication.Models
{
    class TrainingItem : IEnumerable
    {
        public double[] Inputs { get; private set; }

        public bool Output { get; private set; }

        public TrainingItem(bool expectedOutput, params double[] inputs)
        {
            Inputs = inputs;
            Output = expectedOutput;
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
