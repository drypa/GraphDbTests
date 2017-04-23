using ArangoDbTests.Models.Attributes;
using ArangoDB.Client;

namespace ArangoDbTests.Models
{
    [VertexAttriute]
    public class Node : ILinkableObject
    {
        [DocumentProperty(Identifier = IdentifierType.Key)]
        public string Id { get; set; }

        public string Name { get; set; }

        public string GetIdentifier()
        {
            return $"{GetType().Name}/{Id}";
        }

        public static Node Create(string name)
        {
            return new Node
            {
                Name = name,
                Id = name
            };
        }
    }
}