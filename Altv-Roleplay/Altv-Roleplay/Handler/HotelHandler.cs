using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using Altv_Roleplay.Model;
using Altv_Roleplay.models;
using Altv_Roleplay.Utils;

namespace Altv_Roleplay.Handler
{
    class HotelHandler : IScript
    {
        #region all
        internal static void openCEF(IPlayer player, Server_Hotels hotelPos)
        {
            try
            {
                if (player == null || !player.Exists) return;
                if (hotelPos == null) return;
                if (hotelPos.id <= 0) return;
                int charId = User.GetPlayerOnline(player);
                if (charId <= 0) return;
                ServerHotels.RequestHotelApartmentItems(player, hotelPos.id);                
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [AsyncClientEvent("Server:Hotel:RentHotel")]
        public async Task RentHotel(IPlayer player, int hotelId, int apartmentId)
        {
            try
            {
                if (player == null || !player.Exists || hotelId <= 0 || apartmentId <= 0) return;
                int charId = User.GetPlayerOnline(player);
                if (charId <= 0) return;
                if(!ServerHotels.ExistHotelApartment(hotelId, apartmentId)) { HUDHandler.SendNotification(player, 3, 5000, "Ein unerwarteter Fehler ist aufgetreten [HOTEL-001]."); return; }
                if (ServerHotels.HasCharacterAnApartment(charId)) { HUDHandler.SendNotification(player, 3, 5000, $"Du besitzt bereits ein Hotelzimmer in dem Hotel '{ServerHotels.GetCharacterRentedHotelName(charId)}'."); return; }
                if (ServerHotels.GetApartmentOwner(hotelId, apartmentId) > 0) { HUDHandler.SendNotification(player, 3, 5000, "Dieses Apartment ist bereits vermietet."); return; }
                if(!CharactersBank.HasCharacterBankMainKonto(charId)) { HUDHandler.SendNotification(player, 3, 5000, "Du hast noch kein Hauptkonto in der Bank festgelegt."); return; }
                int accNumber = CharactersBank.GetCharacterBankMainKonto(charId);
                if (accNumber <= 0) return;
                if (CharactersBank.GetBankAccountLockStatus(accNumber)) { HUDHandler.SendNotification(player, 3, 5000, "Dein Bankkonto ist gesperrt."); return; }
                if(CharactersBank.GetBankAccountMoney(accNumber) < ServerHotels.GetApartmentPrice(hotelId, apartmentId)) { HUDHandler.SendNotification(player, 3, 5000, $"Soviel Geld hast du auf deinem Konto nicht ({ServerHotels.GetApartmentPrice(hotelId, apartmentId)}$) - du hast {CharactersBank.GetBankAccountMoney(accNumber)}$"); return; }
                CharactersBank.SetBankAccountMoney(accNumber, CharactersBank.GetBankAccountMoney(accNumber) - ServerHotels.GetApartmentPrice(hotelId, apartmentId));
                ServerBankPapers.CreateNewBankPaper(accNumber, DateTime.Now.ToString("d", CultureInfo.CreateSpecificCulture("de-DE")), DateTime.Now.ToString("t", CultureInfo.CreateSpecificCulture("de-DE")), "Ausgehende Überweisung", "Hotelzahlung", $"Zimmerbuchung: {apartmentId}", $"+{ServerHotels.GetApartmentPrice(hotelId, apartmentId)}$", "Bankeinzug");
                ServerHotels.SetApartmentOwner(hotelId, apartmentId, charId);
                HUDHandler.SendNotification(player, 2, 5000, $"Sie haben sich das Zimmer mit der Zimmernummer '{apartmentId}' erfolgreich gemietet (Kosten: {ServerHotels.GetApartmentPrice(hotelId, apartmentId)}$). Dieses Zimmer läuft automatisch nach {ServerHotels.GetApartmentRentHours(hotelId, apartmentId)} Stunden ab.");
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [AsyncClientEvent("Server:Hotel:LockHotel")]
        public static async Task LockHotel(IPlayer player, int hotelId, int apartmentId)
        {
            try
            {
                if (player == null || !player.Exists || hotelId <= 0 || apartmentId <= 0) return;
                int charId = User.GetPlayerOnline(player);
                if (charId <= 0) return;
                if (!ServerHotels.ExistHotelApartment(hotelId, apartmentId)) { HUDHandler.SendNotification(player, 3, 5000, "Ein unerwarteter Fehler ist aufgetreten [HOTEL-001]."); return; }
                if (!ServerHotels.HasCharacterAnApartment(charId)) { HUDHandler.SendNotification(player, 3, 5000, $"Du besitzt hier kein Zimmer."); return; }
                if(ServerHotels.GetApartmentOwner(hotelId, apartmentId) != charId) { HUDHandler.SendNotification(player, 3, 5000, $"Du besitzt hier kein Zimmer."); return; }
                var hotel = ServerHotels.ServerHotelsApartments_.FirstOrDefault(x => x.hotelId == hotelId && x.id == apartmentId);
                if (hotel == null) return;
                if (hotel.isLocked) HUDHandler.SendNotification(player, 2, 2500, $"Du hast dein Zimmer aufgeschlossen");
                else HUDHandler.SendNotification(player, 4, 2500, $"Du hast dein Zimmer abgeschlossen");
                hotel.isLocked = !hotel.isLocked;     
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [AsyncClientEvent("Server:Hotel:EnterHotel")]
        public async Task EnterHotel(IPlayer player, int hotelId, int apartmentId)
        {
            try
            {
                if (player == null || !player.Exists || hotelId <= 0 || apartmentId <= 0) return;
                int charId = User.GetPlayerOnline(player);
                if (charId <= 0) return;
                if (!ServerHotels.ExistHotelApartment(hotelId, apartmentId)) { HUDHandler.SendNotification(player, 3, 5000, "Ein unerwarteter Fehler ist aufgetreten [HOTEL-001]."); return; }               
                if(ServerHotels.GetApartmentOwner(hotelId, apartmentId) <= 0) { HUDHandler.SendNotification(player, 3, 5000, "Fehler: Dieses Zimmer ist nicht vermietet."); return; }
                var hotel = ServerHotels.ServerHotelsApartments_.FirstOrDefault(x => x.hotelId == hotelId && x.id == apartmentId);
                if (hotel == null) return;
                if (!player.Position.IsInRange(ServerHotels.GetHotelPosition(hotelId, apartmentId), 3f)) return;
                if(hotel.isLocked) { HUDHandler.SendNotification(player, 3, 5000, "Fehler: Das Zimmer ist abgeschlossen."); return; }
                if (!ServerHouses.ExistInteriorId(hotel.interiorId)) return;
                player.Position = ServerHouses.GetInteriorExitPosition(hotel.interiorId);
                player.Dimension = 5000 + apartmentId;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        internal static void LeaveHotel(IPlayer player)
        {
            try
            {
                if (player == null || !player.Exists) return;
                int charId = User.GetPlayerOnline(player);
                if (charId <= 0) return;
                if (player.Dimension <= 0 || player.Dimension - 5000 < 0) return;
                int apartmentId = player.Dimension - 5000;
                int hotelId = ServerHotels.GetHotelIdByApartmentId(apartmentId);
                if (hotelId <= 0 || apartmentId <= 0) return;
                if (!ServerHotels.ExistHotelApartment(hotelId, apartmentId)) { HUDHandler.SendNotification(player, 3, 5000, "Ein unerwarteter Fehler ist aufgetreten [HOTEL-001]."); return; }
                player.Position = ServerHotels.GetHotelPosition(hotelId, apartmentId);
                player.Dimension = 0;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }
        #endregion

        #region storage
        internal static void openStorage(IPlayer player)
        {
            try
            {
                if (player == null || !player.Exists) return;
                int charId = User.GetPlayerOnline(player);
                if (charId <= 0) return;
                if (!ServerHotels.HasCharacterAnApartment(charId)) return;
                int dimension = player.Dimension;
                if (dimension <= 5000) return;
                int apartmentId = dimension - 5000;
                int hotelId = ServerHotels.GetHotelIdByApartmentId(apartmentId);
                if (apartmentId <= 0 || hotelId <= 0 || !ServerHotels.ExistHotelApartment(hotelId, apartmentId)) return;
                var hotelStorageContent = ServerHotels.GetServerHotelStorageItems(apartmentId); //Apartment Items
                var characterInvArray = CharactersInventory.GetCharacterInventory(charId); //Spieler Inventar
                player.EmitLocked("Client:FactionStorage:openCEF", charId, apartmentId, "hotel", characterInvArray, hotelStorageContent);
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [AsyncClientEvent("Server:HotelStorage:StorageItem")]
        public async Task StorageHotelItem(IPlayer player, int apartmentId, string itemName, int itemAmount, string fromContainer)
        {
            try
            {
                if (player == null || !player.Exists || apartmentId <= 0 || itemName == "" || itemName == "undefined" || itemAmount <= 0 || fromContainer == "none" || fromContainer == "") return;
                int cCharId = User.GetPlayerOnline(player);
                if (cCharId <= 0) return;
                if (player.HasPlayerHandcuffs() || player.HasPlayerRopeCuffs()) { HUDHandler.SendNotification(player, 3, 5000, "Wie willst du das mit Handschellen/Fesseln machen?"); return; }
                if (!CharactersInventory.ExistCharacterItem(cCharId, itemName, fromContainer)) { HUDHandler.SendNotification(player, 4, 5000, "Fehler: Diesen Gegenstand besitzt du nicht."); return; }
                if (CharactersInventory.GetCharacterItemAmount(cCharId, itemName, fromContainer) < itemAmount) { HUDHandler.SendNotification(player, 4, 5000, "Fehler: Du hast nicht genügend Gegenstände davon dabei."); return; }
                if (CharactersInventory.IsItemActive(player, itemName)) { HUDHandler.SendNotification(player, 4, 5000, "Fehler: Ausgerüstete Gegenstände können nicht umgelagert werden."); return; }
                float itemWeight = ServerItems.GetItemWeight(itemName) * itemAmount;
                if (ServerHotels.GetHotelStorageItemWeight(apartmentId) >= 15f || (ServerHotels.GetHotelStorageItemWeight(apartmentId) + itemWeight) >= 15f)
                {
                    HUDHandler.SendNotification(player, 3, 5000, "Fehler: Soviel passt in das Lager nicht rein (maximal 15kg Lagerplatz).");
                    return;
                }
                CharactersInventory.RemoveCharacterItemAmount(cCharId, itemName, itemAmount, fromContainer);
                ServerHotels.AddServerHotelStorageItem(apartmentId, itemName, itemAmount);
                HUDHandler.SendNotification(player, 2, 5000, $"Der Gegenstand wurde erfolgreich eingelagert ({itemName} - {itemAmount}x).");
                //LoggingService.NewHotelLog(apartmentId, cCharId, 0, "storage", $"{Characters.GetCharacterName(charId)} ({charId}) hat den Gegenstand '{itemName} ({amount}x)' in seinen Spind gelegt."); //ToDo: Hotel Storage Log
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }


        [AsyncClientEvent("Server:HotelStorage:TakeItem")]
        public async Task TakeHotelItem(IPlayer player, int apartmentId, string itemName, int itemAmount)
        {
            try
            {
                if (player == null || !player.Exists || apartmentId <= 0 | itemAmount <= 0 || itemName == "" || itemName == "undefined") return;
                int charId = User.GetPlayerOnline(player);
                if (charId <= 0) return;
                if (player.HasPlayerHandcuffs() || player.HasPlayerRopeCuffs()) { HUDHandler.SendNotification(player, 3, 5000, "Wie willst du das mit Handschellen/Fesseln machen?"); return; }
                if(!ServerHotels.ExistServerHotelStorageItem(apartmentId, itemName)) { HUDHandler.SendNotification(player, 4, 5000, "Fehler: Der Gegenstand existiert im Lager nicht."); return; }
                if (ServerHotels.GetServerHotelStorageItemAmount(apartmentId, itemName) < itemAmount) { HUDHandler.SendNotification(player, 4, 5000, "Fehler: Soviele Gegenstände sind nicht im Lager."); return; }
                float itemWeight = ServerItems.GetItemWeight(itemName) * itemAmount;
                float invWeight = CharactersInventory.GetCharacterItemWeight(charId, "inventory");
                float backpackWeight = CharactersInventory.GetCharacterItemWeight(charId, "backpack");
                var itemType = ServerItems.GetItemType(itemName);
                if (invWeight + itemWeight > 15f && backpackWeight + itemWeight > Characters.GetCharacterBackpackSize(Characters.GetCharacterBackpack(charId))) { HUDHandler.SendNotification(player, 3, 5000, $"Du hast nicht genug Platz in deinen Taschen."); return; }
                ServerHotels.RemoveServerHotelStorageItemAmount(apartmentId, itemName, itemAmount);
                //LoggingService.NewFactionLog(factionId, charId, 0, "storage", $"{Characters.GetCharacterName(charId)} ({charId}) hat den Gegenstand '{itemName} ({amount}x)' aus seinem Spind entnommen."); // ToDo: Hotel Log
                if(itemName.Contains("Fahrzeugschluessel"))
                {
                    HUDHandler.SendNotification(player, 2, 5000, $"Du hast {itemName} ({itemAmount}x) aus deinem Lager genommen (Lagerort: Schluesselbund).");
                    CharactersInventory.AddCharacterItem(charId, itemName, itemAmount, "schluessel");
                    return;
                }if(itemName.Contains("Handschellenschluessel"))
                {
                    HUDHandler.SendNotification(player, 2, 5000, $"Du hast {itemName} ({itemAmount}x) aus deinem Lager genommen (Lagerort: Schluesselbund).");
                    CharactersInventory.AddCharacterItem(charId, itemName, itemAmount, "schluessel");
                    return;
                }if (itemName.Contains("Generalschluessel"))
                {
                    HUDHandler.SendNotification(player, 2, 5000, $"Du hast {itemName} ({itemAmount}x) aus deinem Lager genommen (Lagerort: Schluesselbund).");
                    CharactersInventory.AddCharacterItem(charId, itemName, itemAmount, "schluessel");
                    return;
                }
                else
                {
                    if (invWeight + itemWeight <= 15f)
                    {
                        HUDHandler.SendNotification(player, 2, 5000, $"Du hast {itemName} ({itemAmount}x) aus deinem Lager genommen (Lagerort: Inventar).");
                        CharactersInventory.AddCharacterItem(charId, itemName, itemAmount, "inventory");
                        return;
                    }

                    if (Characters.GetCharacterBackpack(charId) != -2 && backpackWeight + itemWeight <= Characters.GetCharacterBackpackSize(Characters.GetCharacterBackpack(charId)))
                    {
                        HUDHandler.SendNotification(player, 2, 5000, $"Du hast {itemName} ({itemAmount}x) aus deinem Lager genommen (Lagerort: Rucksack / Tasche).");
                        CharactersInventory.AddCharacterItem(charId, itemName, itemAmount, "backpack");
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }
        #endregion
    }
}
