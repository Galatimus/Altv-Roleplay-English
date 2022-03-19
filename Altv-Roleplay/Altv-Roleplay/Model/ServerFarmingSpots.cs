using AltV.Net.Elements.Entities;
using Altv_Roleplay.models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Altv_Roleplay.Model
{
    class ServerFarmingSpots
    {
        public static List<Server_Farming_Spots> ServerFarmingSpots_ = new List<Server_Farming_Spots>();
        public static List<Server_Farming_Producer> ServerFarmingProducer_ = new List<Server_Farming_Producer>();
        public static List<IColShape> ServerFarmingSpotsColshapes_ = new List<IColShape>();
        public static List<IColShape> ServerFarmingProducerColshapes_ = new List<IColShape>();
    }
}
