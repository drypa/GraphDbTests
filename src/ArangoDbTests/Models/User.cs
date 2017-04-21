using ArangoDbTests.Models.Attributes;
using ArangoDB.Client;

namespace ArangoDbTests.Models
{
    [VertexAttriute]
    public class User : ILinkableObject
    {
        [DocumentProperty(Identifier = IdentifierType.Key)]
        public string Id { get; set; }

        public string Name { get; set; }

        public static User Create(string name)
        {
            return new User
            {
                Name = name,
                Id = name
            };
        }
    }
}