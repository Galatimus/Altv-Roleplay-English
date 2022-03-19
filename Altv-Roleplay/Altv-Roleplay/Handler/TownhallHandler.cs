using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using Altv_Roleplay.Model;
using Altv_Roleplay.Utils;

namespace Altv_Roleplay.Handler
{
    class TownhallHandler : IScript
    {
        [AsyncClientEvent("Server:HUD:sendIdentityCardApplyForm")]
        public async Task sendIdentityCardApplyForm(IPlayer player, string birthplace)
        {
            if (player == null || !player.Exists) return;
            int charId = User.GetPlayerOnline(player);
            if (charId == 0 || birthplace == "") return;
            Characters.SetCharacterBirthplace(charId, birthplace);
            Characters.setCharacterAccState(charId, 1);
            CharactersInventory.AddCharacterItem(charId, $"Ausweis {Characters.GetCharacterName(charId)}", 1, "inventory");
            HUDHandler.SendNotification(player, 2, 5000, "Du hast dir erfolgreich deinen Personalausweis beantragt.");
            HUDHandler.SendNotification(player, 1, 5000, "Erfolg freigeschaltet: Identifizierung");
        }

        internal static void tryCreateIdentityCardApplyForm(IPlayer player)
        {
            if (player == null || !player.Exists) return;
            int charId = User.GetPlayerOnline(player);
            if (charId == 0) return;
            var charname = Characters.GetCharacterName(charId);
            var birthdate = Characters.GetCharacterBirthdate(charId);
            var adress = $"{Characters.GetCharacterStreet(charId)}";
            var curBirthpl = Characters.GetCharacterBirthplace(charId);
            bool gender = Characters.GetCharacterGender(charId);
            player.EmitLocked("Client:HUD:createIdentityCardApplyForm", charname, gender, adress, birthdate, curBirthpl);
        }

        internal static void createJobcenterBrowser(IPlayer player)
        {
            if (player == null || !player.Exists) return;
            int charId = User.GetPlayerOnline(player);
            if (charId == 0) return;
            var jobs = ServerJobs.GetAllServerJobs(); 
            player.EmitLocked("Client:Jobcenter:OpenCEF", jobs);
        }

        [AsyncClientEvent("Server:Jobcenter:SelectJob")]
        public async Task SelectJobcenterJob(IPlayer player, string jobName)
        {
            try
            {
                if (player == null || !player.Exists || jobName == "" || jobName == "undefined") return;
                int charId = User.GetPlayerOnline(player);
                if (charId == 0) return;
                if (jobName == "None") { HUDHandler.SendNotification(player, 2, 5000, $"Du hast deinen Job als {Characters.GetCharacterJob(charId)} gekündigt."); Characters.SetCharacterLastJobPaycheck(charId, DateTime.Now); Characters.SetCharacterJob(charId, "None"); return; }
                Characters.SetCharacterJob(charId, jobName);
                Characters.SetCharacterLastJobPaycheck(charId, DateTime.Now);
                Characters.ResetCharacterJobHourCounter(charId);
                HUDHandler.SendNotification(player, 2, 5000, $"Du hast den Vertrag für den Beruf '{jobName}' unterschrieben. Dein Gehalt liegt bei {ServerJobs.GetJobPaycheck(jobName)}$. Du musst täglich {ServerJobs.GetJobNeededHours(jobName)} Stunden anwesend sein.");
                if (!CharactersTablet.HasCharacterTutorialEntryFinished(charId, "acceptJob"))
                {
                    CharactersTablet.SetCharacterTutorialEntryState(charId, "acceptJob", true);
                    HUDHandler.SendNotification(player, 1, 2500, "Erfolg freigeschaltet: Die Tür vom Arbeitsamt");
                }
            }
            catch(Exception e) { Alt.Log($"{e}"); }
        }

        internal static void openHouseSelector(IPlayer player)
        {
            try
            {
                if (player == null || !player.Exists) return;
                int charId = (int)player.GetCharacterMetaId();
                if (charId <= 0) return;
                string info = ServerHouses.GetAllCharacterHouses(charId);
                player.EmitLocked("Client:Townhall:openHouseSelector", info);
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }
    }
}
