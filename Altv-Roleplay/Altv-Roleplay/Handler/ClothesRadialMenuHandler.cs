using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using Altv_Roleplay.Factories;
using Altv_Roleplay.Model;
using Altv_Roleplay.Services;
using Altv_Roleplay.Utils;

namespace Altv_Roleplay.Handler
{
    class ClothesRadialMenuHandler : IScript
    {
        [AsyncClientEvent("Server:ClothesRadial:GetClothesRadialItems")]
        public async Task GetAnimationItems(IPlayer player)
        {
            try
            {
                var interactHTML = ""; 
                interactHTML += "<li><p id='InteractionMenu-SelectedTitle'>Schließen</p></li><li class='interactitem' data-action='close' data-actionstring='Schließen'><img src='../utils/img/cancel.png'></li>";

                interactHTML += "<li class='interactitem' id='InteractionMenu-maske' data-action='maske' data-actionstring='Maske ausziehen'><img src='../utils/img/Maske.png'></li>";
                interactHTML += "<li class='interactitem' id='InteractionMenu-hut' data-action='hut' data-actionstring='Hut ausziehen'><img src='../utils/img/witch-hat.png'></li>";
                interactHTML += "<li class='interactitem' id='InteractionMenu-brille' data-action='brille' data-actionstring='Brille ausziehen'><img src='../utils/img/sun-glasses.png'></li>";
                interactHTML += "<li class='interactitem' id='InteractionMenu-tshirt' data-action='tshirt' data-actionstring='T-Shirt ausziehen'><img src='../utils/img/shirt.png'></li>";
                interactHTML += "<li class='interactitem' id='InteractionMenu-unterhemd' data-action='unterhemd' data-actionstring='Unterhemd ausziehen'><img src='../utils/img/undershirt.png'></li>";
                interactHTML += "<li class='interactitem' id='InteractionMenu-hose' data-action='hose' data-actionstring='Hose ausziehen'><img src='../utils/img/jeans.png'></li>";
                interactHTML += "<li class='interactitem' id='InteractionMenu-schuhe' data-action='schuhe' data-actionstring='Schuhe ausziehen'><img src='../utils/img/shoes.png'></li>";
                interactHTML += "<li class='interactitem' id='InteractionMenu-kette' data-action='kette' data-actionstring='Kette ausziehen'><img src='../utils/img/necklace.png'></li>";

                player.EmitLocked("Client:ClothesRadial:SetMenuItems", interactHTML);
            }
            catch(Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [AsyncClientEvent("Server:ClothesRadial:SetNormalSkin")]
        public static async Task SetNormalSkin(IPlayer player, string action)
        {
            if (player == null || !player.Exists) return;
            int charid = User.GetPlayerOnline(player);
            int type = 0;
            string ClothesType = "Cloth";
            string TypeText = "none";
            if (charid == 0) return;

            if (action == "maske")
            {
                if (!player.HasData("HasMaskOn"))
                {
                    player.SetClothes(1, 0, 0, 2);
                    player.SetData("HasMaskOn", true);
                    return;
                }
                type = 1;
                TypeText = "Mask";
                player.DeleteData("HasMaskOn");
            } else if (action == "hut")
            {
                if (!player.HasData("HasHatOn"))
                {
                    player.SetProps(0, 11, 0);
                    player.SetData("HasHatOn", true);
                    return;
                }
                type = 0;
                TypeText = "Hat";
                ClothesType = "Prop";
                player.DeleteData("HasHatOn");
            }
            else if (action == "brille")
            {
                if (!player.HasData("HasGlassesOn"))
                {
                    player.SetProps(1, 0, 0);
                    player.SetData("HasGlassesOn", true);
                    return;
                }
                type = 1;
                TypeText = "Glass";
                ClothesType = "Prop";
                player.DeleteData("HasGlassesOn");
            }
            else if (action == "tshirt")
            {
                if (!player.HasData("HasShirtOn"))
                {
                    player.SetClothes(3, 15, 0, 2);
                    player.SetClothes(11, 15, 0, 2);
                    player.SetData("HasShirtOn", true);
                    return;
                }
                type = 11;
                TypeText = "Top";
                player.DeleteData("HasShirtOn");
                player.SetClothes(3, (byte)ServerClothes.GetClothesDraw(Characters.GetCharacterClothes(charid, "Torso"), (byte)Convert.ToInt32(Characters.GetCharacterGender(((ClassicPlayer)player).CharacterId))), (byte)ServerClothes.GetClothesTexture(Characters.GetCharacterClothes(charid, "Torso"), (byte)Convert.ToInt32(Characters.GetCharacterGender(((ClassicPlayer)player).CharacterId))), 2);
            }
            else if (action == "unterhemd")
            {
                if (!player.HasData("HasUndershirtOn"))
                {
                    player.SetClothes(8, 15, 0, 2);
                    player.SetData("HasUndershirtOn", true);
                    return;
                }
                type = 8;
                TypeText = "Undershirt";
                player.DeleteData("HasUndershirtOn");
            }
            else if (action == "hose")
            {
                if (!player.HasData("HasPantsOn"))
                {
                    player.SetClothes(4, 14, 0, 2);
                    player.SetData("HasPantsOn", true);
                    return;
                }
                type = 4;
                TypeText = "Leg";
                player.DeleteData("HasPantsOn");
            }
            else if (action == "schuhe")
            {
                if (!player.HasData("HasShoesOn"))
                {
                    player.SetClothes(6, 34, 0, 2);
                    player.SetData("HasShoesOn", true);
                    return;
                }
                type = 6;
                TypeText = "Feet";
                player.DeleteData("HasShoesOn");
            }
            else if (action == "kette")
            {
                if (!player.HasData("HasNecklaceOn"))
                {
                    player.SetClothes(7, 0, 0, 2);
                    player.SetData("HasNecklaceOn", true);
                    return;
                }
                type = 7;
                TypeText = "Necklace";
                player.DeleteData("HasNecklaceOn");
            }


            if (TypeText == "none") return;
            if (ClothesType == "Prop")
            {
                player.SetProps((byte)type, (byte)ServerClothes.GetClothesDraw(Characters.GetCharacterClothes(charid, TypeText), (byte)Convert.ToInt32(Characters.GetCharacterGender(((ClassicPlayer)player).CharacterId))), (byte)ServerClothes.GetClothesTexture(Characters.GetCharacterClothes(charid, TypeText), (byte)Convert.ToInt32(Characters.GetCharacterGender(((ClassicPlayer)player).CharacterId)))); return;
            }
            player.SetClothes((byte)type, (byte)ServerClothes.GetClothesDraw(Characters.GetCharacterClothes(charid, TypeText), (byte)Convert.ToInt32(Characters.GetCharacterGender(((ClassicPlayer)player).CharacterId))), (byte)ServerClothes.GetClothesTexture(Characters.GetCharacterClothes(charid, TypeText), (byte)Convert.ToInt32(Characters.GetCharacterGender(((ClassicPlayer)player).CharacterId))), 2);
        }
    }
}
