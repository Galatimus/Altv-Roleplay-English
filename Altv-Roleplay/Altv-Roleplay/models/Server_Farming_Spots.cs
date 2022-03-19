using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Altv_Roleplay.models
{
    public partial class Server_Farming_Spots
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public float posX { get; set; }
        public float posY { get; set; }
        public float posZ { get; set; }
        public string itemName { get; set; }
        public string animation { get; set; }
        public string neededItemToFarm { get; set; }
        public int itemMinAmount { get; set; }
        public int itemMaxAmount { get; set; }
        public int markerColorR { get; set; }
        public int markerColorG { get; set; }
        public int markerColorB { get; set; }
        public int blipColor { get; set; }
        public float range { get; set; }
        public int duration { get; set; }
        public bool isBlipVisible { get; set; }
    }
}