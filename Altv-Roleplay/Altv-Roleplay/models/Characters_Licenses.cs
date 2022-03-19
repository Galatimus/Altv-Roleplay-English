using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Altv_Roleplay.models
{
    public partial class Characters_Licenses
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public int charId { get; set; }
        public bool PKW { get; set; }
        public bool LKW { get; set; }
        public bool Bike { get; set; }
        public bool Boat { get; set; }
        public bool Fly { get; set; }
        public bool Helicopter { get; set; }
        public bool PassengerTransport { get; set; }
        public bool weaponlicense { get; set; }
    }
}