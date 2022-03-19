using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Altv_Roleplay.models
{
    public partial class Server_Animations
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int animId { get; set; }

        public string displayName { get; set; }
        public string animDict { get; set; }
        public string animName { get; set; }
        public int animFlag { get; set; }
        public int duration { get; set; }
    }
}