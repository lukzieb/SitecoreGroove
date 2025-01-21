using HotChocolate;
using HotChocolate.Types;
using Sitecore;
using Sitecore.Abstractions;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SitecoreGroove.Feature.AuthoringAndManagementApi.RelatedItems
{
    [ExtendObjectType(Name = "Query")]
    public class ReleatedItems
    {
        private readonly BaseClient _client;

        public ReleatedItems(BaseClient client)
        {
            _client = client;
        }

        [GraphQLDescription("Gets related items for requested item.")]
        public IEnumerable<RelatedItem> GetRelatedItems(RelatedItemsQueryInput relatedItemsQuery)
        {
            Item item = _client.GetItemNotNull(ItemUri.Parse(relatedItemsQuery.ItemUri));

            List<Item> allRealtedItems = new List<Item>();

            ResolveReferences(item, allRealtedItems, relatedItemsQuery.FilterByWorkflowState, relatedItemsQuery.ExcludeItemsWithLayout, relatedItemsQuery.Resursive);

            return allRealtedItems
                .OrderBy(x => x.Paths.FullPath)
                .Select(x => new RelatedItem
                {
                    ItemUri = x.Uri.ToString(),
                    Language = x.Language.Name,
                    Name = x.Name,
                    Path = x.Paths.FullPath,
                    TemplateName = x.TemplateName,
                    Updated = x.Statistics.Updated,
                    UpdatedBy = x.Statistics.UpdatedBy,
                    Version = x.Version.Number,
                    WorkflowStateId = Guid.Parse(x[FieldIDs.WorkflowState])
                });
        }

        private void ResolveReferences(Item item, List<Item> allReferences, bool filterByWorkflowState, bool excludeItemsWithLayout, bool recursive)
        {
            IEnumerable<Item> relatedItems = item.Links.GetAllLinks(false, true)
                .Select(x => x.GetTargetItem())
                .Where(x => x != null)
                .Where(x => x.Paths.IsContentItem || x.Paths.IsMediaItem);

            if (excludeItemsWithLayout)
            {
                relatedItems = relatedItems.Where(x => string.IsNullOrEmpty(LayoutField.GetFieldValue(x.Fields[FieldIDs.LayoutField])));
            }

            if (filterByWorkflowState)
            {
                relatedItems = relatedItems.Where(x => x[FieldIDs.WorkflowState] == item[FieldIDs.WorkflowState]);
            }

            foreach (Item relatedItem in relatedItems)
            {
                if (allReferences.Any(x => x.ID == relatedItem.ID))
                {
                    continue;
                }

                allReferences.Add(relatedItem);

                if (recursive)
                {
                    ResolveReferences(relatedItem, allReferences, filterByWorkflowState, excludeItemsWithLayout, recursive);
                }
            }
        }
    }
}