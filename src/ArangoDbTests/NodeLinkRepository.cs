using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using ArangoDbTests.Models;
using ArangoDB.Client;
using ArangoDB.Client.Data;

namespace ArangoDbTests
{
    public class NodeLinkRepository
    {
        private static readonly string DatabaseName = "NodeLinks";
        private static readonly string NodesCollectionName = "Node";
        private static readonly string UsersCollectionName = "User";
        private static readonly string LinksCollectionName = "Link";
        public NodeLinkRepository()
        {
            ArangoDatabase.ChangeSetting(s =>
            {
                s.Database = DatabaseName;
                s.Url = "http://localhost:8529";
                s.Credential = new NetworkCredential("root", "root");
                s.SystemDatabaseCredential = new NetworkCredential("root", "root");
            });
        }

        public void CreateDb()
        {
            using (var db = ArangoDatabase.CreateWithSetting())
            {
                db.CreateDatabase(DatabaseName);
          
                db.CreateCollection(NodesCollectionName);
                db.CreateCollection(UsersCollectionName);
                db.CreateCollection(LinksCollectionName, type:CollectionType.Edge);
            }
        }

        public void InsertNode(Node node)
        {
            using (var db = ArangoDatabase.CreateWithSetting())
            {
                db.Collection<Node>().Insert(node);
            }
        }

        public void InsertNodes(List<Node> nodes)
        {
            using (var db = ArangoDatabase.CreateWithSetting())
            {
                db.Collection<Node>().InsertMultiple(nodes);
            }
        }

        public void InsertUser(User user)
        {
            using (var db = ArangoDatabase.CreateWithSetting())
            {
                db.Collection<User>().Insert(user);
            }
        }

        public void InsertUsers(List<User> users)
        {
            using (var db = ArangoDatabase.CreateWithSetting())
            {
                db.Collection<User>().InsertMultiple(users);
            }
        }

        public void InsertLink(Link link)
        {
            using (var db = ArangoDatabase.CreateWithSetting())
            {
                db.Collection<Link>().Insert(link);
            }
        }

        public void InsertLinks(IEnumerable<Link> links)
        {
            using (var db = ArangoDatabase.CreateWithSetting())
            {
                db.Collection<Link>().InsertMultiple(links.ToList());
            }
        }

        public void CreateGraph(string name)
        {
            using (var db = ArangoDatabase.CreateWithSetting())
            {
                EdgeDefinitionTypedData data = new EdgeDefinitionTypedData
                {
                    From = new List<Type> { typeof(User), typeof(Node) },
                    To = new List<Type> { typeof(Node), typeof(User) },
                    Collection = typeof(Link)
                };

                db.Graph(name).Create(new List<EdgeDefinitionTypedData> { data });
            }
        }
    }
}