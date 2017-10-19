using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

// ADD THIS PART TO YOUR CODE
using System.Net;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using System.IO;
using Microsoft.IdentityModel.Protocols;
using System.Configuration;
using System.Threading.Tasks;

namespace AngkasaPura.Botsky.Helpers
{
    public class CosmosDB : IDataRepository
    {
        private readonly string EndpointUrl = ConfigurationManager.AppSettings["EndPointUrl"];
        private readonly string PrimaryKey = ConfigurationManager.AppSettings["PrimaryKey"];
        private DocumentClient client;
        public CosmosDB()
        {
            this.client = new DocumentClient(new Uri(EndpointUrl), PrimaryKey);// ADD THIS PART TO YOUR CODE
          
        }

        public string DatabaseName { get; set; } = "AngkasaPuraDB";

        public async Task<bool> InsertDoc<T>(string CollectionName, T Data)
        {
            try
            {
                await this.client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseName,CollectionName), Data);
                return await Task.FromResult(true);
            }
            catch
            {
                return await Task.FromResult(false);
            }
        }

        public List<T> GetDataById<T>(string CollectionName, string ID)
        {
            // Set some common query options
            FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1 };
            
            // Now execute the same query via direct SQL
            IQueryable<T> TQueryInSql = this.client.CreateDocumentQuery<T>(
                    UriFactory.CreateDocumentCollectionUri(DatabaseName, CollectionName),
                    $"SELECT * FROM {CollectionName} WHERE {CollectionName}.Id = '{ID}'",
                    queryOptions);

            Console.WriteLine("Running direct SQL query...");
            return TQueryInSql.ToList();
        }

    

        public List<T> GetDataByQuery<T>(string CollectionName, string Query)
        {

            // Set some common query options
            FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1 };

            // Now execute the same query via direct SQL
            IQueryable<T> TQueryInSql = this.client.CreateDocumentQuery<T>(
                    UriFactory.CreateDocumentCollectionUri(DatabaseName, CollectionName),
                    Query,
                    queryOptions);

            Console.WriteLine("Running direct SQL query...");
            return TQueryInSql.ToList();
        }
    }
}