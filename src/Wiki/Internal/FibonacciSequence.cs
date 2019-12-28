using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Wiki.Internal
{
    internal class FibonacciSequence : IEnumerable<uint>
    {
        public IEnumerator<uint> GetEnumerator()
        {
            var last = 0u;
            var prev = 0u;
            var value = 0u;
            while (true)
            {
                value = last + prev;
                if(value == 0u)
                {
                    value = 1u;
                }
                prev = last;
                last = value;
                yield return value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
