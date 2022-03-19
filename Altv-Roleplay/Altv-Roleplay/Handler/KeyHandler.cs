
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using Altv_Roleplay.Factories;
using Altv_Roleplay.Model;
using Altv_Roleplay.models;
using Altv_Roleplay.Utils;

namespace Altv_Roleplay.Handler
{
    class KeyHandler : IScript
    {
        [AsyncClientEvent("Server:KeyHandler:PressE")]
        public async Task PressE(IPlayer player)
        {
            lock (player)
            {
                if (player == null || !player.Exists) return;
                int charId = User.GetPlayerOnline(player);
                if (charId == 0) return;


                ClassicColshape serverDoorLockCol = (ClassicColshape)ServerDoors.ServerDoorsLockColshapes_.FirstOrDefault(x => ((ClassicColshape)x).IsInRange((ClassicPlayer)player));
                if (serverDoorLockCol != null)
                {
                    var doorColData = ServerDoors.ServerDoors_.FirstOrDefault(x => x.id == (int)serverDoorLockCol.GetColShapeId());
                    if (doorColData != null)
                    {
                        string doorKey = doorColData.doorKey;
                        string doorKey2 = doorColData.doorKey2;
                        if (doorKey == null || doorKey2 == null) return;
                        if (!CharactersInventory.ExistCharacterItem(charId, doorKey, "schluessel") && !CharactersInventory.ExistCharacterItem(charId, doorKey, "schluessel") && !CharactersInventory.ExistCharacterItem(charId, doorKey2, "schluessel") && !CharactersInventory.ExistCharacterItem(charId, doorKey2, "schluessel")) return;

                        if (!doorColData.state) { HUDHandler.SendNotification(player, 4, 1500, "Tür abgeschlossen."); }
                        else { HUDHandler.SendNotification(player, 2, 1500, "Tür aufgeschlossen."); }
                        doorColData.state = !doorColData.state;
                        Alt.EmitAllClients("Client:DoorManager:ManageDoor", doorColData.hash, new Position(doorColData.posX, doorColData.posY, doorColData.posZ), (bool)doorColData.state);
                        return;
                    }
                }

                ClassicColshape farmCol = (ClassicColshape)ServerFarmingSpots.ServerFarmingSpotsColshapes_.FirstOrDefault(x => ((ClassicColshape)x).IsInRange((ClassicPlayer)player));
                if (farmCol != null && !player.IsInVehicle)
                {
                    if (player.HasPlayerHandcuffs() || player.HasPlayerRopeCuffs()) { HUDHandler.SendNotification(player, 3, 5000, "Wie willst du das mit Handschellen/Fesseln machen?"); return; }
                    if (player.GetPlayerFarmingActionMeta() != "None") return;
                    var farmColData = ServerFarmingSpots.ServerFarmingSpots_.FirstOrDefault(x => x.id == (int)farmCol.GetColShapeId());

                    if (farmColData != null)
                    {
                        if (farmColData.itemName.Contains("Eisenerz") && farmColData.neededItemToFarm != "None")
                        {
                            if (!CharactersInventory.ExistCharacterItem(charId, "Spitzhacke", "inventory") && !CharactersInventory.ExistCharacterItem(charId, "Spitzhacke", "backpack")) 
                            { 
                                HUDHandler.SendNotification(player, 3, 3500, $"Zum Farmen benötigst du: *Spitzhacke*."); 
                                return; 
                            }
                        }
                        if (farmColData.itemName.Contains("Kupfererz") && farmColData.neededItemToFarm != "None")
                        {
                            if (!CharactersInventory.ExistCharacterItem(charId, "Spitzhacke", "inventory") && !CharactersInventory.ExistCharacterItem(charId, "Spitzhacke", "backpack")) 
                            { 
                                HUDHandler.SendNotification(player, 3, 3500, $"Zum Farmen benötigst du: *Spitzhacke*."); 
                                return; 
                            }
                        }
                        player.SetPlayerFarmingActionMeta("farm");
                        FarmingHandler.FarmFieldAction(player, farmColData.itemName, farmColData.itemMinAmount, farmColData.itemMaxAmount, farmColData.animation, farmColData.duration);
                        return;
                    }
                }

                ClassicColshape farmProducerCol = (ClassicColshape)ServerFarmingSpots.ServerFarmingProducerColshapes_.FirstOrDefault(x => ((ClassicColshape)x).IsInRange((ClassicPlayer)player));
                if (farmProducerCol != null && !player.IsInVehicle)
                {
                    if (player.GetPlayerFarmingActionMeta() != "None") { HUDHandler.SendNotification(player, 3, 5000, $"Warte einen Moment."); return; }
                    var farmColData = ServerFarmingSpots.ServerFarmingProducer_.FirstOrDefault(x => x.id == (int)farmProducerCol.GetColShapeId());
                    if (farmColData != null)
                    {
                        //FarmingHandler.ProduceItem(player, farmColData.neededItem, farmColData.producedItem, farmColData.neededItemAmount, farmColData.producedItemAmount, farmColData.duration);
                        FarmingHandler.openFarmingCEF(player, farmColData.neededItem, farmColData.producedItem, farmColData.neededItemAmount, farmColData.producedItemAmount, farmColData.duration, farmColData.neededItemTWO, farmColData.neededItemTHREE, farmColData.neededItemTWOAmount, farmColData.neededItemTHREEAmount);
                        return;
                    }
                }        
                
                if(((ClassicColshape)Minijobs.Elektrolieferant.Main.startJobShape).IsInRange((ClassicPlayer)player))
                {
                    Minijobs.Elektrolieferant.Main.StartMinijob(player);
                    return;
                }

                if(((ClassicColshape)Minijobs.Pilot.Main.startJobShape).IsInRange((ClassicPlayer)player))
                {
                    Minijobs.Pilot.Main.TryStartMinijob(player);
                    return;
                }

                if(((ClassicColshape)Minijobs.Müllmann.Main.startJobShape).IsInRange((ClassicPlayer)player))
                {
                    Minijobs.Müllmann.Main.StartMinijob(player);
                    return;
                }

                if(((ClassicColshape)Minijobs.Busfahrer.Main.startJobShape).IsInRange((ClassicPlayer)player))
                {
                    Minijobs.Busfahrer.Main.TryStartMinijob(player);
                    return;
                }

                var houseEntrance = ServerHouses.ServerHouses_.FirstOrDefault(x => ((ClassicColshape)x.entranceShape).IsInRange((ClassicPlayer)player));
                if (houseEntrance != null && player.Dimension == 0)
                {
                    HouseHandler.openEntranceCEF(player, houseEntrance.id);
                    return;
                }

                var hotelPos = ServerHotels.ServerHotels_.FirstOrDefault(x => player.Position.IsInRange(new Position(x.posX, x.posY, x.posZ), 2f));
                if (hotelPos != null && !player.IsInVehicle)
                {
                    HotelHandler.openCEF(player, hotelPos);
                    return;
                }
                
                if(player.Dimension >= 5000)
                {
                    int houseInteriorCount = ServerHouses.GetMaxInteriorsCount();
                    for(var i = 1; i <= houseInteriorCount; i++)
                    {
                        if (i > houseInteriorCount || i <= 0) continue;
                        if((player.Dimension >= 5000 && player.Dimension < 10000) && player.Position.IsInRange(ServerHouses.GetInteriorExitPosition(i), 2f)) {
                            //Apartment Leave
                            HotelHandler.LeaveHotel(player);
                            return;
                        } 
                        else if((player.Dimension >= 5000 && player.Dimension < 10000) && player.Position.IsInRange(ServerHouses.GetInteriorStoragePosition(i), 2f))
                        {
                            //Apartment Storage
                            HotelHandler.openStorage(player);
                            return;
                        }
                        else if(player.Dimension >= 10000 && player.Position.IsInRange(ServerHouses.GetInteriorExitPosition(i), 2f))
                        {
                            //House Leave
                            HouseHandler.LeaveHouse(player, i);
                            return;
                        }
                        else if(player.Dimension >= 10000 && player.Position.IsInRange(ServerHouses.GetInteriorStoragePosition(i), 2f))
                        {
                            //House Storage
                            HouseHandler.openStorage(player);
                            return;
                        }
                        else if(player.Dimension >= 10000 && player.Position.IsInRange(ServerHouses.GetInteriorManagePosition(i), 2f))
                        {
                            //Hausverwaltung
                            HouseHandler.openManageCEF(player);
                            return;
                        }
                    }
                }                 

                var teleportsPos = ServerItems.ServerTeleports_.FirstOrDefault(x => player.Position.IsInRange(new Position(x.posX, x.posY, x.posZ), 1.5f));
                if (teleportsPos != null && !player.IsInVehicle)
                {
                    player.Position = new Position(teleportsPos.targetX, teleportsPos.targetY, teleportsPos.targetZ + 0.5f);
                    return;
                }

                var shopPos = ServerShops.ServerShops_.FirstOrDefault(x => player.Position.IsInRange(new Position(x.posX, x.posY, x.posZ), 3f));
                if (shopPos != null && !player.IsInVehicle)
                {
                    if (player.HasPlayerHandcuffs() || player.HasPlayerRopeCuffs()) { HUDHandler.SendNotification(player, 3, 5000, "Wie willst du das mit Handschellen/Fesseln machen?"); return; }
                    ShopHandler.openShop(player, shopPos);
                    return;
                }

                var garagePos = ServerGarages.ServerGarages_.FirstOrDefault(x => player.Position.IsInRange(new Position(x.posX, x.posY, x.posZ), 2f));
                if (garagePos != null && !player.IsInVehicle)
                {
                    GarageHandler.OpenGarageCEF(player, garagePos.id);
                    return;
                }

                var schluesselPos = player.Position.IsInRange(Constants.Positions.Vehicleschluesseldienst_Position, 3f);
                if (schluesselPos != false && !player.IsInVehicle)
                {
/*                    HUDHandler.SendNotification(player, 2, 3500, "Schluesseldienst oeffnet...! Bitte Warten!");
*/                    ShopHandler.openschluesselShop((ClassicPlayer)player);
                    return;
                }

                var waschPos = player.Position.IsInRange(Constants.Positions.Waschstrasse, 7.5f);
                var waschPos2 = player.Position.IsInRange(Constants.Positions.Waschstrasse2, 7.5f);
                var waschPos3 = player.Position.IsInRange(Constants.Positions.Waschstrasse3, 7.5f);
                var waschPos4 = player.Position.IsInRange(Constants.Positions.Waschstrasse4, 7.5f);
                var waschPos5 = player.Position.IsInRange(Constants.Positions.Waschstrasse5, 7.5f);
                var waschPos6 = player.Position.IsInRange(Constants.Positions.Waschstrasse6, 7.5f);
                var waschPos7 = player.Position.IsInRange(Constants.Positions.Waschstrasse7, 7.5f);
                if (waschPos != false && player.IsInVehicle)
                {
                    /*                  HUDHandler.SendNotification(player, 2, 3500, "Fahrzeug wird gewaschen! Bitte Warten!");
                    */
                    ShopHandler.usewaschstrasse(player);
                    return;
                }
                if (waschPos2 != false && player.IsInVehicle)
                {
                    /*                  HUDHandler.SendNotification(player, 2, 3500, "Fahrzeug wird gewaschen! Bitte Warten!");
                    */
                    ShopHandler.usewaschstrasse(player);
                    return;
                }
                if (waschPos3 != false && player.IsInVehicle)
                {
                    /*                  HUDHandler.SendNotification(player, 2, 3500, "Fahrzeug wird gewaschen! Bitte Warten!");
                    */
                    ShopHandler.usewaschstrasse(player);
                    return;
                }
                if (waschPos4 != false && player.IsInVehicle)
                {
                    /*                  HUDHandler.SendNotification(player, 2, 3500, "Fahrzeug wird gewaschen! Bitte Warten!");
                    */
                    ShopHandler.usewaschstrasse(player);
                    return;
                }
                if (waschPos5 != false && player.IsInVehicle)
                {
                    /*                  HUDHandler.SendNotification(player, 2, 3500, "Fahrzeug wird gewaschen! Bitte Warten!");
                    */
                    ShopHandler.usewaschstrasse(player);
                    return;
                }
                if (waschPos6 != false && player.IsInVehicle)
                {
                    /*                  HUDHandler.SendNotification(player, 2, 3500, "Fahrzeug wird gewaschen! Bitte Warten!");
                    */
                    ShopHandler.usewaschstrasse(player);
                    return;
                }
                if (waschPos7 != false && player.IsInVehicle)
                {
                    /*                  HUDHandler.SendNotification(player, 2, 3500, "Fahrzeug wird gewaschen! Bitte Warten!");
                    */
                    ShopHandler.usewaschstrasse(player);
                    return;
                }

                var clothesShopPos = ServerClothesShops.ServerClothesShops_.FirstOrDefault(x => player.Position.IsInRange(new Position(x.posX, x.posY, x.posZ), 2f));
                if(clothesShopPos != null && !player.IsInVehicle)
                {
/*                    HUDHandler.SendNotification(player, 2, 3500, "Kleidungsladen oeffnet...! Bitte Warten!");
*/                    ShopHandler.openClothesShop((ClassicPlayer)player, clothesShopPos.id);
                    return;
                }

                var vehicleShopPos = ServerVehicleShops.ServerVehicleShops_.FirstOrDefault(x => player.Position.IsInRange(new Position(x.pedX, x.pedY, x.pedZ), 2f));
                if (vehicleShopPos != null && !player.IsInVehicle)
                {
                    if (vehicleShopPos.neededLicense != "None" && !Characters.HasCharacterPermission(charId, vehicleShopPos.neededLicense)) { HUDHandler.SendNotification(player, 3, 5000, $"Du hast nicht die benötigte Lizenz."); return; }
                    //LSPD
                    if (vehicleShopPos.id == 6 && ServerFactions.GetCharacterFactionId(charId) != 1 && !CharactersInventory.ExistCharacterItem(charId, "Fahrzeugschluessel LSPD", "schluessel")) { HUDHandler.SendNotification(player, 3, 5000, $"Du hast hier keinen Zugriff drauf. [LSPD]"); return; }
                    if (vehicleShopPos.id == 7 && ServerFactions.GetCharacterFactionId(charId) != 1 && !CharactersInventory.ExistCharacterItem(charId, "Fahrzeugschluessel LSPD", "schluessel")) { HUDHandler.SendNotification(player, 3, 5000, $"Du hast hier keinen Zugriff drauf. [LSPD]"); return; }
                    //MD
                    if (vehicleShopPos.id == 8 && ServerFactions.GetCharacterFactionId(charId) != 4 && !CharactersInventory.ExistCharacterItem(charId, "Fahrzeugschluessel MD", "schluessel")) { HUDHandler.SendNotification(player, 3, 5000, $"Du hast hier keinen Zugriff drauf. [MD]"); return; }
                    if (vehicleShopPos.id == 9 && ServerFactions.GetCharacterFactionId(charId) != 4 && !CharactersInventory.ExistCharacterItem(charId, "Fahrzeugschluessel MD", "schluessel")) { HUDHandler.SendNotification(player, 3, 5000, $"Du hast hier keinen Zugriff drauf. [MD]"); return; }
                    //ACLS
                    if (vehicleShopPos.id == 10 && ServerFactions.GetCharacterFactionId(charId) != 5 && !CharactersInventory.ExistCharacterItem(charId, "Fahrzeugschluessel ACLS", "schluessel")) { HUDHandler.SendNotification(player, 3, 5000, $"Du hast hier keinen Zugriff drauf. [ACLS]"); return; }
                    //LSF
                    if (vehicleShopPos.id == 23 && ServerFactions.GetCharacterFactionId(charId) != 6 && !CharactersInventory.ExistCharacterItem(charId, "Fahrzeugschluessel LSF", "schluessel")) { HUDHandler.SendNotification(player, 3, 5000, $"Du hast hier keinen Zugriff drauf. [LSF]"); return; }
                    if (vehicleShopPos.id == 24 && ServerFactions.GetCharacterFactionId(charId) != 6 && !CharactersInventory.ExistCharacterItem(charId, "Fahrzeugschluessel LSF", "schluessel")) { HUDHandler.SendNotification(player, 3, 5000, $"Du hast hier keinen Zugriff drauf. [LSF]"); return; }
                    if (vehicleShopPos.id == 99999 && ServerFactions.GetCharacterFactionId(charId) != 6 && !CharactersInventory.ExistCharacterItem(charId, "Fahrzeugschluessel LSF", "schluessel")) { HUDHandler.SendNotification(player, 3, 5000, $"Du hast hier keinen Zugriff drauf. [LSF]"); return; }
                    if (vehicleShopPos.id == 99998 && ServerFactions.GetCharacterFactionId(charId) != 6 && !CharactersInventory.ExistCharacterItem(charId, "Fahrzeugschluessel LSF", "schluessel")) { HUDHandler.SendNotification(player, 3, 5000, $"Du hast hier keinen Zugriff drauf. [LSF]"); return; }
                    //VUC
                    if (vehicleShopPos.id == 1000 && player.AdminLevel() <= 7) { HUDHandler.SendNotification(player, 3, 5000, $"Du hast hier keinen Zugriff drauf. [ADMINSHOP]"); return; }
                    //REST
                    ShopHandler.OpenVehicleShop(player, vehicleShopPos.name, vehicleShopPos.id);
                    return;
                }

                var vehicleSellPos = ServerVehicleShops.ServerVehicleShops_.FirstOrDefault(x => player.Position.IsInRange(new Position(x.sellX, x.sellY, x.sellZ), 10f));
                if (vehicleSellPos != null && player.IsInVehicle)
                {
                    if (vehicleSellPos.neededLicense != "None" && !Characters.HasCharacterPermission(charId, vehicleSellPos.neededLicense)) { HUDHandler.SendNotification(player, 3, 5000, $"Du hast nicht die benötigte Lizenz."); return; }
                    //LSPD
                    if (vehicleSellPos.id == 6 && ServerFactions.GetCharacterFactionId(charId) != 1 && !CharactersInventory.ExistCharacterItem(charId, "Fahrzeugschluessel LSPD", "schluessel")) { HUDHandler.SendNotification(player, 3, 5000, $"Du hast hier keinen Zugriff drauf. [LSPD]"); return; }
                    if (vehicleSellPos.id == 7 && ServerFactions.GetCharacterFactionId(charId) != 1 && !CharactersInventory.ExistCharacterItem(charId, "Fahrzeugschluessel LSPD", "schluessel")) { HUDHandler.SendNotification(player, 3, 5000, $"Du hast hier keinen Zugriff drauf. [LSPD]"); return; }
                    //MD
                    if (vehicleSellPos.id == 8 && ServerFactions.GetCharacterFactionId(charId) != 4 && !CharactersInventory.ExistCharacterItem(charId, "Fahrzeugschluessel MD", "schluessel")) { HUDHandler.SendNotification(player, 3, 5000, $"Du hast hier keinen Zugriff drauf. [MD]"); return; }
                    if (vehicleSellPos.id == 9 && ServerFactions.GetCharacterFactionId(charId) != 4 && !CharactersInventory.ExistCharacterItem(charId, "Fahrzeugschluessel MD", "schluessel")) { HUDHandler.SendNotification(player, 3, 5000, $"Du hast hier keinen Zugriff drauf. [MD]"); return; }
                    //ACLS
                    if (vehicleSellPos.id == 10 && ServerFactions.GetCharacterFactionId(charId) != 5 && !CharactersInventory.ExistCharacterItem(charId, "Fahrzeugschluessel ACLS", "schluessel")) { HUDHandler.SendNotification(player, 3, 5000, $"Du hast hier keinen Zugriff drauf. [ACLS]"); return; }
                    //LSF
                    if (vehicleSellPos.id == 23 && ServerFactions.GetCharacterFactionId(charId) != 6 && !CharactersInventory.ExistCharacterItem(charId, "Fahrzeugschluessel LSF", "schluessel")) { HUDHandler.SendNotification(player, 3, 5000, $"Du hast hier keinen Zugriff drauf. [LSF]"); return; }
                    if (vehicleSellPos.id == 24 && ServerFactions.GetCharacterFactionId(charId) != 6 && !CharactersInventory.ExistCharacterItem(charId, "Fahrzeugschluessel LSF", "schluessel")) { HUDHandler.SendNotification(player, 3, 5000, $"Du hast hier keinen Zugriff drauf. [LSF]"); return; }
                    if (vehicleSellPos.id == 99999 && ServerFactions.GetCharacterFactionId(charId) != 6 && !CharactersInventory.ExistCharacterItem(charId, "Fahrzeugschluessel LSF", "schluessel")) { HUDHandler.SendNotification(player, 3, 5000, $"Du hast hier keinen Zugriff drauf. [LSF]"); return; }
                    if (vehicleSellPos.id == 99998 && ServerFactions.GetCharacterFactionId(charId) != 6 && !CharactersInventory.ExistCharacterItem(charId, "Fahrzeugschluessel LSF", "schluessel")) { HUDHandler.SendNotification(player, 3, 5000, $"Du hast hier keinen Zugriff drauf. [LSF]"); return; }
                    //VUC
                    if (vehicleSellPos.id == 1000 && player.AdminLevel() <= 7) { HUDHandler.SendNotification(player, 3, 5000, $"Du hast hier keinen Zugriff drauf. [ADMINSHOP]"); return; }
                    //REST
                    ShopHandler.SellVehicle(player, vehicleSellPos.name, vehicleSellPos.id);
                    return;
                }

                var bankPos = ServerBanks.ServerBanks_.FirstOrDefault(x => player.Position.IsInRange(new Position(x.posX, x.posY, x.posZ), 1f));
                if (bankPos != null && !player.IsInVehicle)
                {
                    if (bankPos.zoneName == "Maze Bank Fraktion")
                    {
                        if (!ServerFactions.IsCharacterInAnyFaction(charId)) return;
                        if (ServerFactions.GetCharacterFactionRank(charId) != ServerFactions.GetFactionMaxRankCount(ServerFactions.GetCharacterFactionId(charId)) && ServerFactions.GetCharacterFactionRank(charId) != ServerFactions.GetFactionMaxRankCount(ServerFactions.GetCharacterFactionId(charId)) - 1) { return; }
                        player.EmitLocked("Client:FactionBank:createCEF", "faction", ServerFactions.GetCharacterFactionId(charId), ServerFactions.GetFactionBankMoney(ServerFactions.GetCharacterFactionId(charId)));
                        return;
                    }
                    if (bankPos.zoneName == "LSPD Bank Fraktion")
                    {
                        if (!ServerFactions.IsCharacterInAnyFaction(charId)) return;
                        if (ServerFactions.GetCharacterFactionId(charId) != 1) { HUDHandler.SendNotification(player, 4, 5000, "Fraktionsbank vom LSPD - Zugriff verweiget"); return; }
                        if (ServerFactions.GetCharacterFactionRank(charId) != ServerFactions.GetFactionMaxRankCount(ServerFactions.GetCharacterFactionId(charId)) && ServerFactions.GetCharacterFactionRank(charId) != ServerFactions.GetFactionMaxRankCount(ServerFactions.GetCharacterFactionId(charId)) - 1) { return; }
                        player.EmitLocked("Client:FactionBank:createCEF", "faction", ServerFactions.GetCharacterFactionId(charId), ServerFactions.GetFactionBankMoney(ServerFactions.GetCharacterFactionId(charId)));
                        return;
                    }if (bankPos.zoneName == "LSMD Bank Fraktion")
                    {
                        if (!ServerFactions.IsCharacterInAnyFaction(charId)) return;
                        if (ServerFactions.GetCharacterFactionId(charId) != 4) { HUDHandler.SendNotification(player, 4, 5000, "Fraktionsbank vom LSMD - Zugriff verweiget"); return; }
                        if (ServerFactions.GetCharacterFactionRank(charId) != ServerFactions.GetFactionMaxRankCount(ServerFactions.GetCharacterFactionId(charId)) && ServerFactions.GetCharacterFactionRank(charId) != ServerFactions.GetFactionMaxRankCount(ServerFactions.GetCharacterFactionId(charId)) - 1) { return; }
                        player.EmitLocked("Client:FactionBank:createCEF", "faction", ServerFactions.GetCharacterFactionId(charId), ServerFactions.GetFactionBankMoney(ServerFactions.GetCharacterFactionId(charId)));
                        return;
                    }
                    else if(bankPos.zoneName == "Maze Bank Company")
                    {
                        if (!ServerCompanys.IsCharacterInAnyServerCompany(charId)) return;
                        if(ServerCompanys.GetCharacterServerCompanyRank(charId) != 1 && ServerCompanys.GetCharacterServerCompanyRank(charId) != 2) { HUDHandler.SendNotification(player, 3, 5000, "Du hast kein Unternehmen auf welches du zugreifen kannst."); return; }
                        player.EmitLocked("Client:FactionBank:createCEF", "company", ServerCompanys.GetCharacterServerCompanyId(charId), ServerCompanys.GetServerCompanyMoney(ServerCompanys.GetCharacterServerCompanyId(charId)));
                        return;
                    }
                    else
                    {
                        var bankArray = CharactersBank.GetCharacterBankAccounts(charId);
                        player.EmitLocked("Client:Bank:createBankAccountManageForm", bankArray, bankPos.zoneName);
                        return;
                    }
                }             

                var barberPos = ServerBarbers.ServerBarbers_.FirstOrDefault(x => player.Position.IsInRange(new Position(x.posX, x.posY, x.posZ), 2f));
                if (barberPos != null && !player.IsInVehicle)
                {
                    player.EmitLocked("Client:Barber:barberCreateCEF", Characters.GetCharacterHeadOverlays(charId));
                    return;
                }

                if(player.Position.IsInRange(Constants.Positions.VehicleLicensing_Position, 3f))
                {
                    if (player.HasPlayerHandcuffs() || player.HasPlayerRopeCuffs()) { HUDHandler.SendNotification(player, 3, 5000, "Wie willst du das mit Handschellen/Fesseln machen?"); return; }
                    VehicleHandler.OpenLicensingCEF(player);
                    return;
                }

                if(player.Position.IsInRange(Constants.Positions.Schwarzwasch, 5f))
                {
                    if (player.HasPlayerHandcuffs() || player.HasPlayerRopeCuffs()) { HUDHandler.SendNotification(player, 3, 5000, "Wie willst du das mit Handschellen/Fesseln machen?"); return; }
                    RobberyHandler.washmoney(player);
                    return;
                }

                if (ServerFactions.IsCharacterInAnyFaction(charId))
                {
                    int factionId = ServerFactions.GetCharacterFactionId(charId);
                    var factionDutyPos = ServerFactions.ServerFactionPositions_.FirstOrDefault(x => x.factionId == factionId && x.posType == "duty" && player.Position.IsInRange(new Position(x.posX, x.posY, x.posZ), 5f));
                    if (factionDutyPos != null && !player.IsInVehicle)
                    {
                        bool isDuty = ServerFactions.IsCharacterInFactionDuty(charId);
                        ServerFactions.SetCharacterInFactionDuty(charId, !isDuty);
                        if (isDuty) { 
                            HUDHandler.SendNotification(player, 4, 5000, "Du hast dich erfolgreich vom Dienst abgemeldet."); 
                        }
                        else { 
                            HUDHandler.SendNotification(player, 2, 5000, "Du hast dich erfolgreich zum Dienst angemeldet."); 
                        }
                        if(factionId == 1 || factionId == 12) SmartphoneHandler.RequestLSPDIntranet((ClassicPlayer)player);
                        return;
                    }

                    var factionStoragePos = ServerFactions.ServerFactionPositions_.FirstOrDefault(x => x.factionId == factionId && x.posType == "storage" && player.Position.IsInRange(new Position(x.posX, x.posY, x.posZ), 2f));
                    if(factionStoragePos != null && !player.IsInVehicle)
                    {
                        if (player.HasPlayerHandcuffs() || player.HasPlayerRopeCuffs()) { HUDHandler.SendNotification(player, 3, 5000, "Wie willst du das mit Handschellen/Fesseln machen?"); return; }
                        bool isDuty = ServerFactions.IsCharacterInFactionDuty(charId);
                        if(isDuty && factionId == 1)
                        {
                            var factionStorageContent = ServerFactions.GetServerFactionStorageItems(factionId, charId); //Fraktionsspind Items
                            var CharacterInvArray = CharactersInventory.GetCharacterInventory(charId); //Spieler Inventar
                            player.EmitLocked("Client:FactionStorage:openCEF", charId, factionId, "faction", CharacterInvArray, factionStorageContent);
                            return;
                        }
                        if(isDuty && factionId == 2)
                        {
                            var factionStorageContent = ServerFactions.GetServerFactionStorageItems(factionId, charId); //Fraktionsspind Items
                            var CharacterInvArray = CharactersInventory.GetCharacterInventory(charId); //Spieler Inventar
                            player.EmitLocked("Client:FactionStorage:openCEF", charId, factionId, "faction", CharacterInvArray, factionStorageContent);
                            return;
                        }
                        if(isDuty && factionId == 3)
                        {
                            var factionStorageContent = ServerFactions.GetServerFactionStorageItems(factionId, charId); //Fraktionsspind Items
                            var CharacterInvArray = CharactersInventory.GetCharacterInventory(charId); //Spieler Inventar
                            player.EmitLocked("Client:FactionStorage:openCEF", charId, factionId, "faction", CharacterInvArray, factionStorageContent);
                            return;
                        }
                        if(isDuty && factionId == 4)
                        {
                            var factionStorageContent = ServerFactions.GetServerFactionStorageItems(factionId, charId); //Fraktionsspind Items
                            var CharacterInvArray = CharactersInventory.GetCharacterInventory(charId); //Spieler Inventar
                            player.EmitLocked("Client:FactionStorage:openCEF", charId, factionId, "faction", CharacterInvArray, factionStorageContent);
                            return;
                        }
                        if(isDuty && factionId == 5)
                        {
                            var factionStorageContent = ServerFactions.GetServerFactionStorageItems(factionId, charId); //Fraktionsspind Items
                            var CharacterInvArray = CharactersInventory.GetCharacterInventory(charId); //Spieler Inventar
                            player.EmitLocked("Client:FactionStorage:openCEF", charId, factionId, "faction", CharacterInvArray, factionStorageContent);
                            return;
                        }
                        if(isDuty && factionId == 6)
                        {
                            var factionStorageContent = ServerFactions.GetServerFactionStorageItems(factionId, charId); //Fraktionsspind Items
                            var CharacterInvArray = CharactersInventory.GetCharacterInventory(charId); //Spieler Inventar
                            player.EmitLocked("Client:FactionStorage:openCEF", charId, factionId, "faction", CharacterInvArray, factionStorageContent);
                            return;
                        }
                        if(isDuty && factionId == 7)
                        {
                            var factionStorageContent = ServerFactions.GetServerFactionStorageItems(factionId, charId); //Fraktionsspind Items
                            var CharacterInvArray = CharactersInventory.GetCharacterInventory(charId); //Spieler Inventar
                            player.EmitLocked("Client:FactionStorage:openCEF", charId, factionId, "faction", CharacterInvArray, factionStorageContent);
                            return;
                        }
                        if(isDuty && factionId == 8)
                        {
                            var factionStorageContent = ServerFactions.GetServerFactionStorageItems(factionId, charId); //Fraktionsspind Items
                            var CharacterInvArray = CharactersInventory.GetCharacterInventory(charId); //Spieler Inventar
                            player.EmitLocked("Client:FactionStorage:openCEF", charId, factionId, "faction", CharacterInvArray, factionStorageContent);
                            return;
                        }
                        if(isDuty && factionId == 9)
                        {
                            var factionStorageContent = ServerFactions.GetServerFactionStorageItems(factionId, charId); //Fraktionsspind Items
                            var CharacterInvArray = CharactersInventory.GetCharacterInventory(charId); //Spieler Inventar
                            player.EmitLocked("Client:FactionStorage:openCEF", charId, factionId, "faction", CharacterInvArray, factionStorageContent);
                            return;
                        } else
                        {
                            var factionStorageContent = ServerFactions.GetServerFactionStorageItems(factionId, charId); //Fraktionsspind Items
                            var CharacterInvArray = CharactersInventory.GetCharacterInventory(charId); //Spieler Inventar
                            player.EmitLocked("Client:FactionStorage:openCEF", charId, factionId, "faction", CharacterInvArray, factionStorageContent);
                            return;
                        }
                    }

                    var factionServicePhonePos = ServerFactions.ServerFactionPositions_.ToList().FirstOrDefault(x => x.factionId == factionId && x.posType == "servicephone" && player.Position.IsInRange(new Position(x.posX, x.posY, x.posZ), 2f));
                    if (factionServicePhonePos != null && !player.IsInVehicle && ServerFactions.IsCharacterInFactionDuty(charId))
                    {
                        int activeLeitstelle = ServerFactions.GetCurrentServicePhoneOwner(factionId);

                        if (activeLeitstelle <= 0)
                        {
                            ServerFactions.UpdateCurrentServicePhoneOwner(factionId, charId);
                            ServerFactions.sendMsg(factionId, $"{Characters.GetCharacterName(charId)} hat das Leitstellentelefon deiner Fraktion übernommen.");
                            return;
                        }
                        if (activeLeitstelle != charId)
                        {
                            HUDHandler.SendNotification(player, 2, 5000, $"Die Leitstelle ist aktuell vom Mitarbeiter {Characters.GetCharacterName(activeLeitstelle)} besetzt.");
                            return;
                        }
                        if (activeLeitstelle == charId)
                        {
                            ServerFactions.UpdateCurrentServicePhoneOwner(factionId, 0);
                            ServerFactions.sendMsg(factionId, $"{Characters.GetCharacterName(charId)} hat das Leitstellentelefon deiner Fraktion abgelegt.");
                            return;
                        }
                    }
                }

                if (player.Position.IsInRange(Constants.Positions.Jobcenter_Position, 2.5f) && !Characters.IsCharacterCrimeFlagged(charId) && !player.IsInVehicle) //Arbeitsamt
                {
                    if (ServerFactions.GetCharacterFactionId(charId) > 0) { HUDHandler.SendNotification(player, 4, 7500, "Da du in einer Legalen Fraktion bist, kannst du vom Jobcenter kein Jobangebot bekommen!"); return; }
                    TownhallHandler.createJobcenterBrowser(player);
                    return;
                }

                if (player.Position.IsInRange(Constants.Positions.TownhallHouseSelector, 2.5f))
                {
                    TownhallHandler.openHouseSelector(player);
                    return;
                }

                if (player.Position.IsInRange(Constants.Positions.IdentityCardApply, 2.5f) && Characters.GetCharacterAccState(charId) == 0 && !player.IsInVehicle) //Rathaus IdentityCardApply
                {
                    TownhallHandler.tryCreateIdentityCardApplyForm(player);
                    return;
                }

                if (player.Position.IsInRange(Constants.Positions.Clothes_Police, 2.5f) && !player.IsInVehicle)
                {
                    int factionId = ServerFactions.GetCharacterFactionId(charId);
                    if(factionId == 1)
                    {
                        if (!player.HasData("HasPDClothesOn"))
                        {

                            //player.EmitLocked("Client:SpawnArea:setCharAccessory", 0, 46, 0);    //  Kopfbedeckung
                            player.EmitLocked("Client:SpawnArea:setCharClothes", 1, 0, 0);         //  Sonnenbrille
                            player.EmitLocked("Client:SpawnArea:setCharClothes", 11, 374, 0);       //  Oberbekleidung
                            player.EmitLocked("Client:SpawnArea:setCharClothes", 3, 0, 0);         //  Körper
                            player.EmitLocked("Client:SpawnArea:setCharClothes", 8, 186, 0);       //  Unterbekleidung
                            player.EmitLocked("Client:SpawnArea:setCharClothes", 4, 129, 1);       //  Hose 
                            player.EmitLocked("Client:SpawnArea:setCharAccessory", 7, 0, 0);       //  Gürtel
                            player.EmitLocked("Client:SpawnArea:setCharClothes", 10, 8, 1);        //  Decals
                            player.EmitLocked("Client:SpawnArea:setCharClothes", 6, 25, 0);        //  Schuhe

                            //player.EmitLocked("Client:SpawnArea:setCharClothes", 9, 57, 0);      // Schutzweste

                            HUDHandler.SendNotification(player, 2, 2500, "Du hast deine Arbeitsklamotten angezogen.");
                            player.SetData("HasPDClothesOn", true);
                            Characters.SetCharacterArmor(charId, 100);
                        }
                        else
                        {
                            Characters.SetCharacterCorrectClothes(player);
                            HUDHandler.SendNotification(player, 4, 2500, "Du hast deine Arbeitsklamotten ausgezogen.");
                            player.DeleteData("HasPDClothesOn");
                        }
                    } else
                    {
                        HUDHandler.SendNotification(player, 2, 2500, "Du darfst den Kleiderschrank nicht nutzen!");
                    }
                    return;
                }

                if (player.Position.IsInRange(Constants.Positions.Clothes_Medic, 2.5f) && !player.IsInVehicle)
                {
                    int factionId = ServerFactions.GetCharacterFactionId(charId);
                    if (factionId == 4)
                    {
                        if (!player.HasData("HasMedicClothesOn"))
                        {
                            player.EmitLocked("Client:SpawnArea:setCharClothes", 4, 24, 2);
                            player.EmitLocked("Client:SpawnArea:setCharClothes", 11, 250, 0);
                            player.EmitLocked("Client:SpawnArea:setCharClothes", 8, 155, 0);
                            //player.EmitLocked("Client:SpawnArea:setCharClothes", 9, 12, 1); // WESTE
                            player.EmitLocked("Client:SpawnArea:setCharClothes", 10, 58, 1);
                            player.EmitLocked("Client:SpawnArea:setCharClothes", 6, 25, 0);
                            player.EmitLocked("Client:SpawnArea:setCharClothes", 3, 87, 0);
                            player.EmitLocked("Client:SpawnArea:setCharAccessory", 2, 2, 0);

                            player.EmitLocked("Client:SpawnArea:setCharAccessory", 7, 0, 0);
                            player.EmitLocked("Client:SpawnArea:setCharClothes", 1, 0, 0);
                            player.EmitLocked("Client:SpawnArea:setCharClothes", 9, 0, 0);
                            HUDHandler.SendNotification(player, 2, 2500, "Du hast deine Arbeitsklamotten angezogen.");
                            player.SetData("HasMedicClothesOn", true);
                            Characters.SetCharacterArmor(charId, 100);
                        }
                        else
                        {
                            Characters.SetCharacterCorrectClothes(player);
                            HUDHandler.SendNotification(player, 2, 2500, "Du hast deine Arbeitsklamotten ausgezogen.");
                            player.DeleteData("HasMedicClothesOn");
                        }
                    }
                    else
                    {
                        HUDHandler.SendNotification(player, 2, 2500, "Du darfst den Kleiderschrank nicht nutzen!");
                    }
                    return;
                }

                if (player.Position.IsInRange(Constants.Positions.Clothes_ACLS, 2.5f) && !player.IsInVehicle)
                {
                    int factionId = ServerFactions.GetCharacterFactionId(charId);
                    if (factionId == 5)
                    {
                       if (!player.HasData("HasMechanicClothesOn"))
                            {
                            player.EmitLocked("Client:SpawnArea:setCharClothes", 4, 98, 6);
                            player.EmitLocked("Client:SpawnArea:setCharClothes", 11, 248, 14);
                            player.EmitLocked("Client:SpawnArea:setCharClothes", 8, 153, 0);
                            player.EmitLocked("Client:SpawnArea:setCharClothes", 6, 25, 0);
                            player.EmitLocked("Client:SpawnArea:setCharAccessory", 2, 2, 0);
                            player.EmitLocked("Client:SpawnArea:setCharClothes", 3, 43, 0);

                            player.EmitLocked("Client:SpawnArea:setCharAccessory", 7, 0, 0);
                            player.EmitLocked("Client:SpawnArea:setCharClothes", 1, 0, 0);
                            player.EmitLocked("Client:SpawnArea:setCharClothes", 9, 0, 0);
                            HUDHandler.SendNotification(player, 2, 2500, "Du hast deine Arbeitsklamotten angezogen.");
                            player.SetData("HasMechanicClothesOn", true);
                            Characters.SetCharacterArmor(charId, 100);
                        }
                        else
                        {
                            player.DeleteData("HasMechanicClothesOn");
                            Characters.SetCharacterCorrectClothes(player);
                            HUDHandler.SendNotification(player, 2, 2500, "Du hast deine Arbeitsklamotten ausgezogen.");
                        }
                    }
                    else
                    {
                        HUDHandler.SendNotification(player, 2, 2500, "Du darfst den Kleiderschrank nicht nutzen!");
                    }
                    return;
                }
                if (player.Position.IsInRange(Constants.Positions.Clothes_VUC, 2.5f) && !player.IsInVehicle)
                {
                    int factionId = ServerFactions.GetCharacterFactionId(charId);
                    if (factionId == 5)
                    {
                        if (!player.HasData("HasVUCClothesOn"))
                        {
                            player.EmitLocked("Client:SpawnArea:setCharClothes", 4, 98, 6);
                            player.EmitLocked("Client:SpawnArea:setCharClothes", 11, 248, 14);
                            player.EmitLocked("Client:SpawnArea:setCharClothes", 8, 153, 0);
                            player.EmitLocked("Client:SpawnArea:setCharClothes", 6, 25, 0);
                            player.EmitLocked("Client:SpawnArea:setCharAccessory", 2, 2, 0);
                            player.EmitLocked("Client:SpawnArea:setCharClothes", 3, 43, 0);

                            player.EmitLocked("Client:SpawnArea:setCharAccessory", 7, 0, 0);
                            player.EmitLocked("Client:SpawnArea:setCharClothes", 1, 0, 0);
                            player.EmitLocked("Client:SpawnArea:setCharClothes", 9, 0, 0);
                            HUDHandler.SendNotification(player, 2, 2500, "Du hast deine Arbeitsklamotten angezogen.");
                            player.SetData("HasVUCClothesOn", true);
                            Characters.SetCharacterArmor(charId, 100);
                        }
                        else
                        {
                            player.DeleteData("HasVUCClothesOn");
                            Characters.SetCharacterCorrectClothes(player);
                            HUDHandler.SendNotification(player, 2, 2500, "Du hast deine Arbeitsklamotten ausgezogen.");
                        }
                    }
                    else
                    {
                        HUDHandler.SendNotification(player, 2, 2500, "Du darfst den Kleiderschrank nicht nutzen!");
                    }
                    return;
                }

                var tattooShop = ServerTattooShops.ServerTattooShops_.ToList().FirstOrDefault(x => x.owner != 0 && player.Position.IsInRange(new Position(x.pedX, x.pedY, x.pedZ), 2.5f));
                if (tattooShop != null && !player.IsInVehicle)
                {
                    ShopHandler.openTattooShop((ClassicPlayer)player, tattooShop);
                    return;
                }

                if (player.Position.IsInRange(RobberyHandler.bankRobPosition, 2f) || player.Position.IsInRange(RobberyHandler.bankExitPosition, 2f))
                {
                    RobberyHandler.EnterExitBank((ClassicPlayer)player);
                    return;
                }

                var bankRobPosGold = RobberyHandler.bankPickUpPositions.ToList().FirstOrDefault(x => player.Position.IsInRange(x.position, 1f));
                if (bankRobPosGold != null)
                {
                    RobberyHandler.pickUpBankGold((ClassicPlayer)player, bankRobPosGold);
                    return;
                }

                if (player.Position.IsInRange(RobberyHandler.jeweleryRobPosition, 2f))
                {
                    RobberyHandler.robJewelery((ClassicPlayer)player);
                    return;
                }

                var laborEntry = ServerFactions.ServerFactions_.FirstOrDefault(x => player.Position.IsInRange(x.laborPos, 2.5f) && !x.isLaborLocked);
                if (laborEntry != null)
                {
                    player.Dimension = laborEntry.id;
                    player.Position = ServerFactions.GetLaborExitPosition(laborEntry.id);
                    return;
                }

                if (player.Position.IsInRange(Constants.Positions.weedLabor_ExitPosition, 2.5f) && player.Dimension != 0)
                {
                    Server_Factions faction = ServerFactions.ServerFactions_.ToList().FirstOrDefault(x => x.id == player.Dimension);
                    if (faction == null || faction.laborPos == new Position(0, 0, 0) || faction.isLaborLocked) return;
                    player.Position = faction.laborPos;
                    player.Dimension = 0;
                    return;
                }

                if (player.Position.IsInRange(Constants.Positions.weedLabor_InvPosition, 2.5f) && player.Dimension != 0 && ServerFactions.GetCharacterFactionId(User.GetPlayerOnline(player)) == player.Dimension)
                {
                    LaborHandler.openLabor((ClassicPlayer)player);
                    return;
                }

                Server_Storages storageEntry = ServerStorages.ServerStorages_.ToList().FirstOrDefault(x => player.Position.IsInRange(x.entryPos, 2f) && !x.isLocked);
                if (storageEntry != null && !player.IsInVehicle)
                {
                    player.Dimension = storageEntry.id;
                    player.Position = Constants.Positions.storage_ExitPosition;
                    return;
                }

                if (player.Position.IsInRange(Constants.Positions.storage_ExitPosition, 2f) && player.Dimension != 0)
                {
                    Server_Storages storage = ServerStorages.ServerStorages_.ToList().FirstOrDefault(x => x.id == player.Dimension);
                    if (storage == null || storage.entryPos == new Position(0, 0, 0) || storage.isLocked) return;
                    player.Position = storage.entryPos;
                    player.Dimension = 0;
                    return;
                }

                if (player.Position.IsInRange(Constants.Positions.storage_InvPosition, 2.5f) && player.Dimension != 0)
                {
                    StorageHandler.openStorage((ClassicPlayer)player);
                    return;
                }
                if (player.Position.IsInRange(Constants.Positions.storage_LSPDInvPosition, 2.5f) && ServerStorages.ExistStorage(player.Dimension))
                {
                    StorageHandler.openStorage2((ClassicPlayer)player);
                    return;
                }

                if (player.Position.IsInRange(Constants.Positions.dynasty8_positionStorage, 2f))
                {
                    player.Emit("Client:Dynasty8:create", "storages", ServerStorages.GetAccountStorages(User.GetPlayerOnline(player)), ServerStorages.GetFreeStorages());
                    return;
                }
            }
        }

