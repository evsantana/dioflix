using fnGetMovieDetail;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace fnGetAllMovies
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;
        private readonly CosmosClient _cosmosClient;

        public Function1(ILogger<Function1> logger, CosmosClient cosmosClient)
        {
            _logger = logger;
            _cosmosClient = cosmosClient;
        }

        [Function("all")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var container = _cosmosClient.GetContainer("DioflixDB", "movies");
            var id = req.Query["id"];

            var query = $"SELECT * FROM c";
            var queryDefinition = new QueryDefinition(query);

            var result = container.GetItemQueryIterator<MovieResult>(queryDefinition);

            var list = new List<MovieResult>();

            foreach (var item in await result.ReadNextAsync())
            {
                list.Add(item);
            }

            var responseMessage = req.CreateResponse(System.Net.HttpStatusCode.OK);
            await responseMessage.WriteAsJsonAsync(list.FirstOrDefault());

            return responseMessage;
            //while(result.HasMoreResults)
            //{

            //}

            //return new OkObjectResult(result);


        }
    }
}
