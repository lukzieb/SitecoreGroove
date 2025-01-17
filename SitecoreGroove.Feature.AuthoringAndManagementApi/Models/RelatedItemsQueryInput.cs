using HotChocolate;
using HotChocolate.Types;

namespace SitecoreGroove.Feature.AuthoringAndManagementApi.Models
{
    [InputObjectType]
    [GraphQLDescription("Represents a related items query input.")]
    public class RelatedItemsQueryInput
    {
        [GraphQLNonNullType]
        [GraphQLDescription("Sitecore item Uri")]
        public string ItemUri { get; set; }

        [GraphQLDescription("Indicates if related items should be of the same workflow step. If true and requested item is of 'Draft' state, then it will reurn 'Draft' related items only.")]
        public bool FilterByWorkflowState { get; set; }

        [GraphQLDescription("Indicates if related items should inlcude items that has any presentation details defined.")]
        public bool ExcludeItemsWithLayout { get; set; }

        [GraphQLDescription("If false then direct related items only are returned. If true then related items are evaluated from related item until the last one.")]
        public bool Resursive { get; set; }
    }
}