using System.Net;
using Task9.Clients;
using Task9.Models.Requests;

namespace Task9
{
    [TestFixture]
    public class WalletTests
    {
        private readonly UserServiceClient _userService = new UserServiceClient();
        private readonly WalletServiceClient _walletService = new WalletServiceClient();
       
        //2,3,
        [TestCase(false)]        
        [TestCase(true)]
        public async Task GetBalance_NotActiveUser_StatusResponse500(bool enableDelete) 
        {
            //Precondition
           
            UserServiceRegisterUserRequest requestBody = new UserServiceRegisterUserRequest();
            int id = await requestBody.GenerateUserId();
          
            if (enableDelete)
                await _userService.DeleteUser(id);

            //Action

            HttpResponseMessage response = await _walletService.GetBalance(id);

            //Assert

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));

        }
        //1,15
        [Test]
        public async Task GetBalance_ActiveUser_StatusResponseOK()
        {
            //Precondition
            
            UserServiceRegisterUserRequest requestBody = new UserServiceRegisterUserRequest();
            int id = await requestBody.GenerateUserId();
            await _userService.SetUserStatus(id, true);

            //Action

            HttpResponseMessage response = await _walletService.GetBalance(id);

            //Assert

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        }




        [TestCase(0.01)]
        [TestCase(0.01, true)]
        [TestCase(9999999.99)]
        [TestCase(10000000)]
        public async Task GetBalance_OneTrasaction_StatusResponseExpected(double balance,bool negativeBalance = false)
        {

            UserServiceRegisterUserRequest requestBody = new UserServiceRegisterUserRequest();
            int id = await requestBody.GenerateUserId();
            await _userService.SetUserStatus(id, true);

            WalletServiceChargeRequest requestChargeBody = new WalletServiceChargeRequest();
            requestChargeBody.SetBody(id, balance);

            await (negativeBalance ? requestChargeBody.SetNegativeBalance() : _walletService.Charge(requestChargeBody));
            

            //Action 

            HttpResponseMessage response = await _walletService.GetBalance(id);
            Console.WriteLine(await response.Content.ReadAsStringAsync());

            //Assert

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        }







        //43,44
        [TestCase(false)]
        [TestCase(true)]
        public async Task Charge_NotActiveExistentUser_StatusResponse500(bool enableDelete)
        {

            //Precondition            
            UserServiceRegisterUserRequest requestBody = new UserServiceRegisterUserRequest();
            int id = await requestBody.GenerateUserId();


            //Action
            if (enableDelete)
                await _userService.DeleteUser(id);


            WalletServiceChargeRequest requestChargeBody = new WalletServiceChargeRequest();
            requestChargeBody.SetBody(id, 100000000);
            HttpResponseMessage response = await _walletService.Charge(requestChargeBody);

            //Assert

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));

        }

        //46,47,48,49,50,55
        [TestCase(-30, HttpStatusCode.InternalServerError)]
        [TestCase(10000000.01, HttpStatusCode.InternalServerError)]
        [TestCase(-0.01, HttpStatusCode.InternalServerError)]
        [TestCase(0.001, HttpStatusCode.InternalServerError)]
        [TestCase(0.01, HttpStatusCode.OK)]
        [TestCase(-0.01, HttpStatusCode.OK, true)]
        [TestCase(0, HttpStatusCode.InternalServerError, true)]
        [TestCase(0, HttpStatusCode.InternalServerError, false)]

        public async Task Charge_LessThanBalance_StatusResponseExpected(double amount, HttpStatusCode statusCode, bool addToDefaultBalance = false)
        {

            //Precondition
           
            UserServiceRegisterUserRequest requestBody = new UserServiceRegisterUserRequest();
            int id = await requestBody.GenerateUserId();
            await _userService.SetUserStatus(id, true);

            WalletServiceChargeRequest requestChargeBody = new WalletServiceChargeRequest();

            if (addToDefaultBalance) {                
                requestChargeBody.SetBody(id, Math.Round(new Random().NextDouble() * (10000000 - 0.01) + 0.01, 2));
                await _walletService.Charge(requestChargeBody);
               
            }
                     
            requestChargeBody.SetBody(id, amount);            

            HttpResponseMessage response = await _walletService.Charge(requestChargeBody);
            
            //Assert            
            Assert.That(response.StatusCode, Is.EqualTo(statusCode));
           
            

        }

        //51,52
        [TestCase(10001000,HttpStatusCode.OK)]
        [TestCase(10001000.01, HttpStatusCode.InternalServerError)]
       public async Task Charge_NegativeBalance_StatusResponseExpected(double charge, HttpStatusCode statusCode) 
        {
            //Precondition

            UserServiceRegisterUserRequest requestBody = new UserServiceRegisterUserRequest();
            int id = await requestBody.GenerateUserId();
            await _userService.SetUserStatus(id, true);

            WalletServiceChargeRequest requestChargeBody = new WalletServiceChargeRequest();
            requestChargeBody.SetBody(id, 1000);            
            await requestChargeBody.SetNegativeBalance();

            requestChargeBody.SetBody(id, charge);
            
            //Action 

            HttpResponseMessage response = await _walletService.Charge(requestChargeBody);
            
            //Assert

            Assert.That(response.StatusCode,Is.EqualTo(statusCode));

       }
        //45, 53, 54, 56
        [Test,TestCaseSource(nameof(CreateRandomNumbers))]
        public async Task Charge_N_StatusResponseExpected(double balance, double charge, HttpStatusCode statusCode, bool negativeBalance)
        {

            UserServiceRegisterUserRequest requestBody = new UserServiceRegisterUserRequest();
            int id = await requestBody.GenerateUserId();
            await _userService.SetUserStatus(id, true);

            WalletServiceChargeRequest requestChargeBody = new WalletServiceChargeRequest();
            requestChargeBody.SetBody(id, balance);

            await (negativeBalance ? requestChargeBody.SetNegativeBalance() : _walletService.Charge(requestChargeBody));
            requestChargeBody.SetBody(id, charge);

            //Action 

            HttpResponseMessage response = await _walletService.Charge(requestChargeBody);
            
            //Assert

            Assert.That(response.StatusCode, Is.EqualTo(statusCode));

        }


        private static TestCaseData[] CreateRandomNumbers()
        {
            double n = CreateRandomNumber();
            return new TestCaseData[] {

            new TestCaseData( n, -n - 0.01, HttpStatusCode.InternalServerError, false).SetName("45"),
            new TestCaseData ( n, -n, HttpStatusCode.OK, false).SetName("53"),
            new TestCaseData ( n, -n, HttpStatusCode.InternalServerError, true ).SetName("54"),
            new TestCaseData ( n + 10, -n, HttpStatusCode.OK, false ).SetName("56"),
            };
        }

        private static double CreateRandomNumber()
        {

            return Math.Round(new Random().NextDouble() * (10000000 - 0.01) + 0.01, 2);

        }


    }
}
