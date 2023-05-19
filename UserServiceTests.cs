using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using Task9.Clients;
using Task9.Models.Requests;


namespace Task9
{
    [TestFixture]
    public class UserServiceTests
    {
        private readonly UserServiceClient _userService = new UserServiceClient();

        [SetUp]
        public void Setup()
        { 

        }
        //2      
        [TestCase(null, null)]        
        [TestCase("", null)]
        public async Task RegisterUser_Null_StatusCodeInternalServerError(string firstName, string lastName)
        {
            //Precondition            

            UserServiceRegisterUserRequest requestBody = new UserServiceRegisterUserRequest();
            requestBody.SetBody(firstName,lastName);

            //Action

            HttpResponseMessage response = await _userService.RegisterUser(requestBody);

            //Assert

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));   
        }

        //1,3,4,5,6,7
        [TestCase(0)]//Length 0 means Empty String
        [TestCase(1)]
        [TestCase(100)] 
        [TestCase(500)]
        public async Task RegisterUser_MultipleCharactersVariableLength_StatusCodeOK(int length)
        {
            //Precondition
            UserServiceRegisterUserRequest requestBody = new UserServiceRegisterUserRequest();
            requestBody.SetBody(length);

            //Action

            HttpResponseMessage response = await _userService.RegisterUser(requestBody);

            //Assert

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }
        //8,9
        [TestCase(false)]
        [TestCase(true)]
        public async Task RegisterUser_GetBody_IdIncreases(bool enableDelete)
        {
            //Precondition
            UserServiceRegisterUserRequest requestBody = new UserServiceRegisterUserRequest();
            requestBody.SetBody(10);


            //Action

            HttpResponseMessage response1 = await _userService.RegisterUser(requestBody);
            int id1 = Int32.Parse(await response1.Content.ReadAsStringAsync());

            if (enableDelete)
                await _userService.DeleteUser(id1);

            HttpResponseMessage response2 = await _userService.RegisterUser(requestBody);            
            int id2 = Int32.Parse(await response2.Content.ReadAsStringAsync());

            //Assert
            
            Assert.That(id2 - id1, Is.GreaterThan(0));
        }

        //[test]
        //public async Task DeleteUser_NotActive_StatusCodeOK() {
        
        //}

    }
}