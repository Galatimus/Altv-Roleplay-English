using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Altv_Roleplay.models
{
    public partial class Server_Houses_Interiors
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int interiorId { get; set; }

        public float exitX { get; set; }
        public float exitY { get; set; }
        public float exitZ { get; set; }
        public float storageX { get; set; }
        public float storageY { get; set; }
        public float storageZ { get; set; }
        public float storageLimit { get; set; }
    
        public float manageX { get; set; }
        public float manageY { get; set; }
        public float manageZ { get; set; }
    }
}