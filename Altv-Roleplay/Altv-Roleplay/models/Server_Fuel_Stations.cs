using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Altv_Roleplay.models
{
    public partial class Server_Fuel_Stations
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public string name { get; set; }
        public int owner { get; set; } //charId, 0 = Staat
        public string availableFuel { get; set; }
        public int availableLiters { get; set; }
        public int bank { get; set; }
    }
}