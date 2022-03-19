using AltV.Net.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Altv_Roleplay.models
{
    public partial class Server_Factions
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public string factionName { get; set; }
        public string factionShort { get; set; }
        public int factionMoney { get; set; }

        public int phoneNumber { get; set; }
        public Position laborPos { get; set; } // Labor-System only for Factions with Type 2 or 3.

        [NotMapped]
        public bool isLaborLocked { get; set; } = true;

        [NotMapped]
        public int currentPhoneOwnerId { get; set; } = 0;
    }
}