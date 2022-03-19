using AltV.Net.Async;
using Altv_Roleplay.Factories;
using Altv_Roleplay.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altv_Roleplay.Model
{
    class ServerTattoos
    {
        public static List<Server_Tattoos> ServerTattoos_ = new List<Server_Tattoos>();

        public static async void GetAllTattoos(ClassicPlayer player)
        {
            try
            {
                if (player == null || !player.Exists || player.CharacterId <= 0) return;
                int gender = Convert.ToInt32(Characters.GetCharacterGender(player.CharacterId));
                var tattooItems = ServerTattoos_.ToList().Where(x => x.gender == gender).Select(x => new
                {
                    x.id,
                    x.name,
                    x.nameHash,
                    x.part,
                    x.price,
                    x.collection,
                }).OrderBy(x => x.name).ToList();

                var itemCount = (int)tattooItems.Count;
                var iterations = Math.Floor((decimal)(itemCount / 30));
                var rest = itemCount % 30;
                for (var i = 0; i < iterations; i++)
                {
                    var skip = i * 30;
                    player.EmitAsync("Client:TattooShop:sendItemsToClient", System.Text.Json.JsonSerializer.Serialize(tattooItems.Skip(skip).Take(30).ToList()));
                }
                if (rest != 0) player.EmitAsync("Client:TattooShop:sendItemsToClient", System.Text.Json.JsonSerializer.Serialize(tattooItems.Skip((int)iterations * 30).ToList()));
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e}");
            }
        }

        public static bool ExistTattoo(int tattooId)
        {
            try
            {
                return ServerTattoos_.ToList().Exists(x => x.id == tattooId);
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e}");
            }
            return false;
        }

        public static string GetTattooCollection(int tattooId)
        {
            try
            {
                var tattoo = ServerTattoos_.ToList().FirstOrDefault(x => x.id == tattooId);
                if (tattoo != null) return tattoo.collection;
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e}");
            }
            return "";
        }

        public static string GetTattooNameHash(int tattooId)
        {
            try
            {
                var tattoo = ServerTattoos_.ToList().FirstOrDefault(x => x.id == tattooId);
                if (tattoo != null) return tattoo.nameHash;
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e}");
            }
            return "";
        }

        public static string GetTattooName(int tattooId)
        {
            try
            {
                var tattoo = ServerTattoos_.ToList().FirstOrDefault(x => x.id == tattooId);
                if (tattoo != null) return tattoo.name;
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e}");
            }
            return "";
        }

        public static int GetTattooPrice(int tattooId)
        {
            try
            {
                var tattoo = ServerTattoos_.ToList().FirstOrDefault(x => x.id == tattooId);
                if (tattoo != null) return tattoo.price;
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e}");
            }
            return 0;
        }
    }
}
