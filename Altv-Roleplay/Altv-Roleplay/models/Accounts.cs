using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Altv_Roleplay.models
{
    public partial class Accounts
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int playerid { get; set; }
        public string playerName { get; set; }
        public string Email { get; set; }
        public ulong socialClub { get; set; }
        public ulong hardwareId { get; set; }
        public string password { get; set; }
        public int Online { get; set; } //CharakterID mit welchem der Spieler eingeloggt ist - 0 = offline.
        public bool whitelisted { get; set; }
        public bool ban { get; set; }
        public string banReason { get; set; }
        public int adminLevel { get; set; }

    }
}
