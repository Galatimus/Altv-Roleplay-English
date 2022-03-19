using AltV.Net;
using Altv_Roleplay.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altv_Roleplay.Model
{
    class CharactersTattoos
    {
        public static List<Characters_Tattoos> CharactersTattoos_ = new List<Characters_Tattoos>();

        public static void RemoveAccountTattoo(int charId, int tatooId)
        {
            try
            {
                var entry = CharactersTattoos_.FirstOrDefault(x => x.charId == charId && x.tattooId == tatooId);
                if (entry == null) return;                    
                CharactersTattoos_.Remove(entry);

                using (var db = new gtaContext())
                {
                    db.Characters_Tattoos.Remove(entry);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static void CreateNewEntry(int charId, int tattooId)
        {
            try
            {
                var entry = new Characters_Tattoos
                {
                    charId = charId,
                    tattooId = tattooId
                };
                CharactersTattoos_.Add(entry);
                using (var db = new gtaContext())
                {
                    db.Characters_Tattoos.Add(entry);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static bool ExistAccountTattoo(int charId, int id)
        {
            try
            {
                return CharactersTattoos_.ToList().Exists(x => x.charId == charId && x.tattooId == id);
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return false;
        }

        public static string GetAccountOwnTattoos(int charId)
        {
            try
            {
                return System.Text.Json.JsonSerializer.Serialize(CharactersTattoos_.ToList().Where(x => x.charId == charId).Select(x => new
                {
                    x.tattooId,
                    name = ServerTattoos.GetTattooName(x.tattooId),
                    price = ServerTattoos.GetTattooPrice(x.tattooId),
                }).ToList());
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e}");
            }
            return "[]";
        }

        public static string GetAccountTattoos(int charId)
        {
            try
            {
                return System.Text.Json.JsonSerializer.Serialize(CharactersTattoos_.ToList().Where(x => x.charId == charId).Select(x => new
                {
                    collection = ServerTattoos.GetTattooCollection(x.tattooId),
                    hash = ServerTattoos.GetTattooNameHash(x.tattooId),
                }).ToList());
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e}");
            }
            return "[]";
        }
    }
}
