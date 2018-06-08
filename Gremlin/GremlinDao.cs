using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Graphs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace gremlin_check
{
    public class GremlinDao
    {
        private static DocumentClient client = new DocumentClient(
            new Uri("https://ph-roomies.documents.azure.com:443/"),
            "5EJEogxDiKZle4lLTqS3TjSlnzeoF22uAehGCsWEp9fne4yFiPOPGaXSjlMznFOMT33SAPZmpffsmhvFPgrPww==",
            new ConnectionPolicy { ConnectionMode = ConnectionMode.Direct, ConnectionProtocol = Protocol.Tcp });
        private ResourceResponse<Database> database { get; set; }

        private ResourceResponse<DocumentCollection> graph { get; set; }

        public async static Task<GremlinDao> BuildGremlinDao()
        {
            var gremlinDao = new GremlinDao();
            gremlinDao.database = await client.CreateDatabaseIfNotExistsAsync(new Database { Id = "roomies" });
            gremlinDao.graph = await client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri("roomies"), new DocumentCollection { Id = "Collection1" }, new RequestOptions { OfferThroughput = 1000 });

            return gremlinDao;
        }

        private GremlinDao() { }

        public async Task<IEnumerable<string>> GetObjectJsonStringsByLabel(string label)
        {
            var returnStrs = new List<string>();
            var query = client.CreateGremlinQuery<dynamic>(graph, $"g.V().hasLabel('{label}')");
            while (query.HasMoreResults)
            {
                foreach (dynamic result in await query.ExecuteNextAsync())
                {
                    returnStrs.Add(JObject.FromObject(result).ToString());
                }
            }

            return returnStrs.ToArray();
        }

        public async Task<IEnumerable<T>> GetObjectByLabel<T>(string label)
        where T : new()
        {
            var returnObjs = new List<T>();
            var objectProperties = typeof(T).GetProperties();
            var query = client.CreateGremlinQuery<JObject>(graph, $"g.V().hasLabel('{label}')");
            while (query.HasMoreResults)
            {
                foreach (JToken result in await query.ExecuteNextAsync())
                {
                    var newObj = new T();
                    foreach (var prop in objectProperties)
                    {
                        prop.SetValue(newObj,  result["properties"][prop.Name][0]["value"].ToObject(prop.PropertyType));
                    }
                    returnObjs.Add(newObj);
                    // JObject jObj = JObject.FromObject(result);
                    // returnObjs.Add(jObj.ToObject<T>());
                }
            }

            return returnObjs;
        }

        public static string BuildPropertiesForObject<T>(T obj)
        {
            var props = typeof(T).GetProperties();
            var propertiesStr = "";
            foreach (var property in props)
            {
                if (property.PropertyType == typeof(decimal) || property.PropertyType == typeof(int))
                {
                    propertiesStr = propertiesStr + $".property('{property.Name}', {typeof(T).GetProperty(property.Name).GetValue(obj).ToString()})";
                }
                else
                {
                    propertiesStr = propertiesStr + $".property('{property.Name}', '{typeof(T).GetProperty(property.Name).GetValue(obj).ToString()}')";
                }
            }

            return propertiesStr;
        }
    }
}