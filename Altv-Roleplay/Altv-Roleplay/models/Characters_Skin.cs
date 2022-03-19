using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Altv_Roleplay.models
{
    public partial class Characters_Skin
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int charId { get; set; }
        public string facefeatures { get; set; }
        public string headblendsdata { get; set; }
        public string headoverlays { get; set; }
        public int clothesTop { get; set; }
        public int clothesTorso { get; set; }
        public int clothesLeg { get; set; }
        public int clothesFeet { get; set; }
        public int clothesHat { get; set; }
        public int clothesGlass { get; set; }
        public int clothesEarring { get; set; }
        public int clothesNecklace { get; set; }
        public int clothesMask { get; set; }
        public int clothesArmor { get; set; }
        public int clothesUndershirt { get; set; }
        public int clothesBracelet { get; set; }
        public int clothesWatch { get; set; }
        public int clothesBag { get; set; }
        public int clothesDecal { get; set; }

    }
}