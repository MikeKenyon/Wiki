using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wiki.Tests
{
    [TestClass]
    public class FibonacciSequenceTests
    {
        [TestMethod]
        public void GotTheFibPatternDown()
        {
            // Arrange
            var seq = new Internal.FibonacciSequence();
            var i = 0;
            var list = new List<uint>();
            var expected = new[] { 1u, 1u, 2u, 3u, 5u, 8u, 13u, 21u, 34u, 55u };
     
            // Act
            foreach(var fib in seq)
            {
                list.Add(fib);
                ++i;
                if(i == 10)
                {
                    break;
                }
            }
            // Assert
            CollectionAssert.AreEqual(expected, list);
        }
    }
}
