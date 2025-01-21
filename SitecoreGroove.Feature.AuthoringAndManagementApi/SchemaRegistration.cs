using Sitecore.GraphQL.Core;
using SitecoreGroove.Feature.AuthoringAndManagementApi.RelatedItems;
using System;
using System.Collections.Generic;

namespace SitecoreGroove.Feature.AuthoringAndManagementApi
{
    public class SchemaRegistration : ISchemaRegistration
    {
        private readonly HashSet<Type> _queryExtensionTypes = new HashSet<Type>();
        private readonly HashSet<Type> _mutationExtensionTypes = new HashSet<Type>();
        private readonly HashSet<Type> _subscriptionExtensionTypes = new HashSet<Type>();

        public IReadOnlyCollection<Type> QueryExtensionTypes => _queryExtensionTypes;
        public IReadOnlyCollection<Type> MutationExtensionTypes => _mutationExtensionTypes;
        public IReadOnlyCollection<Type> SubscriptionExtensionTypes => _subscriptionExtensionTypes;

        public SchemaRegistration()
        {
            _queryExtensionTypes.Add(typeof(ReleatedItems));
        }
    }
}