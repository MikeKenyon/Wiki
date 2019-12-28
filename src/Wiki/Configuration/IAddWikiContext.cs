using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wiki.Configuration
{
    public interface IAddWikiContext
    {
        void SetWikiFactory<T>() where T : class, IWikiFactory;
        Dictionary<string, object> Parameters { get; }
        IServiceCollection Services { get; }
    }
}
