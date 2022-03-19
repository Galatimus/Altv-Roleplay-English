using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Altv_Roleplay.models
{
    public partial class Server_Tattoos
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string collection { get; set; }
        public string nameHash { get; set; }
        public string name { get; set; }
        public string part { get; set; }
        public int price { get; set; }
        public int gender { get; set; }
    }
}
