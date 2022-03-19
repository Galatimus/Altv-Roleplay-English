using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Altv_Roleplay.models
{
    public partial class Server_Tablet_Notes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int charId { get; set; }
        public string color { get; set; }
        public string title { get; set; }
        public string text { get; set; }
        public DateTime created { get; set; }
    }
}