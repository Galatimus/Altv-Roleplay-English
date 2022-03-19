using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Altv_Roleplay.models
{
    public partial class Server_Banks
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public float posX { get; set; }
        public float posY { get; set; }
        public float posZ { get; set; }
        public string zoneName { get; set; }
    }
}