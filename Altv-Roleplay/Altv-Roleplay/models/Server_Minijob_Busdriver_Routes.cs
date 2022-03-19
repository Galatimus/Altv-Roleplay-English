using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Altv_Roleplay.models
{
    public partial class Server_Minijob_Busdriver_Routes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public int routeId { get; set; }
        public string routeName { get; set; }
        public ulong hash { get; set; }
        public int neededExp { get; set; }
        public int givenExp { get; set; }
        public int paycheck { get; set; }
        public string neededTime { get; set; }
    }
}