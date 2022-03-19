using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Altv_Roleplay.models
{
    public partial class Server_Vehicle_Items
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public int vehId { get; set; }
        public string itemName { get; set; }
        public int itemAmount { get; set; }
        public bool isInGlovebox { get; set; }
    }
}