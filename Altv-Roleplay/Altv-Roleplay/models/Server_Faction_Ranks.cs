using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Altv_Roleplay.models
{
    public partial class CharactersPhoneVerlauf
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int charId { get; set; }
        public int phoneNumber { get; set; }
        public int anotherNumber { get; set; }
        public DateTime date { get; set; }

    }
}