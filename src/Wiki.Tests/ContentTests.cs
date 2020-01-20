using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Wiki.FileSystem;

namespace Wiki.Tests
{
    [TestClass]
    public class ContentTests
    {
        [TestMethod]
        public async Task SimpleCreateOfFileWiki()
        {
            // Arrange
            var path = Path.GetTempFileName();
            IWiki wiki = null;
            File.Delete(path);
            try
            {
                var services = new ServiceCollection();
                var config = new Configuration.WikiConfiguration(services)
                {
                };
                var factory = new FileWikiFactory(config);
                var create = new WikiOpenOptions
                {
                    NotFound = WikiMissingBehavior.Create,
                    ThrowOnFailureToOpen = true,
                    ThrowOnInvalid = true,
                };
                var flail = new WikiOpenOptions
                {
                    NotFound = WikiMissingBehavior.Throw,
                    ThrowOnFailureToOpen = true,
                    ThrowOnInvalid = true,
                };
                var content = new TestContent();
                var article = new Article
                {
                    Title = "Test",
                }.Set<TestContent>(content);
                // Act
                // Assert
                Assert.IsFalse(content.Stablized);
                Assert.IsFalse(content.Rehydrated);

                wiki = await factory.OpenWikiAsync(path, create);
                await wiki.UpsertAsync(article);
                await wiki.SaveAsync();

                Assert.IsTrue(content.Stablized);
                Assert.IsFalse(content.Rehydrated);

                wiki = await factory.OpenWikiAsync(path, flail);
                var result = await wiki.GetAsync("test");
                await wiki.DisposeAsync();

                Assert.IsTrue(content.Stablized);
                Assert.IsFalse(content.Rehydrated);

                Assert.IsNotNull(result);
                Assert.AreEqual(1, result.Content.Count);
                Assert.IsInstanceOfType(result.Content[0], typeof(TestContent));
                var test = result.Content[0] as TestContent;
                Assert.IsFalse(test.Stablized);
                Assert.IsTrue(test.Rehydrated);

            }
            finally
            {
                if (wiki != null)
                {
                    await wiki.DisposeAsync();
                }
                File.Delete(path);
            }
        }

        public class TestContent : Content
        {
            public bool Rehydrated { get; private set; }
            public bool Stablized { get; private set; }

            protected internal override void Rehydrate(Article article)
            {
                Rehydrated = true;
            }
            protected internal override void Dehydrate(Article article)
            {
                Stablized = true;
            }
        }
    }
}
