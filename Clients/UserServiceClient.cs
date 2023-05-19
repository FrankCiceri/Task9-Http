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

        public async Task<string> GetUserStatus(int userId)
        {

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{_baseUrl}/UserManagement/GetUserStatus?userId={userId}"),
            };

            HttpResponseMessage response = await _client.SendAsync(request);

            return await response.Content.ReadAsStringAsync();

        }


        public async Task<HttpStatusCode> SetUserStatus(int userId, bool newStatus)
        {

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri($"{_baseUrl}/UserManagement/SetUserStatus?userId={userId}&newStatus={newStatus}"),
            };

            HttpResponseMessage response = await _client.SendAsync(request);

            return response.StatusCode;

        }



        public async Task<HttpStatusCode> DeleteUser(int userId) {

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri($"{_baseUrl}/Register/DeleteUser?userId={userId}"),
            };

            HttpResponseMessage response = await _client.SendAsync(request);

            return response.StatusCode;

        }
    }
}
