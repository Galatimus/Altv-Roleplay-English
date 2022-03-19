using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Altv_Roleplay.models
{
    public partial class CharactersPhoneChats
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int chatId { get; set; }

        public int phoneNumber { get; set; }
        public int anotherNumber { get; set; }
    }
}