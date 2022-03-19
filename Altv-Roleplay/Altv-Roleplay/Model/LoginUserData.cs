using System;
using System.Collections.Generic;
using System.Text;

namespace Altv_Roleplay.Model
{
    class LoginUserData
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public bool Banned { get; set; }
        public string BanReason { get; set; }
        public bool Whitelisted { get; set; }
        public string Email { get; set; }
    }
}
