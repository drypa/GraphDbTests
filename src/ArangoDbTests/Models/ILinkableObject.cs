namespace ArangoDbTests.Models
{
    public interface ILinkableObject
    {
        string Id { get; set; }

        string Name { get; set; }

        string GetIdentifier();
    }
}