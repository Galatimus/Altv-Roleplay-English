using Altv_Roleplay.models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using AltV.Net;
using Altv_Roleplay.Factories;

namespace Altv_Roleplay.Model
{
    class ServerClothesShops
    {
        public static List<Server_Clothes_Shops> ServerClothesShops_ = new List<Server_Clothes_Shops>();
        public static List<Server_Clothes_Shops_Items> ServerClothesShopsItems_ = new List<Server_Clothes_Shops_Items>();

        public static bool ExistClothesShop(int shopId)
        {
            try
            {
                return ServerClothesShops_.ToList().Exists(x => x.id == shopId);
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return false;
        }

        public static int GetClothesPrice(ClassicPlayer player, int clothId, bool isProp)
        {
            try
            {
                int price = 100;

                if (!isProp)
                {
                    switch (ServerClothes.GetClothesComponent(clothId, Convert.ToInt32(Characters.GetCharacterGender(player.CharacterId))))
                    {
                        case 1:
                            price = 100;
                            break;
                        case 3:
                            price = 0;
                            break;
                        case 4:
                            price = 150;
                            break;
                        case 5:
                            price = 120;
                            break;
                        case 6:
                            price = 90;
                            break;
                        case 7:
                            price = 80;
                            break;
                        case 8:
                            price = 75;
                            break;
                        case 9:
                            price = 200;
                            break;
                        case 10:
                            price = 40;
                            break;
                        case 11:
                            price = 140;
                            break;
                        default: break;
                    }

                    return price;
                }
                else
                {
                    switch (ServerClothes.GetClothesComponent(clothId, Convert.ToInt32(Characters.GetCharacterGender(player.CharacterId))))
                    {
                        case 0:
                            price = 95;
                            break;
                        case 1:
                            price = 45;
                            break;
                        case 2:
                            price = 55;
                            break;
                        case 6:
                            price = 160;
                            break;
                        case 7:
                            price = 85;
                            break;
                        default: break;
                    }

                    return price;
                }

            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return 0;
        }
    }
}
