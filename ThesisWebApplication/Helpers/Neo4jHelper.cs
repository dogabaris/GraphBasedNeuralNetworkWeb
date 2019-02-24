using System;
using Neo4jClient;

namespace ThesisWebApplication.Helpers
{
    public class Neo4JHelper
    {
        public Neo4JHelper()
        { }

        public static GraphClient ConnectDb()
        {
            var client = new GraphClient(new Uri("http://localhost:7474/db/data"), "neo4j", "password");
            client.Connect();
            Console.WriteLine(client.IsConnected ? "Neo4j DB Connected!" : "Neo4j DB Not Connected!!!");
            return client;
        }
    }
}