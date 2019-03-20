using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Azure;
using Sitecore.ContentSearch.Diagnostics;
using Sitecore.ContentSearch.Linq.Common;
using Sitecore.ContentSearch.Security;
using Sitecore.ContentSearch.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Support.ContentSearch.Azure
{
  public class CloudSearchSearchContext : IProviderSearchContext, IDisposable
  {
    internal Sitecore.ContentSearch.Azure.CloudSearchSearchContext originalContext;
    private readonly ServiceCollectionClient serviceCollectionClient;

    public CloudSearchSearchContext(ServiceCollectionClient serviceCollectionClient, SearchSecurityOptions options =  SearchSecurityOptions.EnableSecurityCheck)
    {
      this.serviceCollectionClient = serviceCollectionClient;
      originalContext = new Sitecore.ContentSearch.Azure.CloudSearchSearchContext(serviceCollectionClient, options);
    }


    public ISearchIndex Index => originalContext.Index;

    public bool ConvertQueryDatesToUtc
    {
      get { return originalContext.ConvertQueryDatesToUtc; }
      set { originalContext.ConvertQueryDatesToUtc = value; }
    }

    public SearchSecurityOptions SecurityOptions => originalContext.SecurityOptions;

    public void Dispose()
    {
      originalContext.Dispose();
    }

    public IQueryable<TItem> GetQueryable<TItem>()
    {
      return this.GetQueryable<TItem>(new IExecutionContext[0]);
    }

    public IQueryable<TItem> GetQueryable<TItem>(IExecutionContext executionContext)
    {
      IExecutionContext[] executionContexts = new IExecutionContext[] { executionContext };
      return this.GetQueryable<TItem>(executionContexts);
    }

    public IQueryable<TItem> GetQueryable<TItem>(params IExecutionContext[] executionContexts)
    {
      object[] args = new object[] { this, executionContexts, this.serviceCollectionClient };
      //Sitecore.Support.ContentSearch.Azure.Query.LinqToCloudIndex<TItem> instance = this.serviceCollectionClient.GetInstance<Sitecore.Support.ContentSearch.Azure.Query.LinqToCloudIndex<TItem>>(args);
      Sitecore.Support.ContentSearch.Azure.Query.LinqToCloudIndex<TItem> instance = new Query.LinqToCloudIndex<TItem>(this, executionContexts, serviceCollectionClient);
      if (this.serviceCollectionClient.GetInstance<IContentSearchConfigurationSettings>(new object[0]).EnableSearchDebug())
      {
        (instance as IHasTraceWriter).TraceWriter = new LoggingTraceWriter(SearchLog.Log);
      }
      return instance.GetQueryable();

    }

    public IEnumerable<SearchIndexTerm> GetTermsByFieldName(string fieldName, string prefix)
    {
      return originalContext.GetTermsByFieldName(fieldName, prefix);
    }
  }
}