using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wiki;

namespace Wiki.Tests
{
    [TestClass]
    public class ExtensionTests
    {
        [TestMethod]
        public void SanatizeWorks()
        {
            // Arrange
            // Act
            var s1 = "test".Sanatize();
            var s2 = "Suitable Title".Sanatize();
            var s3 = "Suitable  Title".Sanatize();
            var s4 = "Th!s is @ T3st".Sanatize();
            var s5 = "54' 40° Orphyte".Sanatize();
            var s6 = "".Sanatize();
            var s7 = "Excalibur 4 - Son of A Sword".Sanatize();
            // Assert
            Assert.AreEqual("Test", s1);
            Assert.AreEqual("SuitableTitle", s2);
            Assert.AreEqual("SuitableTitle", s3);
            Assert.AreEqual("ThSIsT3st", s4);
            Assert.AreEqual("5440Orphyte", s5);
            Assert.AreEqual("", s6);
            Assert.AreEqual("Excalibur4-SonOfASword", s7);
        }
    }
}
