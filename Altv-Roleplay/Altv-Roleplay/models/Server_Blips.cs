using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Altv_Roleplay.models
{
    public partial class Server_Blips
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public string name { get; set; }
        public int color { get; set; }
        public float scale { get; set; }
        public bool shortRange { get; set; }
        public int sprite { get; set; }
        public float posX { get; set; }
        public float posY { get; set; }
        public float posZ { get; set; }
    }
}