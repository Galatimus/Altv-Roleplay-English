using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Altv_Roleplay.models
{
    public partial class Server_Shops_Items
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public int shopId { get; set; }
        public string itemName { get; set; }
        public int itemAmount { get; set; }
        public int itemPrice { get; set; }
        public int itemGender { get; set; }
    }
}