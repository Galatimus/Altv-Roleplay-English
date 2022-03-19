using AltV.Net.Elements.Entities;
using System;
using AltV.Net;
using System.Collections.Generic;
using System.Text;

namespace Altv_Roleplay.Factories
{
    public class ClassicPlayer : Player
    {
        public int accountId { get; set; } = 0;
        public int CharacterId { get; set; } = 0;
        public string FarmingAction { get; set; } = "None";
        public bool IsUsingCrowbar { get; set; } = false;
        public bool IsUnconscious { get; set; } = false;
        public string CurrentMinijob { get; set; } = "None";
        public string CurrentMinijobStep { get; set; } = "None";
        public int CurrentMinijobActionCount { get; set; } = 0;
        public int CurrentMinijobRouteId { get; set; } = 0;
        public bool isAduty { get; set; } = false;
        public bool HasPDClothesOn { get; set; } = false;
        public bool HasMedicClothesOn { get; set; } = false;
        public bool HasMechanicClothesOn { get; set; } = false;
        public bool HasMaskOn { get; set; } = false;
        public bool HasHatOn { get; set; } = false;
        public bool HasGlassesOn { get; set; } = false;
        public bool HasShirtOn { get; set; } = false;
        public bool HasUndershirtOn { get; set; } = false;
        public bool HasPantsOn { get; set; } = false;
        public bool HasShoesOn { get; set; } = false;
        public bool HasNecklaceOn { get; set; } = false;
        public bool isRobbingAShop { get; set; } = false;
        public bool Gummigeschoss { get; set; } = false;


        public ClassicPlayer(IServer server, IntPtr nativePointer, ushort id) : base(server, nativePointer, id)
        {
        }
    }
}
