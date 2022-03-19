using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Altv_Roleplay.models
{
    public partial class Server_Jobs
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public string jobName { get; set; }
        public int jobPaycheck { get; set; }
        public int jobNeededHours { get; set; }
        public string jobPic { get; set; }
    }
}