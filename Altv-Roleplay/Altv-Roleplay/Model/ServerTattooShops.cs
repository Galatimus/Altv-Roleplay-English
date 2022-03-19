using AltV.Net;
using Altv_Roleplay.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altv_Roleplay.Model
{
    class ServerTattooShops
    {
        public static List<Server_Tattoo_Shops> ServerTattooShops_ = new List<Server_Tattoo_Shops>();

        public static void SetTattooShopBankMoney(int shopId, int money)
        {
            try
            {
                var tattooShop = ServerTattooShops_.FirstOrDefault(x => x.id == shopId);
                if (tattooShop == null) return;
                tattooShop.bank = money;

                using (var db = new gtaContext())
                {
                    db.Server_Tattoo_Shops.Update(tattooShop);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e}");
            }
        }

        public static int GetTattooShopBank(int id)
        {
            try
            {
                var tattooShop = ServerTattooShops_.ToList().FirstOrDefault(x => x.id == id);
                if (tattooShop != null) return tattooShop.bank;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return 0;
        }

        public static bool ExistTattooShop(int id)
        {
            try
            {
                return ServerTattooShops_.ToList().Exists(x => x.id == id);
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return false;
        }
    }
}
