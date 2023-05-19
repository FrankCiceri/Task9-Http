using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Task9.Models.Requests;

namespace Task9.Clients
{
    public class UserServiceClient
    {
        private readonly HttpClient _client = new HttpClient();
        private readonly string _baseUrl = "https://userservice-uat.azurewebsites.net";

        public async Task<HttpResponseMessage> RegisterUser(UserServiceRegisterUserRequest requestBody) 
        {
            var request = new HttpRequestMessage
            {   
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{_baseUrl}/Register/RegisterNewUser"),
                Content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json"),
            };

            HttpResponseMessage response = await _client.SendAsync(request);

            return response;
        }

        public async Task<HttpStatusCode> DeleteUser(int userId) {

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{_baseUrl}/Register/DeleteUser"),
                Content = new StringContent(JsonConvert.SerializeObject(userId), Encoding.UTF8, "application/json"),
            };

            HttpResponseMessage response = await _client.SendAsync(request);

            return response.StatusCode;

        }
    }
}
