using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Altv_Roleplay.models
{
    public partial class Characters_Tablet_Apps
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public int charId { get; set; }
        public bool weather { get; set; }
        public bool news { get; set; }
        public bool banking { get; set; }
        public bool lifeinvader { get; set; }
        public bool vehicles { get; set; }
        public bool events { get; set; }
        public bool company { get; set; }
        public bool notices { get; set; }
    }
}