using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using ArangoDbTests.Models;
using ArangoDB.Client;
using ArangoDB.Client.Data;

namespace ArangoDbTests
{
    public class Repository<T> where T: ILinkableObject
    {
        private static readonly string DatabaseName = "NodeLinks";
        private static readonly int BatchSise = 100;

        public Repository()
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

                var type = typeof(ILinkableObject);
                var assignableTypes = Assembly.GetEntryAssembly().GetExportedTypes().Where(x => x.GetTypeInfo().GetInterfaces().Contains(type));
                foreach (Type implementation in assignableTypes)
                {
                    db.CreateCollection(implementation.Name);
                }
               
                db.CreateCollection(typeof(Link).Name, type: CollectionType.Edge);
            }
        }

        public void InsertDocument(T document)
        {
            using (var db = ArangoDatabase.CreateWithSetting())
            {
                db.Collection<T>().Insert(document);
            }
        }

        public void InsertDocuments<T>(List<T> documents)
        {
            using (var db = ArangoDatabase.CreateWithSetting())
            {
                db.Collection<T>().InsertMultiple(documents);
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