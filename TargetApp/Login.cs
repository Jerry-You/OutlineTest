using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TargetApp
{
    public class LoginController
    {
        public static bool Login(string username,string password,out string message)
        {
            if (username == "jerry" && password == "test123")
            {
                message = "qwexxeqer";
                return true;
            }
            message = "invalid password";
            return false;
        }
    }
}
