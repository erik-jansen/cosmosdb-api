using CarAPI.Models;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CarAPI.Controllers
{
    public static class GlobalVariables
    {
        private static readonly object _syncRoot = new object();
        private static DocumentClient _client;

        public static DocumentClient GetClient()
        {
            string endpointUri = string.Format("https://{0}.documents.azure.com:443/", ConfigurationManager.AppSettings["CosmosDbAccountName"]);
            string primaryKey = ConfigurationManager.AppSettings["CosmosDbAuthorizationKey"];

            lock (_syncRoot)
            {
                if (_client == null)
                {
                    var preferredDCs = ConfigurationManager.AppSettings["PreferredDCs"].Split(',').ToList<string>();
                    ConnectionPolicy connectionPolicy = new ConnectionPolicy
                    {
                        ConnectionMode = ConnectionMode.Direct,
                        ConnectionProtocol = Protocol.Tcp
                    };

                    foreach (var dc in preferredDCs)
                    {
                        connectionPolicy.PreferredLocations.Add(dc);
                    }

                    _client = new DocumentClient(new Uri(endpointUri), primaryKey, connectionPolicy);
                }

                return _client;
            }
        }

    }
    public class CarController : ApiController
    {
        private const string Database = "inventory";
        private const string Collection = "cars";
        private DocumentClient client;
        public CarController()
        {
            client = GlobalVariables.GetClient();
        }

        [HttpGet]
        public Car Get(string id)
        {
            Car car = client.ReadDocumentAsync<Car>(UriFactory.CreateDocumentUri("inventory", "cars", id)).Result;

            return car;
        }

        [HttpPost]
        public Car Post(Car car)
        {
            var response = client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(Database, Collection), car).Result;

            return car;
        }

        [HttpPut]
        public Car Put(Car car)
        {
            var response = client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(Database, Collection, car.Id), car).Result;

            return car;

        }

        [HttpDelete]
        public HttpResponseMessage Delete(string id)
        {
            var response = client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(Database, Collection, id)).Result;

            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
