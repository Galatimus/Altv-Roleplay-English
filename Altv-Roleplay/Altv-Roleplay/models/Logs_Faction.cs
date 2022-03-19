using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Altv_Roleplay.models
{
    public partial class Logs_Faction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public int factionId { get; set; }
        public int charId { get; set; }
        public int targetCharId { get; set; }
        public string type { get; set; }
        public string text { get; set; }
        public DateTime timestamp { get; set; }
    }
}