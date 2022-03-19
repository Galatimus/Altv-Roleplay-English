using System;
using System.Collections.Generic;
using System.Text;

namespace Altv_Roleplay.Model
{
    class LoginResponse
    {
        public LoginStatusCode StatusCode { get; set; }
        public LoginUserData UserData { get; set; }

        public LoginResponse(LoginStatusCode statusCode, LoginUserData userData)
        {
            StatusCode = statusCode;
            UserData = userData;
        }
    }
}
