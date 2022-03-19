using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Altv_Roleplay.models
{
    public partial class Server_Faction_Ranks
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public int factionId { get; set; }
        public int rankId { get; set; }
        public string name { get; set; }
        public int paycheck { get; set; }
    }
}