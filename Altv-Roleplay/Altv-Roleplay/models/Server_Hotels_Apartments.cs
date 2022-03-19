using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Altv_Roleplay.models
{
    public partial class Server_Hotels_Apartments
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public int hotelId { get; set; }
        public int interiorId { get; set; }
        public int ownerId { get; set; }
        public int rentPrice { get; set; }
        public int maxRentHours { get; set; }
        public DateTime lastRent { get; set; }

        [NotMapped]
        public bool isLocked { get; set; } = true;
    }
}