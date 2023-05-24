using Newtonsoft.Json;
using Task9.Clients;

namespace Task9.Models.Requests
{
    public class WalletServiceChargeRequest
    {
        private readonly WalletServiceClient _walletService = new WalletServiceClient(); 

        [JsonProperty("userId")]
        public int UserId;

        [JsonProperty("amount")]
        public double Amount;


        public void SetBody(int id , double amount)
        {
            UserId = id;
            Amount = amount;

        }

        public async Task SetNegativeBalance() {
            
            HttpResponseMessage response = await _walletService.Charge(this);
            string transactionId = await response.Content.ReadAsStringAsync();
            this.Amount = -this.Amount;
            
            Guid uuid = Guid.ParseExact(
            transactionId                
            .Replace("\"", ""), "D"); ;

            await _walletService.Charge(this);

            await _walletService.Revert(uuid);
        }

       
    }
}
