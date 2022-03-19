using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Altv_Roleplay.models
{
    public partial class Server_Clothes_Shops
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public string name { get; set; }
        public float posX { get; set; }
        public float posY { get; set; }
        public float posZ { get; set; }
        public float pedX { get; set; }
        public float pedY { get; set; }
        public float pedZ { get; set; }
        public float pedRot { get; set; }
        public string pedModel { get; set; }
    }
}