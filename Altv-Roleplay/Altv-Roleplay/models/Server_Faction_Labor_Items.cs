using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Altv_Roleplay.models
{
    public partial class Server_Faction_Labor_Items
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public int factionId { get; set; }
        public int accountId { get; set; }
        public string itemName { get; set; }
        public int itemAmount { get; set; }
    }
}