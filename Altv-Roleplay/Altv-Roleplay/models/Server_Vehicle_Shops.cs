using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Altv_Roleplay.models
{
    public partial class Server_Vehicle_Shops
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public string name { get; set; }
        public float pedX { get; set; }
        public float pedY { get; set; }
        public float pedZ { get; set; }
        public float pedRot { get; set; }
        public float parkOutX { get; set; }
        public float parkOutY { get; set; }
        public float parkOutZ { get; set; }
        public float parkOutRotX { get; set; }
        public float parkOutRotY { get; set; }
        public float parkOutRotZ { get; set; }
        public string neededLicense { get; set; }
        public float sellX { get; set; }
        public float sellY { get; set; }
        public float sellZ { get; set; }
    }
}