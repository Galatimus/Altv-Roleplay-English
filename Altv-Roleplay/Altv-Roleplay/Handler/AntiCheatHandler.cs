using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using Altv_Roleplay.Factories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using AltV.Net.Enums;
using Altv_Roleplay.Model;
using Altv_Roleplay.Utils;
using AltV.Net.Elements.Entities;
using AltV.Net.Resources.Chat.Api;

namespace Altv_Roleplay.Handler
{
    public class AntiCheatHandler : IScript
    {     
        [AsyncScriptEvent(ScriptEventType.WeaponDamage)]
        public async Task WeaponDamageEvent(ClassicPlayer player, ClassicPlayer target, uint weapon, ushort dmg, Position offset, BodyPart bodypart)
        {
            try
            {
                if (player == null || !player.Exists || target == null || !target.Exists) return;
                WeaponModel weaponModel = (WeaponModel)weapon;
                if (weaponModel == WeaponModel.Fist) return;
                if(Enum.IsDefined(typeof(AntiCheat.forbiddenWeapons), (Utils.AntiCheat.forbiddenWeapons)weaponModel))
                {
                    User.SetPlayerBanned(player, true, $"Blacklisted Weaponkill: {weaponModel}");
                    player.Kick("");
                    foreach(IPlayer p in Alt.GetAllPlayers().ToList().Where(x => x != null && x.Exists && ((ClassicPlayer)x).CharacterId > 0 && x.AdminLevel() > 0))
                    {
                        HUDHandler.SendNotification(player, 4, 2500, $"{Characters.GetCharacterName(player.CharacterId)} wurde gebannt: Waffenhack[2] - {weaponModel}");
                    }
                    return;
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }
    }
}
