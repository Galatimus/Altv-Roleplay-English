using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Altv_Roleplay.models
{
    public partial class Server_Shops
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int shopId { get; set; }

        public string name { get; set; }
        public float posX { get; set; }
        public float posY { get; set; }
        public float posZ { get; set; }
        public float pedX { get; set; }
        public float pedY { get; set; }
        public float pedZ { get; set; }
        public float pedRot { get; set; }
        public string pedModel { get; set; }
        public string neededLicense { get; set; }
        public bool isOnlySelling { get; set; }
        public bool isBlipVisible { get; set; }
        public int faction { get; set; }

        [NotMapped]
        public bool isRobbedNow { get; set; } = false;
    }
}