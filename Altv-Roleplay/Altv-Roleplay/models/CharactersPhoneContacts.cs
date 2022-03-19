using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Altv_Roleplay.models
{
    public partial class CharactersPhoneContacts
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int contactId { get; set; }

        public int phoneNumber { get; set; }
        public string contactName { get; set; }
        public int contactNumber { get; set; }
    }
}