using AltV.Net;
using AltV.Net.Elements.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Altv_Roleplay.Factories
{
    public class ClassicColshape : ColShape
    {
        public int ColshapeId { get; set; } = 0;
        public string ColshapeName { get; set; } = "None";
        public string CarDealerVehName { get; set; }
        public ulong CarDealerVehPrice { get; set; }
        public float Radius { get; set; }


        public ClassicColshape(IServer server, IntPtr nativePointer) : base(server, nativePointer)
        {

        }

        public bool IsInRange(ClassicPlayer player)
        {
            lock (player)
            {
                if (!player.Exists) return false;

                return player.Position.Distance(Position) <= Radius;
            }
        }
    }
}