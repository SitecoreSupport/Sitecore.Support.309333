using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Azure;
using Sitecore.ContentSearch.Maintenance;

namespace Sitecore.Support.ContentSearch.Azure
{
  public class CloudSearchProviderIndex : Sitecore.ContentSearch.Azure.CloudSearchProviderIndex
  {
    public CloudSearchProviderIndex(string name, string connectionStringName, string totalParallelServices, IIndexPropertyStore propertyStore) : this(name, connectionStringName, totalParallelServices, propertyStore, null, new ServiceCollectionClient())

    {
    }

    protected internal CloudSearchProviderIndex(string name, string connectionStringName, string totalParallelServices, IIndexPropertyStore propertyStore, string group, ServiceCollectionClient serviceCollectionClient) : base(name, connectionStringName, totalParallelServices, propertyStore, group, serviceCollectionClient)
    {
      serviceCollectionClient.Register<IProviderSearchContext>(typeof(Sitecore.Support.ContentSearch.Azure.CloudSearchSearchContext));
    }

  }
}