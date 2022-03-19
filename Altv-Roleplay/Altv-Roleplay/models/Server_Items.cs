using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Altv_Roleplay.models
{
    public partial class Server_Items
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public string itemName { get; set; }
        public string itemType { get; set; }
        public string itemDescription { get; set; }
        public float itemWeight { get; set; }
        public bool isItemDesire { get; set; }
        public int itemDesireFood { get; set; }
        public int itemDesireDrink { get; set; }
        public bool hasItemAnimation { get; set; }
        public string itemAnimationName { get; set; }
        public bool isItemDroppable { get; set; }
        public bool isItemUseable { get; set; }
        public bool isItemGiveable { get; set; }
        public bool isItemClothes { get; set; }
        public string ClothesType { get; set; }
        public int ClothesDraw { get; set; }
        public int ClothesTexture { get; set; }
        public int ClothesUndershirt { get; set; }
        public int ClothesUndershirtTexture { get; set; }
        public int ClothesDecals { get; set; }
        public int ClothesDecalsTexture { get; set; }
        public string itemPicSRC { get; set; }
    }
}