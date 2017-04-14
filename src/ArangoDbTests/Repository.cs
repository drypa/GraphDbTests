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
        private readonly IEnumerable<Type> _assignableTypes;

        public Repository()
        {
            ArangoDatabase.ChangeSetting(s =>
            {
                s.Database = DatabaseName;
                s.Url = "http://localhost:8529";
                s.Credential = new NetworkCredential("root", "root");
                s.SystemDatabaseCredential = new NetworkCredential("root", "root");
            });
            var type = typeof(ILinkableObject);
            _assignableTypes = Assembly.GetEntryAssembly().GetExportedTypes().Where(x => x.GetTypeInfo().GetInterfaces().Contains(type));
        }

        public void CreateDb()
        {
            using (var db = ArangoDatabase.CreateWithSetting())
            {
                db.CreateDatabase(DatabaseName);

                foreach (Type implementation in _assignableTypes)
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

        public void InsertDocuments(IEnumerable<T> documents)
        {
            using (var db = ArangoDatabase.CreateWithSetting())
            {
                db.Collection<T>().InsertMultiple(documents.ToList());
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
                    From = _assignableTypes.ToList(),
                    To = _assignableTypes.ToList(),
                    Collection = typeof(Link)
                };

                db.Graph(name).Create(new List<EdgeDefinitionTypedData> { data });
            }
        }
    }
}