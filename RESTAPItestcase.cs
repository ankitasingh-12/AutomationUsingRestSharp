using RestSharp;
using FluentAssertions;
using Newtonsoft.Json.Linq;

namespace APIAutomationDemo
{
    public class RESTAPItestcase
    {
        RestClient restClient;
        public RESTAPItestcase()
        {
            // intentiate RestClient
            restClient = new RestClient(new RestClientOptions
            {
                BaseUrl = new Uri("https://fakerestapi.azurewebsites.net")
            });
        }

        public static JToken? userId;
        
        [Fact]
        public void Test1PostRequest()
        {        
            //Post resquest : endpoint
            var postRequest = new RestRequest("/api/v1/Users");
            var requestBody = $$"""
                               {
                                 "id": 10,
                                 "userName": "s1",
                                 "password": "p1"
                               }
                           """;
            postRequest.AddStringBody(requestBody, DataFormat.Json);

            var postResponse = restClient.Post(postRequest);

            // store userId to use further in get, put and delete request
            userId = JObject.Parse(postResponse.Content)["id"];
           
            //// if there Bearer token is available in postResponse  // Deserialization The Token

            //var token = JObject.Parse(postResponse.Content)["token"];

            Assert.True(postResponse.StatusCode.ToString().Equals("OK"));

        }
        [Fact]
        public void Test2GetRequest()
        {
            // Perform GET Request
            var getRequest = new RestRequest("/api/v1/Users/" + userId!.ToString());

            // // if getRequest requires Authorization token //1 place the token in the header 

            //getRequest.AddHeader("Authorization", $"Bearer{token}"); 

            // call the get api and get response value
            var getResponse = restClient.Get(getRequest);

            //fluent assertion
            getResponse.Content.Should().NotBeNull();
            getResponse.StatusCode.ToString().Should().Be("OK");
            getResponse.Content.Should().Contain(userId!.ToString());
        }
        [Fact]
        public void Test3PutRequest()
        {
            //Put request

            var putRequest = new RestRequest("/api/v1/Users/" + userId);
            var putRequestBody = $$"""
                               {
                                 "id": 10,
                                 "userName": "update",
                                 "password": "p1"
                               }
                           """;
            putRequest.AddStringBody(putRequestBody, DataFormat.Json);
            var putResponse = restClient.Put(putRequest);
            var username = JObject.Parse(putResponse.Content)["userName"];

            putResponse.Content.Should().NotBeNull();
            putResponse.StatusCode.ToString().Should().Be("OK");
            //  putResponse.Content.Should().Contain((string)username.Value(),"update");

        }

        [Fact]
        public void Test4DeletRequest()
        {
            // DeleteRequest
            var deleteRequest = new RestRequest("api/v1/Users/" + userId);
            var deleteResponse = restClient.Delete(deleteRequest);

            deleteResponse.StatusCode.ToString().Should().Be("OK");

        }
 }
}