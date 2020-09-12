using System;
using System.Collections.Generic;
using System.Text;
using Xunit.Gherkin.Quick;
using Xunit;

namespace TestRunner 
{
    
    [FeatureFile("./FeatureFiles/LoginTest.feature")]
    public sealed class LoginTest : Feature
    {
        private string returnmessage;
        private string username;
        private string password;
        private bool result;
        [Given(@"I enter (\w+) as username")]
        public void IEnterUserName(string username)
        {
            this.username = username;
        }

        [And(@"I enter (\w+) as password")]
        public void IEnterPassword(string password)
        {
            this.password = password;
        }

        [When(@"I press login")]
        public void IPressLogin()
        {
            result = TargetApp.LoginController.Login(username,password,out returnmessage);
        }

        [Then(@"I should get login (\w+) with (.*)")]
        public void ResultAssertion(string resultmsg, string message)
        {
            bool success = resultmsg == "success" ? true : false;
            Assert.Equal(result,success);
            Assert.Equal(message, result? $"token {returnmessage}":$"message {returnmessage}");
        }
    }
     


}
