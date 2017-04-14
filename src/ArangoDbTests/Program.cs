using System;
using System.Collections.Generic;
using ArangoDbTests.Models;

namespace ArangoDbTests
{
    public class Program
    {
        private static readonly int usersCount = 10;
        private static readonly int eachUserNodesCount = 10;

        public static void Main(string[] args)
        {
            var repository = new NodeLinkRepository();
            repository.CreateDb();

            CreateUsersWithNodes(repository);

            var users = repository.GetAllUsers();

            foreach (var user in users)
            {
                var usersLink = CreateLink(user, users);
                repository.InsertLinks(usersLink);
            }

            repository.CreateGraph("first_graph");
        }

        private static void CreateUsersWithNodes(NodeLinkRepository repository)
        {
            var users = new List<User>(usersCount);
            var userNodeLinks = new List<Link>(usersCount * eachUserNodesCount);

            for (var i = 0; i < usersCount; i++)
            {
                var userName = "user" + i;
                var user = User.Create(userName);
                users.Add(user);
                var nodes = new List<Node>(eachUserNodesCount);

                for (var j = 0; j < eachUserNodesCount; j++)
                {
                    var nodeName = $"node_{i}_{j}";
                    var node = Node.Create(nodeName);
                    nodes.Add(node);

                    var userNodeLink = CreateLink(user, node);

                    userNodeLinks.Add(userNodeLink);
                }
                repository.InsertNodes(nodes);
                repository.InsertLinks(userNodeLinks);

                foreach (var node in nodes)
                {
                    var nodeLinks = CreateLink(node, nodes);
                    repository.InsertLinks(nodeLinks);
                }
            }
        }

        private static IEnumerable<Link> CreateLink(ILinkableObject first,
            IEnumerable<ILinkableObject> objectsToLinkWith)
        {
            foreach (var user2 in objectsToLinkWith)
                yield return CreateLink(first, user2);
        }

        private static Link CreateLink(ILinkableObject object1, ILinkableObject object2)
        {
            return new Link
            {
                CreatedDate = DateTime.Now,
                From = $"{object1.GetType().Name}/{object1.Id}",
                To = $"{object2.GetType().Name}/{object2.Id}",
                Label = $"{object1.Id}_{object2.Id}",
                Key = $"{object1.Id}_{object2.Id}"
            };
        }
    }
}