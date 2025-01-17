using System;
using HotChocolate;

namespace SitecoreGroove.Feature.AuthoringAndManagementApi.Models
{
    [GraphQLDescription("Represents a related items search result.")]
    public class RelatedItem
    {
        [GraphQLDescription("Sitecore item Uri")]
        public string ItemUri { get; set; }

        [GraphQLDescription("Sitecore item name")]
        public string Name { get; set; }

        [GraphQLDescription("Sitecore item version")]
        public int Version { get; set; }

        [GraphQLDescription("Sitecore item language")]
        public string Language { get; set; }

        [GraphQLDescription("Sitecore item path")]
        public string Path { get; set; }

        [GraphQLDescription("Sitecore item template name")]
        public string TemplateName { get; set; }

        [GraphQLDescription("Sitecore item last modification date")]
        public DateTime Updated { get; set; }

        [GraphQLDescription("User name who last time modified given Sitecore item")]
        public string UpdatedBy { get; set; }

        [GraphQLDescription("Sitecore item workflow state")]
        public Guid WorkflowStateId { get; set; }
    }
}