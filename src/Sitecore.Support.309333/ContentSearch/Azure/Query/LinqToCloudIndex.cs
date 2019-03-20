using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Sitecore.ContentSearch.Azure;
using Sitecore.ContentSearch.Azure.Query;
using Sitecore.ContentSearch.Linq.Common;
using Sitecore.ContentSearch.Linq.Methods;
using Sitecore.ContentSearch.Linq.Nodes;

namespace Sitecore.Support.ContentSearch.Azure.Query
{
  public class LinqToCloudIndex<TItem> : Sitecore.ContentSearch.Azure.Query.LinqToCloudIndex<TItem>
  {

    private readonly Sitecore.Support.ContentSearch.Azure.CloudSearchSearchContext context;

    public LinqToCloudIndex(Sitecore.Support.ContentSearch.Azure.CloudSearchSearchContext context, IExecutionContext[] executionContexts, ServiceCollectionClient serviceCollectionClient) : base(context.originalContext, executionContexts, serviceCollectionClient)
    {
      this.context = context;
    }

    protected override string GetOrderByExpression(CloudQuery query, Sitecore.ContentSearch.Azure.CloudSearchProviderIndex index)
    {
      List<OrderByMethod> source = (from m in query.Methods
                                    where m.MethodType == QueryMethodType.OrderBy
                                    select (OrderByMethod)m).ToList<OrderByMethod>();
      if (!source.Any<OrderByMethod>())
      {
        return string.Empty;
      }
      StringBuilder builder = new StringBuilder();
      foreach (OrderByMethod method in (from x in source
                                       group x by x.Field into x
                                       select x.First<OrderByMethod>()).Reverse())
      {
        string field = method.Field;
        string indexFieldName = this.context.Index.FieldNameTranslator.GetIndexFieldName(field, typeof(TItem));
        if (index.SearchService.Schema.AllFieldNames.Contains(indexFieldName))
        {
          builder.Append(builder.ToString().Contains("$orderby") ? "," : "&$orderby=");
          builder.Append(indexFieldName);
          builder.Append((method.SortDirection == SortDirection.Descending) ? " desc" : string.Empty);
        }
      }
      return builder.ToString();
    }

  }
}