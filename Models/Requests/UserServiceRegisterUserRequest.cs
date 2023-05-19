using Newtonsoft.Json;
using System;
using System.Text;

namespace Task9.Models.Requests
{
    public class UserServiceRegisterUserRequest
    {
        [JsonProperty("firstName")]
        public string FirstName;

        [JsonProperty("lastName")]
        public string LastName;


        private Func<string, int, Random, string> _randomString = (chars, length, random) => new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        private Random _random = new Random();
        private string _chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#^&*-_";


        public void SetBody(int length) {
            FirstName = "FName" + _randomString(_chars, length, _random);
            LastName = "LName" + _randomString(_chars, length, _random);

        }

        public void SetBody(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;

        }
    }
}