using System;
using ArangoDbTests.Models.Attributes;
using ArangoDB.Client;

namespace ArangoDbTests.Models
{
    [Edge]
    public class Link
    {
        [DocumentProperty(Identifier = IdentifierType.Key)]
        public string Key { get; set; }

        [DocumentProperty(Identifier = IdentifierType.Handle)]
        public string Id { get; set; }

        [DocumentProperty(Identifier = IdentifierType.EdgeFrom)]
        public string From { get; set; }

        [DocumentProperty(Identifier = IdentifierType.EdgeTo)]
        public string To { get; set; }

        public DateTime CreatedDate { get; set; }

        public string Label { get; set; }
    }
}