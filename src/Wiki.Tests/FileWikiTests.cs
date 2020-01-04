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
    public class FileWikiTests
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
                var options = new WikiOpenOptions
                {
                    NotFound = WikiMissingBehavior.Create,
                    ThrowOnFailureToOpen = true,
                    ThrowOnInvalid = true,
                };
                var article = new Article
                {
                    Title = "Test", 
                }.Set<Body>(new Body { Markdown = "This is **content**." });
                // Act
                wiki = await factory.OpenWikiAsync(path, options);
                await wiki.UpsertAsync(article);
                await wiki.SaveAsync();
                // Assert
                var file = new FileInfo(path);
                Assert.IsTrue(file.Exists);
                Assert.IsTrue(file.Length > 0);
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
        [TestMethod]
        public async Task RouteTripOneArticleWiki()
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
                var article = new Article
                {
                    Title = "Test",
                }.Set<Body>(new Body { Markdown = "This is **content**." });
                // Act
                wiki = await factory.OpenWikiAsync(path, create);
                await wiki.UpsertAsync(article);
                await wiki.SaveAsync();
                await wiki.DisposeAsync(); // released the lock

                wiki = await factory.OpenWikiAsync(path, flail);
                var result = await wiki.GetAsync("test");
                await wiki.DisposeAsync();
                // Assert
                var file = new FileInfo(path);
                Assert.IsTrue(file.Exists);
                Assert.IsTrue(file.Length > 0);
                AssertArticle(article, result);
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

        private static void AssertArticle(Article article, Article result)
        {
            Assert.IsNotNull(result);
            Assert.AreEqual(article.Title, result.Title);
            Assert.AreEqual(article.Created, result.Created);
            Assert.AreEqual(article.Modified, result.Modified);
            Assert.AreEqual(article.Content.Count, result.Content.Count);
            Assert.IsInstanceOfType(result.Content[0], typeof(Body));
            var made = article.Content[0] as Body;
            var found = article.Content[0] as Body;
            Assert.AreEqual(made.Markdown, found.Markdown);
        }

        [TestMethod]
        public async Task TwoTrips()
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
                var first = new Article
                {
                    Title = "Test",
                }.Set<Body>(new Body { Markdown = "This is **content**." });
                var second = new Article
                {
                    Title = "Alternate Content",
                }.Set<Body>(new Body { Markdown = "This is **alternate content**." });
                // Act
                wiki = await factory.OpenWikiAsync(path, create);
                await wiki.UpsertAsync(first);
                await wiki.SaveAsync();
                await wiki.DisposeAsync(); // released the lock

                wiki = await factory.OpenWikiAsync(path, flail);
                var result = await wiki.GetAsync("test");
                Assert.IsNotNull(result);
                await wiki.UpsertAsync(second);
                await wiki.SaveAsync();
                await wiki.DisposeAsync();

                wiki = await factory.OpenWikiAsync(path, flail);
                var altresult = await wiki.GetAsync("alternatecontent");
                Assert.IsNotNull(result);
                await wiki.DisposeAsync();

                // Assert
                var file = new FileInfo(path);
                Assert.IsTrue(file.Exists);
                Assert.IsTrue(file.Length > 0);
                AssertArticle(first, result);
                AssertArticle(second, altresult);
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
        [TestMethod]
        public async Task AutosaveTest()
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
                    PreferAutosave = true
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
                var first = new Article
                {
                    Title = "Test",
                }.Set<Body>(new Body { Markdown = "This is **content**." });
                var second = new Article
                {
                    Title = "Alternate Content",
                }.Set<Body>(new Body { Markdown = "This is **alternate content**." });
                // Act
                wiki = await factory.OpenWikiAsync(path, create);
                await wiki.UpsertAsync(first);
                await wiki.DisposeAsync(); // released the lock

                wiki = await factory.OpenWikiAsync(path, flail);
                var result = await wiki.GetAsync("test");
                Assert.IsNotNull(result);
                await wiki.UpsertAsync(second);
                await wiki.DisposeAsync();

                wiki = await factory.OpenWikiAsync(path, flail);
                var altresult = await wiki.GetAsync("alternatecontent");
                Assert.IsNotNull(result);
                await wiki.DisposeAsync();

                // Assert
                var file = new FileInfo(path);
                Assert.IsTrue(file.Exists);
                Assert.IsTrue(file.Length > 0);
                AssertArticle(first, result);
                AssertArticle(second, altresult);
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
    }
}
