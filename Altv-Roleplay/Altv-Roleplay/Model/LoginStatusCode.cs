using System;
using System.Collections.Generic;
using System.Text;

namespace Altv_Roleplay.Model
{
    public enum LoginStatusCode
    {
        Error = 0,
        KeyWrong = 1,
        DataMissing = 2,
        Success = 10,
        WrongPasswordUsername = 11
    }
}
