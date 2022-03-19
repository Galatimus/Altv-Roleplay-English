using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Altv_Roleplay.models
{
    public partial class Server_Markers
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public int type { get; set; }
        public float posX { get; set; }
        public float posY { get; set; }
        public float posZ { get; set; }
        public float scaleX { get; set; }
        public float scaleY { get; set; }
        public float scaleZ { get; set; }
        public int red { get; set; }
        public int green { get; set; }
        public int blue { get; set; }
        public int alpha { get; set; }
        public bool bobUpAndDown { get; set; }
    }
}