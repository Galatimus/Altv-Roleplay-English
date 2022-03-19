using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using Altv_Roleplay.Factories;
using Altv_Roleplay.Model;
using Altv_Roleplay.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altv_Roleplay.Handler
{
    class RobberyHandler : IScript
    {
        #region Pacific Bank Robbery
        public static Position bankRobPosition = new Position(254.28131f, 225.389f, 101.8689f);
        public static Position bankExitPosition = new Position(252.31f, 220.21f, 101.66f);
        public static List<bankPickUpPosition> bankPickUpPositions = new List<bankPickUpPosition>
        {
            new bankPickUpPosition { position = new Position(258.1055f, 217.97803f, 101.666f)},
            new bankPickUpPosition { position = new Position(260.545f, 217.252f, 101.666f)},
            new bankPickUpPosition { position = new Position(259.3978f, 214.021f, 101.666f)},
            new bankPickUpPosition { position = new Position(275.063f, 215.037f, 101.666f)},
            new bankPickUpPosition { position = new Position(262.180f, 213.059f, 101.666f)},
            new bankPickUpPosition { position = new Position(264.250f, 212.096f, 101.666f)},
            new bankPickUpPosition { position = new Position(265.912f, 213.665f, 101.666f)},
            new bankPickUpPosition { position = new Position(265.542f, 215.657f, 101.666f)},
            new bankPickUpPosition { position = new Position(263.39f, 216.435f, 101.666f)},

        };
        public static bool isBankCurrentlyRobbing = false;
        public static bool isBankOpened = false;

        internal static void EnterExitBank(ClassicPlayer player)
        {
            try
            {
                if (player == null || !player.Exists || player.CharacterId <= 0) return;
                if (player.Position.IsInRange(bankRobPosition, 2f))
                {
                    if (isBankOpened == false) { HUDHandler.SendNotification(player, 3, 2500, "Der Tresor ist verschlossen, öffne diesen erst mit einem Schweißgerät."); return; }
                    player.Position = bankExitPosition;
                }
                else if (player.Position.IsInRange(bankExitPosition, 2f))
                    player.Position = bankRobPosition;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        internal static void pickUpBankGold(ClassicPlayer player, bankPickUpPosition bankRobPosGold)
        {
            try
            {
                if (player == null || !player.Exists || player.CharacterId <= 0 || bankRobPosGold == null) return;
                if (!isBankOpened) { HUDHandler.SendNotification(player, 3, 2500, "Der Tresor wurde noch nicht aufgeschweißt."); return; }
                if (bankRobPosGold.isPickedUp) { HUDHandler.SendNotification(player, 3, 2500, "Dieses Fach wurde bereits leer geräumt oder ist verschlossen."); return; }
                int randomAnzahl = new Random().Next(1000, 4000);
                float weight = ServerItems.GetItemWeight("Schwarzgeld") * randomAnzahl;
                if (CharactersInventory.GetCharacterItemWeight(player.CharacterId, "inventory") + weight <= 15f)
                    CharactersInventory.AddCharacterItem(player.CharacterId, "Schwarzgeld", randomAnzahl, "inventory");
                else if (CharactersInventory.GetCharacterItemWeight(player.CharacterId, "backpack") + weight <= Characters.GetCharacterBackpackSize(Characters.GetCharacterBackpack(player.CharacterId)))
                    CharactersInventory.AddCharacterItem(player.CharacterId, "Schwarzgeld", randomAnzahl, "backpack");
                else { HUDHandler.SendNotification(player, 3, 2500, "Du hast nicht genügend Platz für die Schwarzgeld."); return; }
                bankRobPosGold.isPickedUp = true;
                player.Emit("Client:Inventory:PlayAnimation", "anim@narcotics@trash", "drop_front", 500, 1, false);
                HUDHandler.SendNotification(player, 2, 1000, $"Du hast {randomAnzahl} Schwarzgeld erbeutet.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        internal static async Task breakUpBank(ClassicPlayer player)
        {
            try
            {
                if (player == null || !player.Exists || player.CharacterId <= 0) return;
                if (isBankOpened || isBankCurrentlyRobbing) { HUDHandler.SendNotification(player, 3, 2000, "Der Tresor wird bereits aufgeschweißt oder ist bereits offen."); return; }
                isBankCurrentlyRobbing = true;
                HUDHandler.SendNotification(player, 1, 100000, "Du schweißt den Tresor auf...");
                player.EmitLocked("Client:Inventory:PlayAnimation", "amb@world_human_welding@male@idle_a", "idle_a", 600000, 1, false);

                foreach (var PDmember in Alt.GetAllPlayers().ToList().Where(x => x != null && x.Exists && x.GetCharacterMetaId() > 0 && ServerFactions.IsCharacterInAnyFaction((int)x.GetCharacterMetaId()) && ServerFactions.GetCharacterFactionId((int)x.GetCharacterMetaId()) == 2))
                    HUDHandler.SendNotification(PDmember, 3, 5000, "Ein Einbruch in die Staatsbank wurde gemeldet.");

                await Task.Delay(100000);
                if (player == null || !player.Exists || player.CharacterId <= 0 || !player.Position.IsInRange(bankRobPosition, 5f))
                {
                    isBankCurrentlyRobbing = false;
                    isBankOpened = false;
                    return;
                }

                isBankCurrentlyRobbing = false;
                isBankOpened = true;
                foreach (var goldPos in bankPickUpPositions)
                    goldPos.isPickedUp = false;

                player.EmitLocked("Client:Inventory:StopAnimation");
                HUDHandler.SendNotification(player, 2, 2000, "Der Tresor ist offen, gehe hinein, du hast 5 Minuten Zeit.");
                await Task.Delay(300000); //reset bank
                foreach (var goldPos in bankPickUpPositions)
                    goldPos.isPickedUp = true;
                isBankOpened = false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        #endregion

        #region ATM

        public static bool isATMCurrentlyRobbing = false;
        public static bool isATMOpened = false;

        internal static async Task breakUpATM(ClassicPlayer player, Position atmpos)
        {
            try
            {
                if (player == null || !player.Exists || player.CharacterId <= 0) return;
                var atmPos2 = ServerATM.ServerATM_.FirstOrDefault(x => player.Position.IsInRange(new Position(x.posX, x.posY, x.posZ), 1f));
                //if (atmPos2.isrobbed == 1) { HUDHandler.SendNotification(player, 3, 5000, "Der ATM wurde bereits ausgeraubt"); return; }

                if (atmPos2.isrobbed == 1)
                {
                    HUDHandler.SendNotification(player, 4, 2500, "Der Bankautomaten wurde bereits ausgeraubt.");
                    return;
                }

                /*if (ServerFactions.GetFactionDutyMemberCount(2) + ServerFactions.GetFactionDutyMemberCount(12) < 1) // Wider (1) auf 4 machen
                {
                    HUDHandler.SendNotification(player, 4, 2500, "Es sind weniger als 4 Polizisten im Staat.");
                    return;
                }*/

                ServerATM.SetRobbed(ServerATM.GetATMIdbypos(atmpos), 1);

                ServerFactions.AddNewFactionDispatch(0, 2, $"Aktiver Bankautomaten Raub", player.Position);
                ServerFactions.AddNewFactionDispatch(0, 12, $"Aktiver Bankautomaten Raub", player.Position);

                HUDHandler.SendNotification(player, 1, 20000, "Du schweißt den Bankautomaten auf...");
                player.EmitLocked("Client:Inventory:PlayAnimation", "amb@world_human_welding@male@idle_a", "idle_a", 240000, 1, false); //Animation 

                foreach (var PDmember in Alt.GetAllPlayers().ToList().Where(x => x != null && x.Exists && x.GetCharacterMetaId() > 0 && ServerFactions.IsCharacterInAnyFaction((int)x.GetCharacterMetaId()) && ServerFactions.GetCharacterFactionId((int)x.GetCharacterMetaId()) == 2))
                    HUDHandler.SendNotification(PDmember, 3, 20000, "Ein Bankautomaten meldet einen stillen Alarm.");
                ServerFactions.AddNewFactionDispatch(0, 1, "Ein Bankautomaten meldet einen stillen Alarm.", atmpos);

                await Task.Delay(17500); //Zeit bis offen 1
                if (!player.Position.IsInRange(atmpos, 2f))
                {
                    HUDHandler.SendNotification(player, 4, 5000, "Du hast dich vom Bankautomaten entfernt - Abbruch");
                    foreach (var PDmember in Alt.GetAllPlayers().ToList().Where(x => x != null && x.Exists && x.GetCharacterMetaId() > 0 && ServerFactions.IsCharacterInAnyFaction((int)x.GetCharacterMetaId()) && ServerFactions.GetCharacterFactionId((int)x.GetCharacterMetaId()) == 2))
                        HUDHandler.SendNotification(PDmember, 3, 5000, "Der Bankautomaten konnte nicht aufgebrochen werden!");
                    ServerATM.SetRobbed(ServerATM.GetATMIdbypos(atmpos), 0);
                    return;
                }
                await Task.Delay(17500); //Zeit bis offen 2
                if (!player.Position.IsInRange(atmpos, 2f))
                {
                    HUDHandler.SendNotification(player, 4, 5000, "Du hast dich vom Bankautomaten entfernt - Abbruch");
                    foreach (var PDmember in Alt.GetAllPlayers().ToList().Where(x => x != null && x.Exists && x.GetCharacterMetaId() > 0 && ServerFactions.IsCharacterInAnyFaction((int)x.GetCharacterMetaId()) && ServerFactions.GetCharacterFactionId((int)x.GetCharacterMetaId()) == 2))
                        HUDHandler.SendNotification(PDmember, 3, 5000, "Der Bankautomaten konnte nicht aufgebrochen werden!");
                    ServerATM.SetRobbed(ServerATM.GetATMIdbypos(atmpos), 0);
                    return;
                }
                await Task.Delay(17500); //Zeit bis offen 3
                if (!player.Position.IsInRange(atmpos, 2f))
                {
                    HUDHandler.SendNotification(player, 4, 5000, "Du hast dich vom Bankautomaten entfernt - Abbruch");
                    foreach (var PDmember in Alt.GetAllPlayers().ToList().Where(x => x != null && x.Exists && x.GetCharacterMetaId() > 0 && ServerFactions.IsCharacterInAnyFaction((int)x.GetCharacterMetaId()) && ServerFactions.GetCharacterFactionId((int)x.GetCharacterMetaId()) == 2))
                        HUDHandler.SendNotification(PDmember, 3, 5000, "Der Bankautomaten konnte nicht aufgebrochen werden!");
                    ServerATM.SetRobbed(ServerATM.GetATMIdbypos(atmpos), 0);
                    return;
                }
                await Task.Delay(17500); //Zeit bis offen 4
                if (!player.Position.IsInRange(atmpos, 2f))
                {
                    HUDHandler.SendNotification(player, 4, 5000, "Du hast dich vom Bankautomaten entfernt - Abbruch");
                    foreach (var PDmember in Alt.GetAllPlayers().ToList().Where(x => x != null && x.Exists && x.GetCharacterMetaId() > 0 && ServerFactions.IsCharacterInAnyFaction((int)x.GetCharacterMetaId()) && ServerFactions.GetCharacterFactionId((int)x.GetCharacterMetaId()) == 2))
                        HUDHandler.SendNotification(PDmember, 3, 5000, "Der Bankautomaten konnte nicht aufgebrochen werden!");
                    ServerATM.SetRobbed(ServerATM.GetATMIdbypos(atmpos), 0);
                    return;
                }
                await Task.Delay(17500); //Zeit bis offen 1
                if (!player.Position.IsInRange(atmpos, 2f))
                {
                    HUDHandler.SendNotification(player, 4, 5000, "Du hast dich vom Bankautomaten entfernt - Abbruch");
                    foreach (var PDmember in Alt.GetAllPlayers().ToList().Where(x => x != null && x.Exists && x.GetCharacterMetaId() > 0 && ServerFactions.IsCharacterInAnyFaction((int)x.GetCharacterMetaId()) && ServerFactions.GetCharacterFactionId((int)x.GetCharacterMetaId()) == 2))
                        HUDHandler.SendNotification(PDmember, 3, 5000, "Der Bankautomaten konnte nicht aufgebrochen werden!");
                    ServerATM.SetRobbed(ServerATM.GetATMIdbypos(atmpos), 0);
                    return;
                }
                await Task.Delay(17500); //Zeit bis offen 2
                if (!player.Position.IsInRange(atmpos, 2f))
                {
                    HUDHandler.SendNotification(player, 4, 5000, "Du hast dich vom Bankautomaten entfernt - Abbruch");
                    foreach (var PDmember in Alt.GetAllPlayers().ToList().Where(x => x != null && x.Exists && x.GetCharacterMetaId() > 0 && ServerFactions.IsCharacterInAnyFaction((int)x.GetCharacterMetaId()) && ServerFactions.GetCharacterFactionId((int)x.GetCharacterMetaId()) == 2))
                        HUDHandler.SendNotification(PDmember, 3, 5000, "Der Bankautomaten konnte nicht aufgebrochen werden!");
                    ServerATM.SetRobbed(ServerATM.GetATMIdbypos(atmpos), 0);
                    return;
                }
                await Task.Delay(17500); //Zeit bis offen 3
                if (!player.Position.IsInRange(atmpos, 2f))
                {
                    HUDHandler.SendNotification(player, 4, 5000, "Du hast dich vom Bankautomaten entfernt - Abbruch");
                    foreach (var PDmember in Alt.GetAllPlayers().ToList().Where(x => x != null && x.Exists && x.GetCharacterMetaId() > 0 && ServerFactions.IsCharacterInAnyFaction((int)x.GetCharacterMetaId()) && ServerFactions.GetCharacterFactionId((int)x.GetCharacterMetaId()) == 2))
                        HUDHandler.SendNotification(PDmember, 3, 5000, "Der Bankautomaten konnte nicht aufgebrochen werden!");
                    ServerATM.SetRobbed(ServerATM.GetATMIdbypos(atmpos), 0);
                    return;
                }
                await Task.Delay(17500); //Zeit bis offen 4
                if (!player.Position.IsInRange(atmpos, 2f))
                {
                    HUDHandler.SendNotification(player, 4, 5000, "Du hast dich vom Bankautomaten entfernt - Abbruch");
                    foreach (var PDmember in Alt.GetAllPlayers().ToList().Where(x => x != null && x.Exists && x.GetCharacterMetaId() > 0 && ServerFactions.IsCharacterInAnyFaction((int)x.GetCharacterMetaId()) && ServerFactions.GetCharacterFactionId((int)x.GetCharacterMetaId()) == 2))
                        HUDHandler.SendNotification(PDmember, 3, 5000, "Der Bankautomaten konnte nicht aufgebrochen werden!");
                    ServerATM.SetRobbed(ServerATM.GetATMIdbypos(atmpos), 0);
                    return;
                }// Insgesamt 240000

                player.EmitLocked("Client:Inventory:StopAnimation");
                int rnd = new Random().Next(85, 1500); //Wie viel geld soll der ATM geben? //FASTCHANGE
                HUDHandler.SendNotification(player, 1, 4000, $"Der Bankautomaten ist geöffnet und du erhälst {rnd}$ aus dem Bankautomaten");
                foreach (var PDmember in Alt.GetAllPlayers().ToList().Where(x => x != null && x.Exists && x.GetCharacterMetaId() > 0 && ServerFactions.IsCharacterInAnyFaction((int)x.GetCharacterMetaId()) && ServerFactions.GetCharacterFactionId((int)x.GetCharacterMetaId()) == 2))
                    HUDHandler.SendNotification(PDmember, 3, 5000, "Der Bankautomaten wurde aufgebrochen!");
                ServerFactions.AddNewFactionDispatch(0, 1, "Der Bankautomaten wurde aufgebrochen!", atmpos);

                CharactersInventory.AddCharacterItem(player.CharacterId, "Bargeld", rnd, "inventory");

                await Task.Delay(3600000); //60 minuten 3600000

                ServerATM.SetRobbed(ServerATM.GetATMIdbypos(atmpos), 0);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        #endregion

        //todo
        #region LSPD Robbery
        public static Position LSPDRobPosition = new Position(254.28131f, 225.389f, 101.8689f);
        public static List<LSPDPickUpPosition> LSPDPickUpPositions = new List<LSPDPickUpPosition>
        {
            new LSPDPickUpPosition { position = new Position(258.1055f, 217.97803f, 101.666f)},
            new LSPDPickUpPosition { position = new Position(260.545f, 217.252f, 101.666f)},
            new LSPDPickUpPosition { position = new Position(259.3978f, 214.021f, 101.666f)},
            new LSPDPickUpPosition { position = new Position(275.063f, 215.037f, 101.666f)},
            new LSPDPickUpPosition { position = new Position(275.063f, 215.037f, 101.666f)},

        };
        public static bool isLSPDCurrentlyRobbing = false;
        public static bool isLSPDOpened = false;

/*        internal static void EnterExitLSPD(ClassicPlayer player)
        {
            try
            {
                if (player == null || !player.Exists || player.CharacterId <= 0) return;
                if (player.Position.IsInRange(LSPDRobPosition, 2f))
                {
                    if (isLSPDOpened == false) { HUDHandler.SendNotification(player, 3, 2500, "Der Tresor ist verschlossen, öffne diesen erst mit einem Schweißgerät."); return; }
                    player.Position = LSPDExitPosition;
                }
                else if (player.Position.IsInRange(LSPDExitPosition, 2f))
                    player.Position = LSPDRobPosition;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
*/
        internal static void pickUpLSPDGold(ClassicPlayer player, LSPDPickUpPosition LSPDRobPosGold)
        {
            try
            {
                if (player == null || !player.Exists || player.CharacterId <= 0 || LSPDRobPosGold == null) return;
                if (!isLSPDOpened) { HUDHandler.SendNotification(player, 3, 2500, "Der Tresor wurde noch nicht aufgeschweißt."); return; }
                if (LSPDRobPosGold.isPickedUp) { HUDHandler.SendNotification(player, 3, 2500, "Dieses Fach wurde bereits leer geräumt oder ist verschlossen."); return; }
                int randomAnzahl = new Random().Next(2500, 7500);
                float weight = ServerItems.GetItemWeight("Schwarzgeld") * randomAnzahl;
                if (CharactersInventory.GetCharacterItemWeight(player.CharacterId, "inventory") + weight <= 15f)
                    CharactersInventory.AddCharacterItem(player.CharacterId, "Schwarzgeld", randomAnzahl, "inventory");
                else if (CharactersInventory.GetCharacterItemWeight(player.CharacterId, "backpack") + weight <= Characters.GetCharacterBackpackSize(Characters.GetCharacterBackpack(player.CharacterId)))
                    CharactersInventory.AddCharacterItem(player.CharacterId, "Schwarzgeld", randomAnzahl, "backpack");
                else { HUDHandler.SendNotification(player, 3, 2500, "Du hast nicht genügend Platz für die Schwarzgeld."); return; }
                LSPDRobPosGold.isPickedUp = true;
                player.Emit("Client:Inventory:PlayAnimation", "anim@narcotics@trash", "drop_front", 500, 1, false);
                HUDHandler.SendNotification(player, 2, 1000, $"Du hast {randomAnzahl} Schwarzgeld erbeutet.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        internal static async Task breakUpLSPD(ClassicPlayer player)
        {
            try
            {
                if (player == null || !player.Exists || player.CharacterId <= 0) return;
                if (isLSPDOpened || isLSPDCurrentlyRobbing) { HUDHandler.SendNotification(player, 3, 2000, "Der Tresor wird bereits aufgeschweißt oder ist bereits offen."); return; }
                isLSPDCurrentlyRobbing = true;
                HUDHandler.SendNotification(player, 1, 100000, "Du schweißt die Waffenkammer auf...");
                player.EmitLocked("Client:Inventory:PlayAnimation", "amb@world_human_welding@male@idle_a", "idle_a", 600000, 1, false);

                foreach (var PDmember in Alt.GetAllPlayers().ToList().Where(x => x != null && x.Exists && x.GetCharacterMetaId() > 0 && ServerFactions.IsCharacterInAnyFaction((int)x.GetCharacterMetaId()) && ServerFactions.GetCharacterFactionId((int)x.GetCharacterMetaId()) == 2))
                    HUDHandler.SendNotification(PDmember, 3, 5000, "Ein Einbruch in die StaatsLSPD wurde gemeldet.");

                await Task.Delay(100000);
                if (player == null || !player.Exists || player.CharacterId <= 0 || !player.Position.IsInRange(LSPDRobPosition, 5f))
                {
                    isLSPDCurrentlyRobbing = false;
                    isLSPDOpened = false;
                    return;
                }

                isLSPDCurrentlyRobbing = false;
                isLSPDOpened = true;

                ClassicColshape serverDoorLockCol = (ClassicColshape)ServerDoors.ServerDoorsLockColshapes_.FirstOrDefault(x => ((ClassicColshape)x).IsInRange((ClassicPlayer)player));
                if (serverDoorLockCol != null)
                {
                    var doorColData = ServerDoors.ServerDoors_.FirstOrDefault(x => x.id == (int)serverDoorLockCol.GetColShapeId());
                    if (doorColData != null)
                    {

                        if (!doorColData.state) { HUDHandler.SendNotification(player, 4, 1500, "Tür bereits offen."); }
                        else { HUDHandler.SendNotification(player, 2, 1500, "Tür erfolgreich aufberochen"); }
                        doorColData.state = true;
                        Alt.EmitAllClients("Client:DoorManager:ManageDoor", doorColData.hash, new Position(doorColData.posX, doorColData.posY, doorColData.posZ), (bool)doorColData.state);
                    }

                    foreach (var goldPos in LSPDPickUpPositions)
                        goldPos.isPickedUp = false;

                    player.EmitLocked("Client:Inventory:StopAnimation");
                    HUDHandler.SendNotification(player, 2, 300000, "Die Waffenkammer ist offen, gehe hinein, du hast 5 Minuten Zeit.");
                    await Task.Delay(300000); //reset LSPD
                    foreach (var goldPos in LSPDPickUpPositions)
                        goldPos.isPickedUp = true;
                        isLSPDOpened = false;
                    HUDHandler.SendNotification(player, 4, 5000, "Die Türen der Wafenkammer wurden wieder automatisch veriegelt.");
                    Alt.EmitAllClients("Client:DoorManager:ManageDoor", doorColData.hash, new Position(doorColData.posX, doorColData.posY, doorColData.posZ), false);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        #endregion

        #region Jewelery 
        public static Position jeweleryRobPosition = new Position(-621.01f, -228.448f, 38.041f);
        public static bool isJeweleryCurrentlyRobbing = false;

        public static async Task robJewelery(ClassicPlayer player)
        {
            try
            {
                if (player == null || !player.Exists || player.CharacterId <= 0) return;
                if (isJeweleryCurrentlyRobbing) { HUDHandler.SendNotification(player, 3, 2000, "Der Juwelier wird oder wurde bereits ausgeraubt."); return; }
                isJeweleryCurrentlyRobbing = true;
                HUDHandler.SendNotification(player, 1, 600000, "Du raubst den Juwelier aus...");
                player.EmitLocked("Client:Inventory:PlayAnimation", "anim@narcotics@trash", "drop_front", 600000, 1, false);

                foreach (var PDmember in Alt.GetAllPlayers().ToList().Where(x => x != null && x.Exists && x.GetCharacterMetaId() > 0 && ServerFactions.IsCharacterInAnyFaction((int)x.GetCharacterMetaId()) && ServerFactions.GetCharacterFactionId((int)x.GetCharacterMetaId()) == 2))
                    HUDHandler.SendNotification(PDmember, 3, 5000, "Ein Einbruch in den Juwelier wurde gemeldet.");

                await Task.Delay(600000);
                if (player == null || !player.Exists || player.CharacterId <= 0 || !player.Position.IsInRange(jeweleryRobPosition, 5f))
                {
                    isJeweleryCurrentlyRobbing = false;
                    return;
                }
                isJeweleryCurrentlyRobbing = false;
                player.EmitLocked("Client:Inventory:StopAnimation");
                int randomAmount = new Random().Next(40, 100);
                HUDHandler.SendNotification(player, 2, 2000, $"Du konntest {randomAmount} Juwelen erbeuten, verschwinde.");
                CharactersInventory.AddCharacterItem(player.CharacterId, "Juwelen", randomAmount, "inventory");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        #endregion

        #region Moneywash

        public static async Task washmoney(IPlayer player)
        {
            try
            {
                if (player == null || !player.Exists || Characters.GetCharacterIdFromAccId(User.GetPlayerAccountId(player)) <= 0) return;
                int charId = User.GetPlayerOnline(player);
                if (!CharactersInventory.ExistCharacterItem2(charId, "Schwarzgeld")) { HUDHandler.SendNotification(player, 4, 6000, "Du hast kein Schwarzgeld um dieses zu Waschen!"); return; }
                int sam = CharactersInventory.GetCharacterItemAmount2(charId, "Schwarzgeld");
                if (ServerFactions.GetCharacterFactionId(charId) == 1) { HUDHandler.SendNotification(player, 2, 10000, "Korruption ist verboten!"); return; }
                if (ServerFactions.GetCharacterFactionId(charId) == 2) { HUDHandler.SendNotification(player, 2, 10000, "Korruption ist verboten!"); return; }
                if (ServerFactions.GetCharacterFactionId(charId) == 3) { HUDHandler.SendNotification(player, 2, 10000, "Korruption ist verboten!"); return; }
                if (ServerFactions.GetCharacterFactionId(charId) == 4) { HUDHandler.SendNotification(player, 2, 10000, "Korruption ist verboten!"); return; }
                if (ServerFactions.GetCharacterFactionId(charId) == 5) { HUDHandler.SendNotification(player, 2, 10000, "Korruption ist verboten!"); return; }
                if (ServerFactions.GetCharacterFactionId(charId) == 6) { HUDHandler.SendNotification(player, 2, 10000, "Korruption ist verboten!"); return; }
                if (ServerFactions.GetCharacterFactionId(charId) == 7) { HUDHandler.SendNotification(player, 2, 10000, "Korruption ist verboten!"); return; }
                if (ServerFactions.GetCharacterFactionId(charId) == 8) { HUDHandler.SendNotification(player, 2, 10000, "Korruption ist verboten!"); return; }
                HUDHandler.SendNotification(player, 2, 10000, "Das Geldwäschen startet nun... (50s)");

                await Task.Delay(10000); //Zeit bis offen = 50s
                if (!player.Position.IsInRange(Constants.Positions.Schwarzwasch, 5f))
                {
                    HUDHandler.SendNotification(player, 4, 5000, "Du hast dich vom Geldwäschen entfernt - Abbruch");
                    return;
                }
                await Task.Delay(10000); //Zeit bis offen = 50s
                if (!player.Position.IsInRange(Constants.Positions.Schwarzwasch, 5f))
                {
                    HUDHandler.SendNotification(player, 4, 5000, "Du hast dich vom Geldwäschen entfernt - Abbruch");
                    return;
                }
                await Task.Delay(10000); //Zeit bis offen = 50s
                if (!player.Position.IsInRange(Constants.Positions.Schwarzwasch, 5f))
                {
                    HUDHandler.SendNotification(player, 4, 5000, "Du hast dich vom Geldwäschen entfernt - Abbruch");
                    return;
                }
                await Task.Delay(10000); //Zeit bis offen = 50s
                if (!player.Position.IsInRange(Constants.Positions.Schwarzwasch, 5f))
                {
                    HUDHandler.SendNotification(player, 4, 5000, "Du hast dich vom Geldwäschen entfernt - Abbruch");
                    return;
                }
                await Task.Delay(10000); //Zeit bis offen = 50s
                if (!player.Position.IsInRange(Constants.Positions.Schwarzwasch, 5f))
                {
                    HUDHandler.SendNotification(player, 4, 5000, "Du hast dich vom Geldwäschen entfernt - Abbruch");
                    return;
                }


                HUDHandler.SendNotification(player, 2, 50000, $"Die Geldwäsche ist abgeschlossen! ({sam}$)");
                CharactersInventory.AddCharacterItem(charId, "Bargeld", sam, "inventory");
                CharactersInventory.RemoveCharacterItemAmount2(charId, "Schwarzgeld", sam);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        #endregion
    }

    public partial class bankPickUpPosition
    {
        public Position position { get; set; }
        public bool isPickedUp { get; set; } = false;
    }
    
    public partial class LSPDPickUpPosition
    {
        public Position position { get; set; }
        public bool isPickedUp { get; set; } = false;
    }
}
