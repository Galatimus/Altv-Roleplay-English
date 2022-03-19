using AltV.Net.Elements.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Altv_Roleplay.models
{
    public partial class Server_Houses
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public int interiorId { get; set; }
        public int ownerId { get; set; }
        public string street { get; set; }
        public int price { get; set; }
        public int maxRenters { get; set; }
        public int rentPrice { get; set; }
        public bool isRentable { get; set; }
        public bool hasStorage { get; set; }
        public bool hasAlarm { get; set; }
        public bool hasBank { get; set; }
        public float entranceX { get; set; }
        public float entranceY { get; set; }
        public float entranceZ { get; set; }
        public int money { get; set; }

        [NotMapped]
        public bool isLocked { get; set; } = true;

        [NotMapped]
        public IColShape entranceShape { get; set; }
    }
}