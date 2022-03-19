using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Altv_Roleplay.models
{
    public partial class Server_Faction_Members
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public int charId { get; set; }
        public int factionId { get; set; }
        public int rank { get; set; }
        public string rankname { get; set; }
        public int serviceNumber { get; set; }
        public bool isDuty { get; set; }
        public DateTime lastChange { get; set; }
        public int phone { get; set; }
        public string charname { get; set; }
        public string factionname { get; set; }
    }
}