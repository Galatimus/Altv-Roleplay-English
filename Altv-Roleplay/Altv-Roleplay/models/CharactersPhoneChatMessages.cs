using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Altv_Roleplay.models
{
    public partial class CharactersPhoneChatMessages
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public int chatId { get; set; }
        public int fromNumber { get; set; }
        public int toNumber { get; set; }
        public int unix { get; set; }
        public string message { get; set; }
    }
}