using System;
using System.Collections.Generic;
using ArangoDbTests.Models;

namespace ArangoDbTests
{
    public class Program
    {
        private static readonly int usersCount = 3;
        private static readonly int eachUserNodesCount = 2;

        public static void Main(string[] args)
        {
            var repository = new Repository();
            repository.ReCreateDb();

            CreateUsersWithNodes(repository);

            var users = repository.GetAllUsers();

            LinkUsersTogether(users, repository);

            const string graph = "graph";
            repository.CreateGraph(graph);
            var linkedUsers = repository.GetLinkedUsers(users[1], graph);
        }

        private static void LinkUsersTogether(List<User> users, Repository repository)
        {
            foreach (var user in users)
            {
                var usersLink = CreateLink(user, users);
                repository.InsertLinks(usersLink);
            }
        }

        private static void CreateUsersWithNodes(Repository repository)
        {
            var userNodeLinks = new List<Link>(usersCount * eachUserNodesCount);

            for (var i = 0; i < usersCount; i++)
            {
                var userName = "user" + i;
                User user = User.Create(userName);
                var nodes = new List<Node>(eachUserNodesCount);
                repository.InsertUser(user);
                CreateUserNodes(user, nodes, userNodeLinks);
                
                repository.InsertNodes(nodes);
                repository.InsertLinks(userNodeLinks);

                foreach (var node in nodes)
                {
                    var nodeLinks = CreateLink(node, nodes);
                    repository.InsertLinks(nodeLinks);
                }
            }
        }

        private static void CreateUserNodes(User user, List<Node> nodes, List<Link> userNodeLinks)
        {
            for (var j = 0; j < eachUserNodesCount; j++)
            {
                var nodeName = $"node_{user.Id}_{j}";
                var node = Node.Create(nodeName);
                nodes.Add(node);

                var userNodeLink = CreateLink(user, node);

                userNodeLinks.Add(userNodeLink);
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
                From = object1.GetIdentifier(),
                To = object2.GetIdentifier(),
                Label = $"{object1.Id}_{object2.Id}",
                Key = $"{object1.Id}_{object2.Id}"
            };
        }
    }
}