using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wiki.Tests
{
    [TestClass]
    public class ArticleTests
    {
        [TestMethod]
        public void SetClear()
        {
            // Arrange
            var article = new Article();
            var body = new Body { Markdown = "Boo" };
            // Act
            // Assert
            Assert.AreEqual(0, article.Content.Count);
            
            article.Set(body);
            
            Assert.AreEqual(1, article.Content.Count);
            Assert.IsInstanceOfType(article.Content[0], typeof(Body));
            Assert.AreEqual("Boo", ((Body)article.Content[0]).Markdown);

            article.Set<Body>(null);

            Assert.AreEqual(0, article.Content.Count);
        }
    }
}
