using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Altv_Roleplay.models
{
    public partial class Server_Clothes_Shops_Items
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int componentId { get; set; }
        public int drawableId { get; set; }
        public int textureId { get; set; }
        public int gender { get; set; }
        public int isProp { get; set; }

    }
}