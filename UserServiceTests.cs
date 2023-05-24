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

        
        //2      
        [TestCase(null, null)]
        [TestCase("", null)]
        public async Task RegisterUser_Null_StatusCodeInternalServerError(string firstName, string lastName)
        {
            //Precondition            

            UserServiceRegisterUserRequest requestBody = new UserServiceRegisterUserRequest();
            requestBody.SetBody(firstName, lastName);

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
            int id1 = await requestBody.GenerateUserId();

            if (enableDelete)
                await _userService.DeleteUser(id1);

            int id2 = await requestBody.GenerateUserId(); 

            //Assert

            Assert.That(id2 - id1, Is.EqualTo(1));
        }

        //11,20
        [Test]
        public async Task DeleteGetUser_NotActive_StatusCodeOK() {
            UserServiceRegisterUserRequest requestBody = new UserServiceRegisterUserRequest();
            int id = await requestBody.GenerateUserId();

            string getStatus = await _userService.GetUserStatus(id);

            var statusCode = await _userService.DeleteUser(id);

            Assert.Multiple(() => {
                Assert.That(getStatus, Is.EqualTo("false"));
                Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK));
            });


        }
        //10,14,21
        [Test]
        public async Task DeleteGetSetUser_NotExisting_StatusCodeInternalServerError()
        {
            UserServiceRegisterUserRequest requestBody = new UserServiceRegisterUserRequest();
            int id = await requestBody.GenerateUserId();
            await _userService.DeleteUser(id);

            string getStatus = await _userService.GetUserStatus(id);
            var setStatusCode = await _userService.SetUserStatus(id, true);
            var deleteStatusCode = await _userService.DeleteUser(id);


            Assert.Multiple(() => {
                Assert.That(getStatus, Is.EqualTo("Sequence contains no elements"));
                Assert.That(setStatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
                Assert.That(deleteStatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
            });

        }
        //12,13,15,16,17,18,19
        [TestCase(new [] {false, true})]
        [TestCase(new[] { true, false, true, true})]
        public async Task SetGetUser_StatusChange_StatusCodeInternalServerError(bool[] changes)
        {
            //Pre-Condition
            UserServiceRegisterUserRequest requestBody = new UserServiceRegisterUserRequest();
            int id = await requestBody.GenerateUserId();

            //Action

            var setStatusCode = await _userService.SetUserStatus(id, false);

            foreach ( var change in changes){

                setStatusCode = await _userService.SetUserStatus(id, change);

            }

            string getStatus = await _userService.GetUserStatus(id);
            

            //Assert

            Assert.Multiple(() => {
                Assert.That(getStatus, Is.EqualTo($"{changes.Last()}".ToLower()));
                Assert.That(setStatusCode, Is.EqualTo(HttpStatusCode.OK));
            });

        }

    }
}