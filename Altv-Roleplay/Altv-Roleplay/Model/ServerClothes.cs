using AltV.Net;
using Altv_Roleplay.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altv_Roleplay.Model
{
    class ServerClothes
    {

        public static int GetClothesGender(int clothesId)
        {
            try
            {
                var clothes = ServerClothesShops.ServerClothesShopsItems_.ToList().FirstOrDefault(x => x.id == clothesId);
                if (clothes != null) return clothes.gender;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return 0;
        }

        public static int GetClothesComponent(int clothesId, int gender)
        {
            try
            {
                var clothes = ServerClothesShops.ServerClothesShopsItems_.ToList().FirstOrDefault(x => x.id == clothesId && x.gender == gender);
                if (clothes != null) return clothes.componentId;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return 0;
        }

        public static int GetClothesDraw(int clothesId, int gender)
        {
            try
            {
                var clothes = ServerClothesShops.ServerClothesShopsItems_.ToList().FirstOrDefault(x => x.id == clothesId && x.gender == gender);
                if (clothes != null) return clothes.drawableId;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return 0;
        }

        public static int GetClothesTexture(int clothesId, int gender)
        {
            try
            {
                var clothes = ServerClothesShops.ServerClothesShopsItems_.ToList().FirstOrDefault(x => x.id == clothesId && x.gender == gender);
                if (clothes != null) return clothes.textureId;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return 0;
        }

        public static int GetClothesId(int ComponentId, int DrawableId, int TextureId, int gender)
        {
            try
            {
                var clothes = ServerClothesShops.ServerClothesShopsItems_.ToList().FirstOrDefault(x => x.componentId == ComponentId && x.drawableId == DrawableId && x.textureId == TextureId && x.gender == gender);
                if (clothes != null) return clothes.id;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return 0;
        }

        public static bool ExistClothes(int clothId, int gender)
        {
            try
            {
                return ServerClothesShops.ServerClothesShopsItems_.ToList().Exists(x => x.id == clothId && x.gender == gender);
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return false;
        }
    }
}
