﻿using ArangoDB.Client;

namespace ArangoDbTests.Models
{
    public class Node: ILinkableObject
    {
        [DocumentProperty(Identifier = IdentifierType.Key)]
        public string Id { get; set; }

        public string Name { get; set; }

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