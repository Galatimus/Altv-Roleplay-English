using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Altv_Roleplay.models
{
    public partial class Server_Tattoo_Shops
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string name { get; set; }
        public int owner { get; set; }
        public int bank { get; set; }
        public int price { get; set; }
        public float pedX { get; set; }
        public float pedY { get; set; }
        public float pedZ { get; set; }
        public string pedModel { get; set; }
        public float pedRot { get; set; }
    }
}
