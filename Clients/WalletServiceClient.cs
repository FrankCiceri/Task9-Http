using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Task9.Models.Requests;

namespace Task9.Clients
{
    internal class WalletServiceClient
    {

        private readonly HttpClient _client = new HttpClient();
        private readonly string _baseUrl = "https://walletservice-uat.azurewebsites.net";

        public async Task<HttpResponseMessage> GetBalance(int id)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{_baseUrl}/Balance/GetBalance?userId={id}"),
                
            };

            HttpResponseMessage response = await _client.SendAsync(request);

            return response;
        }


        public async Task<HttpResponseMessage> Charge(WalletServiceChargeRequest requestBody)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{_baseUrl}/Balance/Charge"),
                Content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json"),
            };

            HttpResponseMessage response = await _client.SendAsync(request);

            return response;
        }


        public async Task<HttpResponseMessage> Revert(Guid transactionId)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri($"{_baseUrl}/Balance/RevertTransaction?transactionId={transactionId}"),                
            };
            Console.WriteLine($"{_baseUrl}/Balance/RevertTransaction?transactionId={transactionId}");
            HttpResponseMessage response = await _client.SendAsync(request);

            return response;
        }
    }
}
