using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using ArangoDbTests.Models;
using ArangoDbTests.Models.Attributes;
using ArangoDB.Client;
using ArangoDB.Client.Data;

namespace ArangoDbTests
{
    public class Repository
    {
        private static readonly string DatabaseName = "NodeLinks";
        private static readonly int BatchSise = 100;
        private readonly List<Type> _edgeTypes;
        private readonly List<Type> _vertexTypes;

        public Repository()
        {
            ArangoDatabase.ChangeSetting(s =>
            {
                s.Database = DatabaseName;
                s.Url = "http://localhost:8529";
                s.Credential = new NetworkCredential("root", "root");
                s.SystemDatabaseCredential = new NetworkCredential("root", "root");
            });

            _vertexTypes = Assembly
                .GetEntryAssembly()
                .GetExportedTypes()
                .Where(x => x.GetTypeInfo().GetCustomAttributes<VertexAttriute>(false).Any())
                .ToList();
            _edgeTypes = Assembly
                .GetEntryAssembly()
                .GetExportedTypes()
                .Where(x => x.GetTypeInfo().GetCustomAttributes<EdgeAttribute>(false).Any())
                .ToList();
        }

        public void ReCreateDb()
        {
            using (var db = ArangoDatabase.CreateWithSetting())
            {
                if (db.ListDatabases().Contains(DatabaseName))
                    db.DropDatabase(DatabaseName);
                db.CreateDatabase(DatabaseName);

                RegisterTypes(db);
            }
        }

        private void RegisterTypes(IArangoDatabase db)
        {
            foreach (var implementation in _vertexTypes)
                db.CreateCollection(implementation.Name);

            db.CreateCollection(typeof(Link).Name, type: CollectionType.Edge);
        }

        public void CreateDb()
        {
            using (var db = ArangoDatabase.CreateWithSetting())
            {
                db.CreateDatabase(DatabaseName);

                RegisterTypes(db);
            }
        }
        public void InsertUser(User user)
        {
            using (var db = ArangoDatabase.CreateWithSetting())
            {
                db.Collection<User>().Insert(user);
            }
        }
        public void InsertNode(Node node)
        {
            using (var db = ArangoDatabase.CreateWithSetting())
            {
                db.Collection<Node>().Insert(node);
            }
        }


        public void DeleteUser(User user)
        {
            using (var db = ArangoDatabase.CreateWithSetting())
            {
                db.Collection<User>().Remove(user);
            }
        }
        public void DeleteNode(Node node)
        {
            using (var db = ArangoDatabase.CreateWithSetting())
            {
                db.Collection<Node>().Remove(node);
            }
        }

        public void InsertUsers(IEnumerable<User> users)
        {
            using (var db = ArangoDatabase.CreateWithSetting())
            {
                var linksList = users.ToList();
                for (var i = 0; i < linksList.Count; i += BatchSise)
                {
                    var batch = linksList.Skip(i).Take(BatchSise).ToList();
                    db.Collection<User>().InsertMultiple(batch);
                }
            }
        }

        public void InsertNodes(IEnumerable<Node> nodes)
        {
            using (var db = ArangoDatabase.CreateWithSetting())
            {
                var linksList = nodes.ToList();
                for (var i = 0; i < linksList.Count; i += BatchSise)
                {
                    var batch = linksList.Skip(i).Take(BatchSise).ToList();
                    db.Collection<Node>().InsertMultiple(batch);
                }
            }
        }

        public List<User> GetAllUsers()
        {
            using (var db = ArangoDatabase.CreateWithSetting())
            {
                return db.Collection<User>().All().ToList();
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
                var linksList = links.ToList();
                for (var i = 0; i < linksList.Count; i += BatchSise)
                {
                    var batch = linksList.Skip(i).Take(BatchSise).ToList();
                    db.Collection<Link>().InsertMultiple(batch);
                }
            }
        }

        public void CreateGraph(string name)
        {
            using (var db = ArangoDatabase.CreateWithSetting())
            {
                var edgeDefinitions = _edgeTypes.Select(x =>
                        new EdgeDefinitionTypedData
                        {
                            From = _vertexTypes.ToList(),
                            To = _vertexTypes.ToList(),
                            Collection = x
                        }
                    )
                    .ToList();

                db.Graph(name).Create(edgeDefinitions);
            }
        }
    }
}