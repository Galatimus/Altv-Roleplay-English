using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Altv_Roleplay.models
{
    public partial class Characters_LastPos
    {
        public int charId { get; set; }
        public float lastPosX { get; set; }
        public float lastPosY { get; set; }
        public float lastPosZ { get; set; }
        public int lastDimension { get; set; }
    }
}