        [AsyncClientEvent("Server:KeyHandler:PressU")]
        public async Task PressU(IPlayer player)
        {
            try
            {
                lock (player)
                {
                    if (player == null || !player.Exists) return;
                    int charId = User.GetPlayerOnline(player);
                    if (charId <= 0) return;
                    if (player.HasPlayerHandcuffs() || player.HasPlayerRopeCuffs()) { HUDHandler.SendNotification(player, 3, 5000, "Wie willst du das mit Handschellen/Fesseln machen?"); return; }

                    /*ClassicColshape serverDoorLockCol = (ClassicColshape)ServerDoors.ServerDoorsLockColshapes_.FirstOrDefault(x => ((ClassicColshape)x).IsInRange((ClassicPlayer)player));
                    if (serverDoorLockCol != null)
                    {
                        var doorColData = ServerDoors.ServerDoors_.FirstOrDefault(x => x.id == (int)serverDoorLockCol.GetColShapeId());
                        if (doorColData != null)
                        {
                            string doorKey = doorColData.doorKey;
                            string doorKey2 = doorColData.doorKey2;
                            if (doorKey == null || doorKey2 == null) return;
                            if (!CharactersInventory.ExistCharacterItem(charId, doorKey, "schluessel") && !CharactersInventory.ExistCharacterItem(charId, doorKey, "schluessel") && !CharactersInventory.ExistCharacterItem(charId, doorKey2, "schluessel") && !CharactersInventory.ExistCharacterItem(charId, doorKey2, "schluessel")) return;

                            if (!doorColData.state) { HUDHandler.SendNotification(player, 4, 1500, "Tür abgeschlossen."); }
                            else { HUDHandler.SendNotification(player, 2, 1500, "Tür aufgeschlossen."); }
                            doorColData.state = !doorColData.state;
                            Alt.EmitAllClients("Client:DoorManager:ManageDoor", doorColData.hash, new Position(doorColData.posX, doorColData.posY, doorColData.posZ), (bool)doorColData.state);
                            return;
                        }
                    }*/

                    if (player.Dimension >= 5000)
                    {
                        int houseInteriorCount = ServerHouses.GetMaxInteriorsCount();
                        for (var i = 1; i <= houseInteriorCount; i++)
                        {
                            if (player.Dimension >= 5000 && player.Dimension < 10000 && player.Position.IsInRange(ServerHouses.GetInteriorExitPosition(i), 2f))
                            {
                                //Hotel abschließen / aufschließen
                                if (player.Dimension - 5000 <= 0) continue;
                                int apartmentId = player.Dimension - 5000;
                                int hotelId = ServerHotels.GetHotelIdByApartmentId(apartmentId);
                                if (hotelId <= 0 || apartmentId <= 0) continue;
                                if (!ServerHotels.ExistHotelApartment(hotelId, apartmentId)) { HUDHandler.SendNotification(player, 3, 5000, "Ein unerwarteter Fehler ist aufgetreten [HOTEL-001]."); return; }
                                if (ServerHotels.GetApartmentOwner(hotelId, apartmentId) != charId) { HUDHandler.SendNotification(player, 3, 5000, "Du hast keinen Schlüssel."); return; }
                                HotelHandler.LockHotel(player, hotelId, apartmentId);
                                return;
                            }
                            else if (player.Dimension >= 10000 && player.Position.IsInRange(ServerHouses.GetInteriorExitPosition(i), 2f))
                            {
                                //Haus abschließen / aufschließen
                                if (player.Dimension - 10000 <= 0) continue;
                                int houseId = player.Dimension - 10000;
                                if (houseId <= 0) continue;
                                if (!ServerHouses.ExistHouse(houseId)) { HUDHandler.SendNotification(player, 3, 5000, "Ein unerwarteter Fehler ist aufgetreten [HOUSE-001]."); return; }
                                if (ServerHouses.GetHouseOwner(houseId) != charId && !ServerHouses.IsCharacterRentedInHouse(charId, houseId)) { HUDHandler.SendNotification(player, 3, 5000, "Dieses Haus gehört nicht dir und / oder du bist nicht eingemietet."); return; }
                                HouseHandler.LockHouse(player, houseId);
                                return;
                            }
                        }
                    }

                    var houseEntrance = ServerHouses.ServerHouses_.FirstOrDefault(x => ((ClassicColshape)x.entranceShape).IsInRange((ClassicPlayer)player));
                    if (houseEntrance != null)
                    {
                        HouseHandler.LockHouse(player, houseEntrance.id);
                    }


                    if (player == null || !player.Exists || User.GetPlayerOnline(player) <= 0) return;
                    var laborEntry = ServerFactions.ServerFactions_.FirstOrDefault(x => x.laborPos.IsInRange(player.Position, 2.5f));
                    if (laborEntry != null && !player.IsInVehicle && ServerFactions.IsCharacterInAnyFaction(User.GetPlayerOnline(player)) && ServerFactions.GetCharacterFactionId(User.GetPlayerOnline(player)) == laborEntry.id)
                    {
                        if (laborEntry.isLaborLocked) HUDHandler.SendNotification(player, 2, 2500, "Du hast das Labor aufgeschlossen.");
                        else HUDHandler.SendNotification(player, 4, 2500, "Du hast das Labor abgeschlossen.");
                        ServerFactions.SetLaborLocked(laborEntry.id, !laborEntry.isLaborLocked);
                        return;
                    }

                    if (player.Position.IsInRange(Constants.Positions.weedLabor_ExitPosition, 2.5f) && player.Dimension != 0 && ServerFactions.IsCharacterInAnyFaction(User.GetPlayerOnline(player)) && ServerFactions.GetCharacterFactionId(User.GetPlayerOnline(player)) == player.Dimension)
                    {
                        Server_Factions labor = ServerFactions.ServerFactions_.ToList().FirstOrDefault(x => x.id == player.Dimension);
                        if (labor == null) return;
                        if (labor.isLaborLocked) HUDHandler.SendNotification(player, 2, 2500, "Du hast das Labor aufgeschlossen.");
                        else HUDHandler.SendNotification(player, 4, 2500, "Du hast das Labor abgeschlossen.");
                        ServerFactions.SetLaborLocked(labor.id, !labor.isLaborLocked);
                        return;
                    }

                    Server_Storages storage = ServerStorages.ServerStorages_.FirstOrDefault(x => player.Position.IsInRange(x.entryPos, 2f) && (x.owner == User.GetPlayerOnline(player) || x.secondOwner == User.GetPlayerOnline(player) || x.factionid == ServerFactions.GetCharacterFactionId(charId)));
                    if (storage != null && !player.IsInVehicle && player.Dimension == 0)
                    {
                        storage.isLocked = !storage.isLocked;
                        if (storage.isLocked) HUDHandler.SendNotification(player, 4, 2500, "[LaVie Lagersystem] <br><br> Du hast die Lagerhalle abgeschlossen.");
                        else HUDHandler.SendNotification(player, 2, 2500, "[LaVie Lagersystem] <br><br> Du hast die Lagerhalle aufgeschlossen.");
                        return;
                    }

                    if (player.Position.IsInRange(Constants.Positions.storage_ExitPosition, 2f) && player.Dimension != 0 && (ServerStorages.GetOwner(player.Dimension) == User.GetPlayerOnline(player) || ServerStorages.GetSecondOwner(player.Dimension) == User.GetPlayerOnline(player)))
                    {
                        Server_Storages storages = ServerStorages.ServerStorages_.FirstOrDefault(x => x.id == player.Dimension);
                        if (storages == null) return;
                        storages.isLocked = !storages.isLocked;
                        if (storages.isLocked) HUDHandler.SendNotification(player, 4, 2500, "[LaVie Lagersystem] <br><br> Du hast die Lagerhalle abgeschlossen.");
                        else HUDHandler.SendNotification(player, 2, 2500, "[LaVie Lagersystem] <br><br> Du hast die Lagerhalle aufgeschlossen.");
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }



        public static bool isVerbandused = false;
        public static bool isschwestused = false;

        [AsyncClientEvent("Server:KeyHandler:PressF6")]
        public async Task PressF6(IPlayer player)
        {
            try
            {
                if (player == null && !player.Exists && player.GetCharacterMetaId() == 0 && Characters.GetCharacterAccountId((int)player.GetCharacterMetaId()) == 0 && player.IsInVehicle == true) return;
                if (isVerbandused == true) return;
                if (!CharactersInventory.ExistCharacterItem2((int)player.GetCharacterMetaId(), "Verbandskasten")) return;

                HUDHandler.SendNotification(player, 2, 3000, "Du hast ein Verbandskasten benutzt. <br><br> -1 Verbandskasten");
                InventoryHandler.InventoryAnimation(player, "verband", 3000);
                isVerbandused = true;
                await Task.Delay(3000);
                isVerbandused = false;
                Characters.SetCharacterHealth((int)player.GetCharacterMetaId(), 200);
                player.Health = 200;
                player.EmitLocked("Client:HUD:UpdateDesire", Characters.GetCharacterArmor((int)player.GetCharacterMetaId()), Characters.GetCharacterHealth((int)player.GetCharacterMetaId()), Characters.GetCharacterHunger((int)player.GetCharacterMetaId()), Characters.GetCharacterThirst((int)player.GetCharacterMetaId())); //HUD updaten
                CharactersInventory.RemoveCharacterItemAmount2((int)player.GetCharacterMetaId(), "Verbandskasten", 1);
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [AsyncClientEvent("Server:KeyHandler:PressF7")]
        public async Task PressF7(IPlayer player)
        {
            try
            {
                if (player == null && !player.Exists && player.GetCharacterMetaId() == 0 && Characters.GetCharacterAccountId((int)player.GetCharacterMetaId()) == 0 && player.IsInVehicle == true) return;
                if (isschwestused == true) return;
                if (CharactersInventory.ExistCharacterItem2((int)player.GetCharacterMetaId(), "Schutzweste")) 
                {
                    HUDHandler.SendNotification(player, 2, 3000, "Du hast ein Schutzweste benutzt. <br><br> -1 Schutzweste");
                    InventoryHandler.InventoryAnimation(player, "weste", 3000);
                    isschwestused = true;
                    await Task.Delay(3000);
                    isschwestused = false;
                    Characters.SetCharacterArmor((int)player.GetCharacterMetaId(), 100);
                    player.Armor = 100;
                    player.EmitLocked("Client:HUD:UpdateDesire", Characters.GetCharacterArmor((int)player.GetCharacterMetaId()), Characters.GetCharacterHealth((int)player.GetCharacterMetaId()), Characters.GetCharacterHunger((int)player.GetCharacterMetaId()), Characters.GetCharacterThirst((int)player.GetCharacterMetaId())); //HUD updaten
                    CharactersInventory.RemoveCharacterItemAmount2((int)player.GetCharacterMetaId(), "Schutzweste", 1);
                } 
                else if (CharactersInventory.ExistCharacterItem2((int)player.GetCharacterMetaId(), "Beamtenschutzweste"))
                {
                    HUDHandler.SendNotification(player, 2, 3000, "Du hast ein Beamtenschutzweste benutzt. <br><br> -1 Beamtenschutzweste");
                    InventoryHandler.InventoryAnimation(player, "weste", 3000);
                    isschwestused = true;
                    await Task.Delay(3000);
                    isschwestused = false;
                    Characters.SetCharacterArmor((int)player.GetCharacterMetaId(), 100);
                    player.Armor = 100;
                    if (Characters.GetCharacterGender((int)player.GetCharacterMetaId())) player.EmitLocked("Client:SpawnArea:setCharClothes", 9, 17, 2);
                    else player.EmitLocked("Client:SpawnArea:setCharClothes", 9, 57, 0); // Schutzweste
                    CharactersInventory.RemoveCharacterItemAmount2((int)player.GetCharacterMetaId(), "Beamtenschutzweste", 1);
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }


        [AsyncClientEvent("Server:KeyHandler:PressO")]
        public async Task PressO(ClassicPlayer player)
        {
            try
            {
                if (player == null && !player.Exists && player.GetCharacterMetaId() == 0 && Characters.GetCharacterAccountId((int)player.GetCharacterMetaId()) == 0) return;
                if (ServerFactions.GetCharacterFactionId((int)player.GetCharacterMetaId()) != 2) return;
                if (player.CurrentWeapon == 0) return;
                if (player.Gummigeschoss)
                {
                    player.Gummigeschoss = false;
                    HUDHandler.SendNotification(player, 4, 5000, "Du hast die Gummigeschoss - Funktion deaktiviert");
                } else if (!player.Gummigeschoss)
                {
                    player.Gummigeschoss = true;
                    HUDHandler.SendNotification(player, 2, 5000, "Du hast die Gummigeschoss - Funktion aktiviert");
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

    }
}
