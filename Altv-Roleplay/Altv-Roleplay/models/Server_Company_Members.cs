using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Altv_Roleplay.models
{
    public partial class Server_Company_Members
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public int companyId { get; set; }
        public int charId { get; set; }
        public int rank { get; set; } //0 = Mitglied, 1 = Co Chef, 2 = Chef
        public DateTime invitedTimestamp { get; set; }
    }
}