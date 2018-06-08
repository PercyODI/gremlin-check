using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.Graphs;
using Microsoft.Azure.Graphs.Elements;
using Newtonsoft.Json.Linq;

namespace gremlin_check
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            RunAsyncAgain().Wait();

            // var graph = new Graph();
            // var gremlin = new DriverRemoteConnection(new GremlinClient(new GremlinServer("ph-roomies.gremlin.cosmosdb.azure.com", 443, true, "/dbs/roomies/colls/Collection1", "5EJEogxDiKZle4lLTqS3TjSlnzeoF22uAehGCsWEp9fne4yFiPOPGaXSjlMznFOMT33SAPZmpffsmhvFPgrPww==")));

        }

        public static async Task RunAsyncAgain()
        {
            var gremlinDao = await GremlinDao.BuildGremlinDao();
            var billStrings = await gremlinDao.GetObjectJsonStringsByLabel("bill");
            // Console.WriteLine(string.Join("\n\n", billStrings));

            var billObj = await gremlinDao.GetObjectByLabel<Bill>("bill");
            Console.WriteLine(billObj.FirstOrDefault().OwedParty);
        }

        // public static async Task RunAsync(DocumentClient client)
        // {
        //     var database = await client.CreateDatabaseIfNotExistsAsync(new Database { Id = "roomies" });

        //     var graph = await client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri("roomies"), new DocumentCollection { Id = "Collection1" }, new RequestOptions { OfferThroughput = 1000 });

        //     var testBill = new Bill
        //     {
        //         OwedParty = "Dell",
        //         TotalOwed = 123.45m
        //     };
        //     var addBill = await client.CreateGremlinQuery<Vertex>(graph, $"g.addV('bill'){BuildPropertiesForObject(testBill)}").ExecuteNextAsync();

        //     // --- --- ---

        //     // var query = client.CreateGremlinQuery<Vertex>(graph, "g.V()");

        //     // Console.WriteLine(JObject.FromObject(query).ToString());

        //     // if (query.HasMoreResults)
        //     // {
        //     //     foreach (var result in await query.ExecuteNextAsync<Vertex>())
        //     //     {
        //     //         var update = await client.CreateGremlinQuery<Vertex>(graph, $"g.v('{result.Id}').property('name', 'TSW')").ExecuteNextAsync();
        //     //         Console.WriteLine($"name: {result.GetVertexProperties("name").First().Value}");
        //     //         Console.WriteLine(JObject.FromObject(result).ToString());
        //     //     }
        //     // }
        // }

    }
}