using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using System;
using AltV.Net;
using System.Collections.Generic;
using System.Text;

namespace Altv_Roleplay.Factories
{
    public class ClassicVehicle : Vehicle
    {
        public int VehicleId { get; set; }
        public bool Trunkstate { get; set; } = false;

        public ClassicVehicle(IServer server, IntPtr nativePointer, ushort id) : base(server, nativePointer, id)
        {
        }

        public ClassicVehicle(IServer server, uint model, Position position, Rotation rotation) : base(server, model, position, rotation)
        {
        }
    }
}
