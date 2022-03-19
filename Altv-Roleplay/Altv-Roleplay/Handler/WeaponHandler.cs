using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using Altv_Roleplay.Factories;
using Altv_Roleplay.Model;
using Altv_Roleplay.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Altv_Roleplay.Handler
{
    class WeaponHandler : IScript
    {

        [ClientEvent("Server:SetAmmo")]
        public void SetAmmo(ClassicPlayer player, int ammo)
        {

        }

        public static void EquipCharacterWeapon(IPlayer player, string type, string wName, int amount, string fromContainer)
        {
            try
            {
                int charId = User.GetPlayerOnline(player);
                string wType = "None";
                string normalWName = "None";
                string ammoWName = "None";
                WeaponModel wHash = 0;

                switch (wName)
                {
                    case "Pistole":
                    case "Pistolen Munition":
                        wType = "Secondary";
                        normalWName = "Pistole";
                        ammoWName = "Pistolen";
                        wHash = (WeaponModel)0x1B06D571;
                        break;
                    case "MiniMP":
                    case "MiniMP Munition":
                        wType = "Secondary";
                        normalWName = "MiniMP";
                        ammoWName = "MiniMP";
                        wHash = WeaponModel.MicroSMG;
                        break;
                    case "Gusenberg":
                    case "Gusenberg Munition":
                        wType = "Primary";
                        normalWName = "Gusenberg";
                        ammoWName = "Gusenberg";
                        wHash = WeaponModel.GusenbergSweeper;
                        break;
                    case "Revolver":
                    case "Revolver Munition":
                        wType = "Secondary";
                        normalWName = "Revolver";
                        ammoWName = "Revolver";
                        wHash = WeaponModel.HeavyRevolver;
                        break;
                    case "Bullpup":
                    case "Bullpup Munition":
                        wType = "Primary";
                        normalWName = "Bullpup";
                        ammoWName = "Bullpup";
                        wHash = WeaponModel.BullpupRifle;
                        break;
                    case "Doppelschrotflinte":
                    case "Doppelschrotflinte Munition":
                        wType = "Primary";
                        normalWName = "Doppelschrotflinte";
                        ammoWName = "Doppelschrotflinte";
                        wHash = WeaponModel.DoubleBarrelShotgun;
                        break;
                    case "Dolch":
                        wType = "Fist";
                        normalWName = "Dolch";
                        wHash = WeaponModel.AntiqueCavalryDagger;
                        break;
                    case "MiniAK":
                    case "MiniAK Munition":
                        wType = "Primary";
                        normalWName = "MiniAK";
                        ammoWName = "MiniAK";
                        wHash = WeaponModel.CompactRifle;
                        break;
                    case "Sniper":
                    case "Sniper Munition":
                        wType = "Primary";
                        normalWName = "Sniper";
                        ammoWName = "Sniper";
                        wHash = WeaponModel.SniperRifle;
                        break;
                    case "Gefechtspistole":
                    case "Gefechtspistole Munition":
                        wType = "Secondary";
                        normalWName = "Gefechtspistole";
                        ammoWName = "Gefechtspistole";
                        wHash = (WeaponModel)0x5EF9FEC4;
                        break;
                    case "MkII Pistole":
                    case "MkII Pistolen Munition":
                        wType = "Secondary";
                        normalWName = "MkII Pistole";
                        ammoWName = "MkII Pistolen";
                        wHash = (WeaponModel)0xBFE256D4;
                        break;
                    case "Pistole .50":
                    case "Pistole .50 Munition":
                        wType = "Secondary";
                        normalWName = "Pistole .50";
                        ammoWName = "Pistole .50";
                        wHash = (WeaponModel)0x99AEEB3B;
                        break;
                    case "Tazer":
                        wType = "Secondary";
                        wHash = WeaponModel.StunGun;
                        break;
                    case "Flaregun":
                    case "Flaregun Munition":
                        wType = "Secondary";
                        normalWName = "Flaregun";
                        ammoWName = "Flaregun";
                        wHash = (WeaponModel)0x47757124;
                        break;
                    case "PDW":
                    case "PDW Munition":
                        wType = "Primary";
                        normalWName = "PDW";
                        ammoWName = "PDW";
                        wHash = (WeaponModel)0x0A3D4D34;
                        break;
                    case "Karabiner":
                    case "Karabiner Munition":
                        wType = "Primary";
                        normalWName = "Karabiner";
                        ammoWName = "Karabiner";
                        wHash = (WeaponModel)0x83BF0278;
                        break;
                    case "SMG":
                    case "SMG Munition":
                        wType = "Primary";
                        normalWName = "SMG";
                        ammoWName = "SMG";
                        wHash = (WeaponModel)0x2BE6766B;
                        break;
                    case "Schlagstock":
                        wType = "Fist";
                        normalWName = "Schlagstock";
                        wHash = (WeaponModel)0x678B81B1;
                        break;
                    case "Messer":
                        wType = "Fist";
                        normalWName = "Messer";
                        wHash = (WeaponModel)0x99B507EA;
                        break;
                    case "Brecheisen":
                        wType = "Fist";
                        normalWName = "Brecheisen";
                        wHash = (WeaponModel)0x84BD7BFD;
                        break;
                    case "Baseballschlaeger":
                        wType = "Fist";
                        normalWName = "Baseballschlaeger";
                        wHash = (WeaponModel)0x958A4A8F;
                        break;
                    case "Hammer":
                        wType = "Fist";
                        normalWName = "Hammer";
                        wHash = (WeaponModel)0x4E875F73;
                        break;
                    case "Axt":
                        wType = "Fist";
                        normalWName = "Axt";
                        wHash = (WeaponModel)0xF9DCBF2D;
                        break;
                    case "Machete":
                        wType = "Fist";
                        normalWName = "Machete";
                        wHash = (WeaponModel)0xDD5DF8D9;
                        break;
                    case "Klappmesser":
                        wType = "Fist";
                        normalWName = "Klappmesser";
                        wHash = (WeaponModel)0xDFE37640;
                        break;
                    case "Schlagring":
                        wType = "Fist";
                        normalWName = "Schlagring";
                        wHash = (WeaponModel)0xD8DF3C3C;
                        break;
                    case "Taschenlampe":
                        wType = "Fist";
                        normalWName = "Taschenlampe";
                        wHash = (WeaponModel)0x8BB05FD7;
                        break;
                    case "Feuerlöscher":
                        wType = "Fist";
                        wHash = WeaponModel.FireExtinguisher;
                        break;
                }


                if (type == "Weapon")
                {
                    if (wType == "Primary")
                    {
                        string primWeapon = (string)Characters.GetCharacterWeapon(player, "PrimaryWeapon");

                        if (primWeapon == "None")
                        {
                            player.GiveWeapon(wHash, 0, true);
                            Characters.SetCharacterWeapon(player, "PrimaryWeapon", wName);
                            Characters.SetCharacterWeapon(player, "PrimaryAmmo", 0);
                            HUDHandler.SendNotification(player, 2, 5000, $"{wName} erfolgreich ausgerüstet.");
                            return;
                        }
                        else if (primWeapon == wName)
                        {
                            int wAmmoAmount = (int)Characters.GetCharacterWeapon(player, "PrimaryAmmo");
                            float invWeight = CharactersInventory.GetCharacterItemWeight(charId, "inventory");
                            float backpackWeight = CharactersInventory.GetCharacterItemWeight(charId, "backpack");
                            float bigWeight = invWeight + backpackWeight;
                            float itemWeight = ServerItems.GetItemWeight($"{ammoWName} Munition");
                            float multiWeight = itemWeight * wAmmoAmount;
                            float finalWeight = bigWeight + multiWeight;
                            float helpWeight = 15f + Characters.GetCharacterBackpackSize(Characters.GetCharacterBackpack(charId));
                            bool inBackpack = false;

                            if (invWeight + multiWeight > 15f && backpackWeight + multiWeight > Characters.GetCharacterBackpackSize(Characters.GetCharacterBackpack(charId))) { HUDHandler.SendNotification(player, 4, 5000, "Nicht genügend Platz."); return; }

                            if (finalWeight <= helpWeight)
                            {
                                HUDHandler.SendNotification(player, 2, 5000, $"{wName} erfolgreich abgelegt.");
                                player.EmitLocked("Client:WeaponAmmoChange:ComingRespond", (ulong)wHash);
                            }
                        }
                        else
                        {
                            HUDHandler.SendNotification(player, 3, 5000, "Du musst zuerst deine Hauptwaffe ablegen bevor du eine neue anlegen kannst.");
                        }
                    }
                    else if (wType == "Fist")
                    {
                        string fistWeapon = (string)Characters.GetCharacterWeapon(player, "FistWeapon");
                        if (fistWeapon == "None")
                        {
                            player.GiveWeapon(wHash, 0, false);
                            Characters.SetCharacterWeapon(player, "FistWeapon", wName);
                            Characters.SetCharacterWeapon(player, "FistWeaponAmmo", 0);
                            HUDHandler.SendNotification(player, 2, 500, $"{wName} erfolgreich ausgerüstet.");
                        }
                        else if (fistWeapon == wName)
                        {
                            float curWeight = CharactersInventory.GetCharacterItemWeight(charId, "inventory") + CharactersInventory.GetCharacterItemWeight(charId, "backpack");
                            float maxWeight = 15f + Characters.GetCharacterBackpackSize(Characters.GetCharacterBackpack(charId));
                            if (curWeight < maxWeight)
                            {
                                Characters.SetCharacterWeapon(player, "FistWeapon", "None");
                                Characters.SetCharacterWeapon(player, "FistWeaponAmmo", 0);
                                player.RemoveWeapon(wHash);
                                player.RemoveAllWeapons();

                                string secondaryWeapon2REMOVE = (string)Characters.GetCharacterWeapon(player, "SecondaryWeapon2");
                                string secondaryWeaponREMOVE = (string)Characters.GetCharacterWeapon(player, "SecondaryWeapon");
                                string primaryWeaponREMOVE = (string)Characters.GetCharacterWeapon(player, "PrimaryWeapon");
                                string FistWeaponRemove = (string)Characters.GetCharacterWeapon(player, "FistWeapon");

                                if (secondaryWeaponREMOVE != "None") //Wenn nur Sekündär1
                                {
                                    int ammodiS1 = (int)Characters.GetCharacterWeapon(player, "SecondaryAmmo");
                                    player.GiveWeapon(GetWeaponModelByName(secondaryWeaponREMOVE), ammodiS1, false);
                                }

                                if (secondaryWeapon2REMOVE != "None") //Wenn nur Sekündär2
                                {
                                    int ammodiS2 = (int)Characters.GetCharacterWeapon(player, "SecondaryAmmo2");
                                    player.GiveWeapon(GetWeaponModelByName(secondaryWeapon2REMOVE), ammodiS2, false);
                                }

                                if (secondaryWeapon2REMOVE != "None") //Wenn nur Primär1
                                {
                                    int ammodiP = (int)Characters.GetCharacterWeapon(player, "PrimaryAmmo");
                                    player.GiveWeapon(GetWeaponModelByName(primaryWeaponREMOVE), ammodiP, false);
                                }

                                if (FistWeaponRemove != "None") //Wenn nur FAUST
                                {
                                    player.GiveWeapon(GetWeaponModelByName(primaryWeaponREMOVE), 0, false);
                                }
                                HUDHandler.SendNotification(player, 2, 5000, $"{wName} erfolgreich abgelegt.");
                            }
                            else { HUDHandler.SendNotification(player, 4, 5000, "Du hast nicht genügend Platz."); }
                        }
                        else
                        {
                            HUDHandler.SendNotification(player, 3, 5000, "Du musst zuerst deine Schlagwaffe ablegen bevor du eine neue anlegen kannst.");
                        }
                    }
                    else if (wType == "Secondary")
                    {
                        string secondaryWeapon = (string)Characters.GetCharacterWeapon(player, "SecondaryWeapon");
                        string secondaryWeapon2 = (string)Characters.GetCharacterWeapon(player, "SecondaryWeapon2");

                        if (secondaryWeapon == "None" && wName != "Tazer")
                        {
                            if (secondaryWeapon2 == wName)
                            {
                                int ammoAmount = (int)Characters.GetCharacterWeapon(player, "SecondaryAmmo2");
                                float invWeight = CharactersInventory.GetCharacterItemWeight(charId, "inventory");
                                float backpackWeight = CharactersInventory.GetCharacterItemWeight(charId, "backpack");
                                float bigWeight = invWeight + backpackWeight;
                                float itemWeight = ServerItems.GetItemWeight($"{ammoWName} Munition");
                                float multiWeight = itemWeight * ammoAmount;
                                float finalWeight = bigWeight + multiWeight;
                                float helpWeight = 15f + Characters.GetCharacterBackpackSize(Characters.GetCharacterBackpack(charId));
                                bool inBackpack = false;
                                if (invWeight + multiWeight > 15f && backpackWeight + multiWeight > Characters.GetCharacterBackpackSize(Characters.GetCharacterBackpack(charId))) { HUDHandler.SendNotification(player, 4, 5000, "Nicht genügend Platz."); return; }

                                if (finalWeight <= helpWeight)
                                {
                                    HUDHandler.SendNotification(player, 2, 5000, $"{wName} erfolgreich abgelegt.");
                                    player.EmitLocked("Client:WeaponAmmoChange:ComingRespond", (ulong)wHash);

                                }
                            }
                            else
                            {
                                player.GiveWeapon(wHash, 0, true);
                                Characters.SetCharacterWeapon(player, "SecondaryWeapon", wName);
                                Characters.SetCharacterWeapon(player, "SecondaryAmmo", 0);
                                HUDHandler.SendNotification(player, 2, 5000, $"{wName} erfolgreich ausgerüstet.");
                            }
                        }
                        else if (secondaryWeapon == wName)
                        {
                            int ammoAmount = (int)Characters.GetCharacterWeapon(player, "SecondaryAmmo");
                            float invWeight = CharactersInventory.GetCharacterItemWeight(charId, "inventory");
                            float backpackWeight = CharactersInventory.GetCharacterItemWeight(charId, "backpack");
                            float bigWeight = invWeight + backpackWeight;
                            float itemWeight = ServerItems.GetItemWeight($"{ammoWName} Munition");
                            float multiWeight = itemWeight * ammoAmount;
                            float finalWeight = bigWeight + multiWeight;
                            float helpWeight = 15f + Characters.GetCharacterBackpackSize(Characters.GetCharacterBackpack(charId));
                            bool inBackpack = false;
                            if (invWeight + multiWeight > 15f && backpackWeight + multiWeight > Characters.GetCharacterBackpackSize(Characters.GetCharacterBackpack(charId))) { HUDHandler.SendNotification(player, 4, 5000, "Nicht genügend Platz."); return; }
                            if (finalWeight <= helpWeight)
                            {
                                HUDHandler.SendNotification(player, 2, 5000, $"{wName} erfolgreich abgelegt.");
                                player.EmitLocked("Client:WeaponAmmoChange:ComingRespond", (ulong)wHash);
                            }
                        }
                        else
                        {
                            if (secondaryWeapon2 == "None" && wName == "Tazer")
                            {
                                player.GiveWeapon(wHash, 0, true);
                                Characters.SetCharacterWeapon(player, "SecondaryWeapon2", wName);
                                Characters.SetCharacterWeapon(player, "SecondaryAmmo2", 0);
                                HUDHandler.SendNotification(player, 2, 5000, $"{wName} erfolgreich ausgerüstet.");
                            }
                            else if (secondaryWeapon2 == wName && wName == "Tazer")
                            {
                                int ammoAmount = (int)Characters.GetCharacterWeapon(player, "SecondaryAmmo2");
                                float invWeight = CharactersInventory.GetCharacterItemWeight(charId, "inventory");
                                float backpackWeight = CharactersInventory.GetCharacterItemWeight(charId, "backpack");
                                float bigWeight = invWeight + backpackWeight;
                                float itemWeight = ServerItems.GetItemWeight($"{ammoWName} Munition");
                                float multiWeight = itemWeight * ammoAmount;
                                float finalWeight = bigWeight + multiWeight;
                                float helpWeight = 15f + Characters.GetCharacterBackpackSize(Characters.GetCharacterBackpack(charId));
                                bool inBackpack = false;

                                if (finalWeight <= helpWeight)
                                {
                                    HUDHandler.SendNotification(player, 2, 5000, $"{wName} erfolgreich abgelegt.");
                                    player.EmitLocked("Client:WeaponAmmoChange:ComingRespond", (ulong)wHash); //Do In Timer Too

                                }
                            }
                            else { HUDHandler.SendNotification(player, 3, 5000, "Du musst zuerst deine Sekundärwaffe ablegen bevor du eine neue anlegen kannst."); }
                        }
                    }
                }
                else if (type == "Ammo")
                {
                    if (wType == "Primary")
                    {
                        string primaryWeapon = (string)Characters.GetCharacterWeapon(player, "PrimaryWeapon");
                        if (primaryWeapon == "None") { HUDHandler.SendNotification(player, 3, 5000, "Du hast keine Primärwaffe angelegt."); }
                        else if (primaryWeapon == normalWName)
                        {
                            int newAmmo = (int)Characters.GetCharacterWeapon(player, "PrimaryAmmo") + amount;
                            player.GiveWeapon(wHash, amount, true);
                            Characters.SetCharacterWeapon(player, "PrimaryAmmo", newAmmo);
                            HUDHandler.SendNotification(player, 2, 5000, $"Du hast {wName} in deine Waffe geladen.");

                            if (CharactersInventory.ExistCharacterItem(charId, $"{wName}", fromContainer))
                            {
                                CharactersInventory.RemoveCharacterItemAmount(charId, $"{wName}", amount, fromContainer);
                            }
                        }
                        else
                        {
                            HUDHandler.SendNotification(player, 3, 5000, "Die Munitionen passen nicht in deine Waffe.");
                        }
                    }
                    else if (wType == "Secondary")
                    {
                        string secondaryWeapon = (string)Characters.GetCharacterWeapon(player, "SecondaryWeapon");
                        if (secondaryWeapon == "None") { HUDHandler.SendNotification(player, 4, 5000, "Du hast keine Sekundärwaffe angelegt."); }
                        else if (secondaryWeapon == normalWName)
                        {
                            int newAmmo = (int)Characters.GetCharacterWeapon(player, "SecondaryAmmo") + amount;
                            player.GiveWeapon(wHash, amount, true);
                            Characters.SetCharacterWeapon(player, "SecondaryAmmo", newAmmo);
                            HUDHandler.SendNotification(player, 2, 5000, $"Du hast {wName} in deine Waffe geladen.");

                            if (CharactersInventory.ExistCharacterItem(charId, $"{wName}", fromContainer))
                            {
                                CharactersInventory.RemoveCharacterItemAmount(charId, $"{wName}", amount, fromContainer);
                            }
                        }
                        else
                        {
                            string secondary2Weapon = (string)Characters.GetCharacterWeapon(player, "SecondaryWeapon2");
                            if (secondary2Weapon == "None") { HUDHandler.SendNotification(player, 4, 5000, "Du hast keine Sekundärwaffe angelegt."); }
                            else if (secondary2Weapon == normalWName)
                            {
                                int newAmmo = (int)Characters.GetCharacterWeapon(player, "SecondaryAmmo2") + amount;
                                player.GiveWeapon(wHash, amount, true);
                                Characters.SetCharacterWeapon(player, "SecondaryAmmo2", newAmmo);
                                HUDHandler.SendNotification(player, 2, 5000, $"Du hast {wName} in deine Waffe geladen.");

                                if (CharactersInventory.ExistCharacterItem(charId, $"{wName}", fromContainer))
                                {
                                    CharactersInventory.RemoveCharacterItemAmount(charId, $"{wName}", amount, fromContainer);
                                }
                            }
                            else
                            {
                                HUDHandler.SendNotification(player, 4, 5000, "Die Munitionen passen nicht in deine Waffe.");
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static void SetWeaponComponents(IPlayer player, string wName)
        {
            if (player == null || !player.Exists) return;
            switch (wName)
            {
                case "PDW":
                    player.AddWeaponComponent(WeaponModel.CombatPDW, 0x7BC4CDDC); //Flashlight
                    player.AddWeaponComponent(WeaponModel.CombatPDW, 0xAA2C45B4); //Scope
                    player.AddWeaponComponent(WeaponModel.CombatPDW, 0xC164F53); //Grip
                    player.AddWeaponComponent(WeaponModel.CombatPDW, 0x4317F19E); //Magazin
                    break;
            }
        }

        public static WeaponModel GetWeaponModelByName(string wName)
        {
            WeaponModel wHash = 0;
            switch (wName)
            {
                case "Pistole": wHash = WeaponModel.Pistol; break;
                case "Gefechtspistole": wHash = WeaponModel.CombatPistol; break;
                case "Tazer": wHash = WeaponModel.StunGun; break;
                case "PDW": wHash = WeaponModel.CombatPDW; break;
                case "SMG": wHash = WeaponModel.SMG; break;
                case "Schlagstock": wHash = WeaponModel.Nightstick; break;
                case "Messer": wHash = WeaponModel.Knife; break;
                case "Brecheisen": wHash = WeaponModel.Crowbar; break;
                case "Baseballschlaeger": wHash = WeaponModel.BaseballBat; break;
                case "Dolch": wHash = WeaponModel.AntiqueCavalryDagger; break;
                case "Hammer": wHash = WeaponModel.Hammer; break;
                case "Axt": wHash = WeaponModel.Hatchet; break;
                case "Machete": wHash = WeaponModel.Machete; break;
                case "Klappmesser": wHash = WeaponModel.Switchblade; break;
                case "Feuerlöscher": wHash = WeaponModel.FireExtinguisher; break;
                case "Taschenlampe": wHash = WeaponModel.Flashlight; break;
                case "MiniMP": wHash = WeaponModel.MicroSMG; break;
                case "Gusenberg": wHash = WeaponModel.GusenbergSweeper; break;
                case "Bullpup": wHash = WeaponModel.BullpupRifle; break;
                case "Doppelschrotflinte": wHash = WeaponModel.DoubleBarrelShotgun; break;
                case "MiniAK": wHash = WeaponModel.CompactRifle; break;
                case "Sniper": wHash = WeaponModel.SniperRifle; break;
                case "Revolver": wHash = WeaponModel.HeavyRevolver; break;
            }
            return wHash;
        }

        [AsyncClientEvent("Server:WeaponAmmoChange:SecondRespond")]
        public static async Task WeaponAmmoChange(IPlayer player, int ammo, int hash)
        {
            try
            {
                if (WeaponHandler.GetWeaponModelByName((string)Characters.GetCharacterWeapon(player, "PrimaryWeapon")) == (WeaponModel)hash)
                {
                    var wName = Characters.GetCharacterWeapon(player, "PrimaryWeapon");
                } else if (WeaponHandler.GetWeaponModelByName((string)Characters.GetCharacterWeapon(player, "PrimaryWeapon")) == (WeaponModel)hash)
                {
                    var wName = Characters.GetCharacterWeapon(player, "SecondaryWeapon");
                } else if (WeaponHandler.GetWeaponModelByName((string)Characters.GetCharacterWeapon(player, "PrimaryWeapon")) == (WeaponModel)hash)
                {
                    var wName = Characters.GetCharacterWeapon(player, "SecondaryWeapon2");
                }



                if (WeaponHandler.GetWeaponModelByName((string)Characters.GetCharacterWeapon(player, "PrimaryWeapon")) == (WeaponModel)hash)
                {
                    var wName = Characters.GetCharacterWeapon(player, "PrimaryWeapon");

                    string wType = "None";
                    string normalWName = "None";
                    string ammoWName = "None";
                    WeaponModel wHash = 0;

                    switch (wName)
                    {
                        case "Gusenberg":
                        case "Gusenberg Munition":
                            wType = "Primary";
                            normalWName = "Gusenberg";
                            ammoWName = "Gusenberg";
                            wHash = WeaponModel.GusenbergSweeper;
                            break;
                        case "Bullpup":
                        case "Bullpup Munition":
                            wType = "Primary";
                            normalWName = "Bullpup";
                            ammoWName = "Bullpup";
                            wHash = WeaponModel.BullpupRifle;
                            break;
                        case "Doppelschrotflinte":
                        case "Doppelschrotflinte Munition":
                            wType = "Primary";
                            normalWName = "Doppelschrotflinte";
                            ammoWName = "Doppelschrotflinte";
                            wHash = WeaponModel.DoubleBarrelShotgun;
                            break;
                        case "MiniAK":
                        case "MiniAK Munition":
                            wType = "Primary";
                            normalWName = "MiniAK";
                            ammoWName = "MiniAK";
                            wHash = WeaponModel.CompactRifle;
                            break;
                        case "Sniper":
                        case "Sniper Munition":
                            wType = "Primary";
                            normalWName = "Sniper";
                            ammoWName = "Sniper";
                            wHash = WeaponModel.SniperRifle;
                            break;
                        case "Karabiner":
                        case "Karabiner Munition":
                            wType = "Primary";
                            normalWName = "Karabiner";
                            ammoWName = "Karabiner";
                            wHash = (WeaponModel)0x83BF0278;
                            break;
                        case "SMG":
                        case "SMG Munition":
                            wType = "Primary";
                            normalWName = "SMG";
                            ammoWName = "SMG";
                            wHash = (WeaponModel)0x2BE6766B;
                            break;
                    }


                    CharactersInventory.AddCharacterItem((int)player.GetCharacterMetaId(), $"{ammoWName} Munition", ammo, "inventory");
                    Characters.SetCharacterWeapon(player, "PrimaryWeapon", "None");
                    Characters.SetCharacterWeapon(player, "PrimaryAmmo", 0);

                    player.RemoveWeapon((uint)hash);
                    player.RemoveAllWeapons();


                    string secondaryWeapon2REMOVE = (string)Characters.GetCharacterWeapon(player, "SecondaryWeapon2");
                    string secondaryWeaponREMOVE = (string)Characters.GetCharacterWeapon(player, "SecondaryWeapon");
                    string primaryWeaponREMOVE = (string)Characters.GetCharacterWeapon(player, "PrimaryWeapon");
                    string FistWeaponRemove = (string)Characters.GetCharacterWeapon(player, "FistWeapon");

                    if (secondaryWeaponREMOVE != "None") //Wenn nur Sekündär1
                    {
                        int ammodiS1 = (int)Characters.GetCharacterWeapon(player, "SecondaryAmmo");
                        player.GiveWeapon(GetWeaponModelByName(secondaryWeaponREMOVE), ammodiS1, false);
                    }

                    if (secondaryWeapon2REMOVE != "None") //Wenn nur Sekündär2
                    {
                        int ammodiS1 = (int)Characters.GetCharacterWeapon(player, "SecondaryAmmo2");
                        player.GiveWeapon(GetWeaponModelByName(secondaryWeapon2REMOVE), ammodiS1, false);
                    }

                    if (primaryWeaponREMOVE != "None") //Wenn nur Primär1
                    {
                        int ammodiS1 = (int)Characters.GetCharacterWeapon(player, "PrimaryAmmo");
                        player.GiveWeapon(GetWeaponModelByName(primaryWeaponREMOVE), ammodiS1, false);
                    }

                    if (FistWeaponRemove != "None") //Wenn nur FAUST
                    {
                        player.GiveWeapon(GetWeaponModelByName(FistWeaponRemove), 0, false);
                    }
                }
                else if (WeaponHandler.GetWeaponModelByName((string)Characters.GetCharacterWeapon(player, "SecondaryWeapon")) == (WeaponModel)hash)
                {
                    var wName = Characters.GetCharacterWeapon(player, "SecondaryWeapon");

                    string wType = "None";
                    string normalWName = "None";
                    string ammoWName = "None";
                    WeaponModel wHash = 0;

                    switch (wName)
                    {
                        case "Pistole":
                        case "Pistolen Munition":
                            wType = "Secondary";
                            normalWName = "Pistole";
                            ammoWName = "Pistolen";
                            wHash = (WeaponModel)0x1B06D571;
                            break;
                        case "MiniMP":
                        case "MiniMP Munition":
                            wType = "Secondary";
                            normalWName = "MiniMP";
                            ammoWName = "MiniMP";
                            wHash = WeaponModel.MicroSMG;
                            break;
                        case "Revolver":
                        case "Revolver Munition":
                            wType = "Secondary";
                            normalWName = "Revolver";
                            ammoWName = "Revolver";
                            wHash = WeaponModel.HeavyRevolver;
                            break;
                        case "Gefechtspistole":
                        case "Gefechtspistole Munition":
                            wType = "Secondary";
                            normalWName = "Gefechtspistole";
                            ammoWName = "Gefechtspistole";
                            wHash = (WeaponModel)0x5EF9FEC4;
                            break;
                        case "MkII Pistole":
                        case "MkII Pistolen Munition":
                            wType = "Secondary";
                            normalWName = "MkII Pistole";
                            ammoWName = "MkII Pistolen";
                            wHash = (WeaponModel)0xBFE256D4;
                            break;
                        case "Pistole .50":
                        case "Pistole .50 Munition":
                            wType = "Secondary";
                            normalWName = "Pistole .50";
                            ammoWName = "Pistole .50";
                            wHash = (WeaponModel)0x99AEEB3B;
                            break;
                        case "Tazer":
                            wType = "Secondary";
                            wHash = WeaponModel.StunGun;
                            break;
                        case "Flaregun":
                        case "Flaregun Munition":
                            wType = "Secondary";
                            normalWName = "Flaregun";
                            ammoWName = "Flaregun";
                            wHash = (WeaponModel)0x47757124;
                            break;
                    }

                    CharactersInventory.AddCharacterItem((int)player.GetCharacterMetaId(), $"{ammoWName} Munition", ammo, "inventory");
                    Characters.SetCharacterWeapon(player, "SecondaryWeapon", "None");
                    Characters.SetCharacterWeapon(player, "SecondaryAmmo", 0);

                    player.RemoveWeapon((uint)hash);
                    player.RemoveAllWeapons();

                    string secondaryWeapon2REMOVE = (string)Characters.GetCharacterWeapon(player, "SecondaryWeapon2");
                    string secondaryWeaponREMOVE = (string)Characters.GetCharacterWeapon(player, "SecondaryWeapon");
                    string primaryWeaponREMOVE = (string)Characters.GetCharacterWeapon(player, "PrimaryWeapon");
                    string FistWeaponRemove = (string)Characters.GetCharacterWeapon(player, "FistWeapon");

                    if (secondaryWeaponREMOVE != "None") //Wenn nur Sekündär1
                    {
                        int ammodiS1 = (int)Characters.GetCharacterWeapon(player, "SecondaryAmmo");
                        player.GiveWeapon(GetWeaponModelByName(secondaryWeaponREMOVE), ammodiS1, false);
                    }

                    if (secondaryWeapon2REMOVE != "None") //Wenn nur Sekündär2
                    {
                        int ammodiS1 = (int)Characters.GetCharacterWeapon(player, "SecondaryAmmo2");
                        player.GiveWeapon(GetWeaponModelByName(secondaryWeapon2REMOVE), ammodiS1, false);
                    }

                    if (primaryWeaponREMOVE != "None") //Wenn nur Primär1
                    {
                        int ammodiS1 = (int)Characters.GetCharacterWeapon(player, "PrimaryAmmo");
                        player.GiveWeapon(GetWeaponModelByName(primaryWeaponREMOVE), ammodiS1, false);
                    }

                    if (FistWeaponRemove != "None") //Wenn nur FAUST
                    {
                        player.GiveWeapon(GetWeaponModelByName(FistWeaponRemove), 0, false);
                    }
                }
                else if (WeaponHandler.GetWeaponModelByName((string)Characters.GetCharacterWeapon(player, "SecondaryWeapon2")) == (WeaponModel)hash)
                {
                    var wName = Characters.GetCharacterWeapon(player, "SecondaryWeapon2");

                    string wType = "None";
                    string normalWName = "None";
                    string ammoWName = "None";
                    WeaponModel wHash = 0;

                    switch (wName)
                    {
                        case "Tazer":
                            wType = "Secondary";
                            wHash = WeaponModel.StunGun;
                            break;
                    }

                    Characters.SetCharacterWeapon(player, "SecondaryWeapon2", "None");
                    Characters.SetCharacterWeapon(player, "SecondaryAmmo2", 0);
                    player.RemoveWeapon((uint)hash);
                    player.RemoveAllWeapons();


                    string secondaryWeapon2REMOVE = (string)Characters.GetCharacterWeapon(player, "SecondaryWeapon2");
                    string secondaryWeaponREMOVE = (string)Characters.GetCharacterWeapon(player, "SecondaryWeapon");
                    string primaryWeaponREMOVE = (string)Characters.GetCharacterWeapon(player, "PrimaryWeapon");
                    string FistWeaponRemove = (string)Characters.GetCharacterWeapon(player, "FistWeapon");

                    if (secondaryWeaponREMOVE != "None") //Wenn nur Sekündär1
                    {
                        int ammodiS1 = (int)Characters.GetCharacterWeapon(player, "SecondaryAmmo");
                        player.GiveWeapon(GetWeaponModelByName(secondaryWeaponREMOVE), ammodiS1, false);
                    }

                    if (secondaryWeapon2REMOVE != "None") //Wenn nur Sekündär2
                    {
                        int ammodiS1 = (int)Characters.GetCharacterWeapon(player, "SecondaryAmmo2");
                        player.GiveWeapon(GetWeaponModelByName(secondaryWeapon2REMOVE), ammodiS1, false);
                    }

                    if (primaryWeaponREMOVE != "None") //Wenn nur Primär1
                    {
                        int ammodiS1 = (int)Characters.GetCharacterWeapon(player, "PrimaryAmmo");
                        player.GiveWeapon(GetWeaponModelByName(primaryWeaponREMOVE), ammodiS1, false);
                    }

                    if (FistWeaponRemove != "None") //Wenn nur FAUST
                    {
                        player.GiveWeapon(GetWeaponModelByName(FistWeaponRemove), 0, false);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }


        [AsyncClientEvent("Server:WeaponAmmoChange:ChangeRespond")]
        public static async Task WeaponAmmoChangeTimer(IPlayer player, int ammo, int hash)
        {
            try
            {
                if (WeaponHandler.GetWeaponModelByName((string)Characters.GetCharacterWeapon(player, "PrimaryWeapon")) == (WeaponModel)hash)
                {
                    Characters.SetCharacterWeapon(player, "PrimaryAmmo", ammo);
                }else if (WeaponHandler.GetWeaponModelByName((string)Characters.GetCharacterWeapon(player, "SecondaryWeapon")) == (WeaponModel)hash)
                {
                    Characters.SetCharacterWeapon(player, "SecondaryAmmo", ammo);
                }else if (WeaponHandler.GetWeaponModelByName((string)Characters.GetCharacterWeapon(player, "SecondaryWeapon2")) == (WeaponModel)hash)
                {
                    Characters.SetCharacterWeapon(player, "SecondaryAmmo2", ammo);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
