using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using Altv_Roleplay.Handler;
using Altv_Roleplay.Model;
using Altv_Roleplay.Utils;

namespace Altv_Roleplay.Factions.Justice
{
    class Functions : IScript
    {
        [AsyncClientEvent("Server:Tablet:JusticeAppGiveWeaponLicense")]
        public async Task GiveWeaponLicense(IPlayer player, string targetCharName)
        {
            try
            {
                if (player == null || !player.Exists || targetCharName == "") return;
                int charId = User.GetPlayerOnline(player);
                if (charId <= 0) return;
                if (player.HasPlayerHandcuffs() || player.HasPlayerRopeCuffs()) { HUDHandler.SendNotification(player, 3, 5000, "Wie willst du das gefesselt machen?"); return; }
                if (!ServerFactions.IsCharacterInAnyFaction(charId)) { HUDHandler.SendNotification(player, 4, 5000, "Du bist in keiner Fraktion."); return; }
                if (!ServerFactions.IsCharacterInFactionDuty(charId)) { HUDHandler.SendNotification(player, 4, 5000, "Du bist nicht im Dienst."); return; }
                if (ServerFactions.GetCharacterFactionId(charId) != 1) { HUDHandler.SendNotification(player, 4, 5000, "Du bist kein Angehöriger der Justiz."); return; }
                if(!Characters.ExistCharacterName(targetCharName)) { HUDHandler.SendNotification(player, 3, 5000, $"Der angegebene Name wurde nicht gefunden ({targetCharName})."); return; }
                int targetCharId = Characters.GetCharacterIdFromCharName(targetCharName);
                if (targetCharId <= 0) return;
                var targetPlayer = Alt.GetAllPlayers().ToList().FirstOrDefault(x => x.GetCharacterMetaId() == (ulong)targetCharId);
                if (targetPlayer == null || !targetPlayer.Exists) return;
                if(!player.Position.IsInRange(targetPlayer.Position, 5f)) { HUDHandler.SendNotification(player, 3, 5000, "Der Spieler ist nicht in Ihrer Nähe."); return; }
                if(CharactersLicenses.HasCharacterLicense(targetCharId, "weaponlicense")) { HUDHandler.SendNotification(player, 3, 5000, "Der Spieler hat bereits einen Waffenschein."); return; }
                CharactersLicenses.SetCharacterLicense(targetCharId, "weaponlicense", true);
                Characters.AddCharacterPermission(targetCharId, "weaponlicense");
                HUDHandler.SendNotification(player, 2, 3500, $"Sie haben dem Spieler {targetCharName} den Waffenschein erfolgreich ausgestellt.");
                HUDHandler.SendNotification(targetPlayer, 2, 3500, $"Ihnen wurde der Waffenschein erfolgreich ausgestellt.");
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [AsyncClientEvent("Server:Tablet:JusticeAppSearchBankAccounts")]
        public async Task SearchBankAccounts(IPlayer player, string targetCharName)
        {
            try
            {
                if (player == null || !player.Exists || targetCharName == "") return;
                int charId = User.GetPlayerOnline(player);
                if (charId <= 0) return;
                if (player.HasPlayerHandcuffs() || player.HasPlayerRopeCuffs()) { HUDHandler.SendNotification(player, 3, 5000, "Wie willst du das gefesselt machen?"); return; }
                if (!ServerFactions.IsCharacterInAnyFaction(charId)) { HUDHandler.SendNotification(player, 3, 5000, "Du bist in keiner Fraktion."); return; }
                if (!ServerFactions.IsCharacterInFactionDuty(charId)) { HUDHandler.SendNotification(player, 3, 5000, "Du bist nicht im Dienst."); return; }
                if (ServerFactions.GetCharacterFactionId(charId) != 1) { HUDHandler.SendNotification(player, 3, 5000, "Du bist kein Angehöriger der Justiz."); return; }
                if (!Characters.ExistCharacterName(targetCharName)) { HUDHandler.SendNotification(player, 3, 5000, $"Der angegebene Name wurde nicht gefunden ({targetCharName})."); return; }
                int targetCharId = Characters.GetCharacterIdFromCharName(targetCharName);
                if (targetCharId <= 0) return;
                var targetBankAccounts = CharactersBank.GetCharacterBankAccounts(targetCharId);
                if(targetBankAccounts == "[]" || targetBankAccounts == "") { HUDHandler.SendNotification(player, 3, 5000, "Der Spieler hat keine Bankkonten."); return; }
                player.EmitLocked("Client:Tablet:SetJusticeAppSearchedBankAccounts", targetBankAccounts);
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [AsyncClientEvent("Server:Tablet:JusticeAppViewBankTransactions")]
        public async Task ViewBankTransactions(IPlayer player, int accNumber)
        {
            try
            {
                if (player == null || !player.Exists || accNumber <= 0) return;
                int charId = User.GetPlayerOnline(player);
                if (charId <= 0) return;
                if (player.HasPlayerHandcuffs() || player.HasPlayerRopeCuffs()) { HUDHandler.SendNotification(player, 3, 5000, "Wie willst du das gefesselt machen?"); return; }
                if (!ServerFactions.IsCharacterInAnyFaction(charId)) { HUDHandler.SendNotification(player, 3, 5000, "Du bist in keiner Fraktion."); return; }
                if (!ServerFactions.IsCharacterInFactionDuty(charId)) { HUDHandler.SendNotification(player, 3, 5000, "Du bist nicht im Dienst."); return; }
                if (ServerFactions.GetCharacterFactionId(charId) != 1) { HUDHandler.SendNotification(player, 3, 5000, "Du bist kein Angehöriger der Justiz."); return; }
                if (!CharactersBank.ExistBankAccountNumber(accNumber)) { HUDHandler.SendNotification(player, 3, 5000, $"Die ausgewählte Kontonummer existiert nicht ({accNumber})."); return; }
                var bankPapers = ServerBankPapers.GetTabletBankAccountBankPaper(accNumber);
                if(bankPapers == "[]" || bankPapers == "") { HUDHandler.SendNotification(player, 3, 5000, "Dieses Konto besitzt keine Transaktionen."); return; }
                player.EmitLocked("Client:Tablet:SetJusticeAppBankTransactions", bankPapers);
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }
    }
}
