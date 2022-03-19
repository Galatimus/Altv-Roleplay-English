using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Altv_Roleplay.Factories
{
    public class VehicleFactory : IEntityFactory<IVehicle>
    {
        public IVehicle Create(IServer server, uint model, Position position, Rotation rotation)
        {
            return new ClassicVehicle(server, model, position, rotation);
        }

        public IVehicle Create(IServer server, IntPtr nativePointer, ushort id)
        {
            return new ClassicVehicle(server, nativePointer, id);
        }
    }

    public class AccountsFactory : IEntityFactory<IPlayer>
    {
        public IPlayer Create(IServer server, IntPtr playerPointer, ushort id)
        {
            return new ClassicPlayer(server, playerPointer, id);
        }
    }

    public class ColshapeFactory : IBaseObjectFactory<IColShape>
    {
        public IColShape Create(IServer server, IntPtr entityPointer)
        {
            return new ClassicColshape(server, entityPointer);
        }
    }
}
