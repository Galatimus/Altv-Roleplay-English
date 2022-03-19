using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Altv_Roleplay.models
{
    public partial class LogsLogin
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public string username { get; set; }
        public ulong socialclub { get; set; }
        public string text { get; set; }
        public string address { get; set; }
        public ulong hwid { get; set; }

        public bool success { get; set; }
        public DateTimeOffset timestamp { get; set; }
    }
}
