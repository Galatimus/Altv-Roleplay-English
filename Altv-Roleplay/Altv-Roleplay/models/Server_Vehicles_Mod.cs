using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Altv_Roleplay.models
{
    public partial class Server_Vehicles_Mod
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public int vehId { get; set; }
        public int colorPrimaryType { get; set; }
        public int colorPrimary_r { get; set; }
        public int colorPrimary_g { get; set; }
        public int colorPrimary_b { get; set; }
        public int colorSecondaryType { get; set; }
        public int colorSecondary_r { get; set; }
        public int colorSecondary_g { get; set; }
        public int colorSecondary_b { get; set; }
        public int colorPearl { get; set; }
        public int headlightColor { get; set; }
        public int spoiler { get; set; }
        public int front_bumper { get; set; }
        public int rear_bumper { get; set; }
        public int side_skirt { get; set; }
        public int exhaust { get; set; }
        public int frame { get; set; }
        public int grille { get; set; }
        public int hood { get; set; }
        public int fender { get; set; }
        public int right_fender { get; set; }
        public int roof { get; set; }
        public int engine { get; set; }
        public int brakes { get; set; }
        public int transmission { get; set; }
        public int horns { get; set; }
        public int suspension { get; set; }
        public int armor { get; set; }
        public int turbo { get; set; }
        public int xenon { get; set; }
        public int wheel_type { get; set; }
        public int wheels { get; set; }
        public int back_wheels { get; set; }
        public int wheelcolor { get; set; }
        public int plate_holder { get; set; }
        public int plate_vanity { get; set; }
        public int trim_design { get; set; }
        public int ornaments { get; set; }
        public int dial_design { get; set; }
        public int door_interior { get; set; }
        public int seats { get; set; }
        public int steering_wheel { get; set; }
        public int shift_lever { get; set; }
        public int plaques { get; set; }
        public int hydraulics { get; set; }
        public int rear_shelf { get; set; }
        public int trunk { get; set; }
        public int engine_block { get; set; }
        public int airfilter { get; set; }
        public int strut_bar { get; set; }
        public int arch_cover { get; set; }
        public int antenna { get; set; }
        public int exterior_parts { get; set; }
        public int tank { get; set; }
        public int door { get; set; }
        public int window_tint { get; set; }
        public int rear_hydraulics { get; set; }
        public int livery { get; set; }
        public int plate_color { get; set; }
        public int plate { get; set; }
        public int interior_color { get; set; }
        public int neon { get; set; }
        public int neon_r { get; set; }
        public int neon_g { get; set; }
        public int neon_b { get; set; }
        public int smoke_r { get; set; }
        public int smoke_g { get; set; }
        public int smoke_b { get; set; }
        public int smoke { get; set; }

    }
}
