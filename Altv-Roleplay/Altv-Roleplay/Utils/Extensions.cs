
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using Altv_Roleplay.Factories;
using Altv_Roleplay.Handler;
using Altv_Roleplay.Model;
using Altv_Roleplay.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altv_Roleplay.Utils
{
    public static partial class Extensions
    {
        public static bool IsInRange(this Position currentPosition, Position otherPosition, float distance)
            => currentPosition.Distance(otherPosition) <= distance;

        public static void kickWithMessage(this IPlayer player, string reason) {
            HUDHandler.SendNotification(player, 3, 250000, $"Du wurdest vom Server gekickt. Grund: {reason}");
            player.Kick(reason);
        }

        public static bool IsCefOpen(this ClassicPlayer player)
        {
            if (player == null || !player.Exists) return false;
            player.GetSyncedMetaData("IsCefOpen", out bool isCefOpened);
            return isCefOpened;
        }

        public static void updateTattoos(this ClassicPlayer player)
        {
            if (player == null || !player.Exists || player.CharacterId <= 0) return;
            player.EmitAsync("Client:Utilities:setTattoos", Model.CharactersTattoos.GetAccountTattoos(player.CharacterId));
        }

        public static bool HasVehicleId(this IVehicle vehicle)
        {
            var myVehicle = (ClassicVehicle)vehicle;
            if (myVehicle == null || !myVehicle.Exists) return false;
            return myVehicle.VehicleId != 0;
        }

        public static void SetVehicleId(this IVehicle vehicle, ulong vehicleId)
        {
            var myVehicle = (ClassicVehicle)vehicle;
            if (myVehicle == null || !myVehicle.Exists) return;
            myVehicle.VehicleId = (int)vehicleId;
        }

        public static void SetVehicleTrunkState(this IVehicle vehicle, bool isOpen) //True = offen, false = Zu
        {
            var myVehicle = (ClassicVehicle)vehicle;
            if (myVehicle == null || !myVehicle.Exists) return;
            myVehicle.Trunkstate = isOpen;
        }

        public static bool GetVehicleTrunkState(this IVehicle vehicle)
        {
            var myVehicle = (ClassicVehicle)vehicle;
            if (myVehicle == null || !myVehicle.Exists) return false;
            return myVehicle.Trunkstate;
        }

        public static ulong GetVehicleId(this IVehicle vehicle)
        {
            var myVehicle = (ClassicVehicle)vehicle;
            if (myVehicle == null || !myVehicle.Exists) return 0;
            return (ulong)myVehicle.VehicleId;
        }

        public static ulong GetCharacterMetaId(this IPlayer player)
        {
            var myPlayer = (ClassicPlayer)player;
            if (myPlayer == null || !myPlayer.Exists) return 0;
            return (ulong)myPlayer.CharacterId;
        }

        public static void SetCharacterMetaId(this IPlayer player, ulong id)
        {
            var myPlayer = (ClassicPlayer)player;
            if (myPlayer == null || !myPlayer.Exists) return;
            myPlayer.CharacterId = (int)id;
        }

        public static void SetColShapeName(this IColShape cols, string name)
        {
            var myColshape = (ClassicColshape)cols;
            if (myColshape == null || !myColshape.Exists) return;
            myColshape.ColshapeName = name;
        }

        public static ulong GetColshapeCarDealerVehPrice(this IColShape cols)
        {
            var myColshape = (ClassicColshape)cols;
            if (myColshape == null || !myColshape.Exists) return 0;
            return myColshape.CarDealerVehPrice;
        }

        public static string GetColshapeCarDealerVehName(this IColShape cols)
        {
            var myColshape = (ClassicColshape)cols;
            if (myColshape == null || !myColshape.Exists) return "";
            return myColshape.CarDealerVehName;
        }

        public static string GetColShapeName(this IColShape cols)
        {
            var myColshape = (ClassicColshape)cols;
            if (myColshape == null || !myColshape.Exists) return "None";
            return myColshape.ColshapeName;
        }

        public static void SetColShapeId(this IColShape cols, ulong id)
        {
            var myColshape = (ClassicColshape)cols;
            if (myColshape == null || !myColshape.Exists) return;
            myColshape.ColshapeId = (int)id;
        }

        public static ulong GetColShapeId(this IColShape cols)
        {
            var myColshape = (ClassicColshape)cols;
            if (myColshape == null || !myColshape.Exists) return 0;
            return (ulong)myColshape.ColshapeId;
        }

        public static string GetPlayerFarmingActionMeta(this IPlayer player)
        {
            var myPlayer = (ClassicPlayer)player;
            if (myPlayer == null || !myPlayer.Exists) return "None";
            return myPlayer.FarmingAction;
        }

        public static void SetPlayerFarmingActionMeta(this IPlayer player, string meta)
        {
            var myPlayer = (ClassicPlayer)player;
            if (myPlayer == null || !myPlayer.Exists) return;
            myPlayer.FarmingAction = meta;
        }

        public static void SetPlayerIsCuffed(this IPlayer player, string cuffType, bool isCuffed)
        {
            //cuffType = handcuffs | ropecuffs
            if (player == null || !player.Exists) return;
            if (cuffType == "handcuffs")
            {
                AltAsync.Do(() => player.SetSyncedMetaData("HasHandcuffs", isCuffed));
            } else if(cuffType == "ropecuffs")
            {
                AltAsync.Do(() => player.SetSyncedMetaData("HasRopeCuffs", isCuffed));
            }
        }

        public static void SetPlayerUsingCrowbar(this IPlayer player, bool isUsing)
        {
            var myPlayer = (ClassicPlayer)player;
            if (myPlayer == null || !myPlayer.Exists) return;
            myPlayer.IsUsingCrowbar = isUsing;
        }

        public static void SetPlayerIsUnconscious(this IPlayer player, bool isUnconscious)
        {
            var myPlayer = (ClassicPlayer)player;
            if (myPlayer == null || !myPlayer.Exists) return;
            myPlayer.IsUnconscious = isUnconscious;
        }

        public static bool IsPlayerUnconscious(this IPlayer player)
        {
            var myPlayer = (ClassicPlayer)player;
            if (myPlayer == null || !myPlayer.Exists) return false;
            return myPlayer.IsUnconscious;
        }

        public static void SetPlayerIsFastFarm(this IPlayer player, bool isFastFarm)
        {
            try
            {
                if (player == null || !player.Exists) return;
                var playerDb = Characters.PlayerCharacters.ToList().FirstOrDefault(x => x.charId == (int)player.GetCharacterMetaId());
                if (playerDb == null) return;
                playerDb.isFastFarm = isFastFarm;
                using (var db = new gtaContext())
                {
                    db.AccountsCharacters.Update(playerDb);
                    db.SaveChanges();
                }
            }
            catch (Exception e) { Alt.Log($"{e}"); }
        }

        public static bool IsPlayerFastFarm(this IPlayer player)
        {
            if (player == null || !player.Exists) return false;
            var playerDb = Characters.PlayerCharacters.ToList().FirstOrDefault(x => x.charId == (int)player.GetCharacterMetaId());
            return playerDb == null ? false : Convert.ToBoolean(playerDb.isFastFarm);
        }

        public static bool HasPlayerHandcuffs(this IPlayer player)
        {
            if (player == null || !player.Exists) return false;
            return player.GetSyncedMetaData("HasHandcuffs", out bool handCuffs) ? handCuffs : false;
        }

        public static bool IsPlayerUsingCrowbar(this IPlayer player)
        {
            var myPlayer = (ClassicPlayer)player;
            if (myPlayer == null || !myPlayer.Exists) return false;
            return myPlayer.IsUsingCrowbar;
        }

        public static bool HasPlayerRopeCuffs(this IPlayer player)
        {
            if (player == null || !player.Exists) return false;
            return player.GetSyncedMetaData("HasRopeCuffs", out bool RopeCuffs) ? RopeCuffs : false;
        }

        public static int AdminLevel(this IPlayer player)
        {
            if (player == null || !player.Exists) return 0;
            var playerDb = User.Player.FirstOrDefault(p => p.socialClub == player.SocialClubId && player.GetCharacterMetaId() == (ulong)p.Online);
            return playerDb == null ? 0 : Convert.ToInt32(playerDb.adminLevel);
        }

        //Minijob Zeug
        public static void SetPlayerCurrentMinijob(this IPlayer player, string minijobName) //Um den Minijob zu speichern (bspw. Holzfäller, Geldtransporter)
        {
            var myPlayer = (ClassicPlayer)player;
            if (myPlayer == null || !myPlayer.Exists) return;
            myPlayer.CurrentMinijob = minijobName;
        }

        public static string GetPlayerCurrentMinijob(this IPlayer player)
        {
            var myPlayer = (ClassicPlayer)player;
            if (myPlayer == null || !myPlayer.Exists) return "None";
            return myPlayer.CurrentMinijob;
        }

        public static void SetPlayerCurrentMinijobStep(this IPlayer player, string minijobStep) //Verwendung: um bspw. Nebenauftrag abzuspeichern
        {
            var myPlayer = (ClassicPlayer)player;
            if (myPlayer == null || !myPlayer.Exists) return;
            myPlayer.CurrentMinijobStep = minijobStep;
        }

        public static string GetPlayerCurrentMinijobStep(this IPlayer player)
        {
            var myPlayer = (ClassicPlayer)player;
            if (myPlayer == null || !myPlayer.Exists) return "None";
            return myPlayer.CurrentMinijobStep;
        }

        public static void SetPlayerCurrentMinijobActionCount(this IPlayer player, ulong count) //Verwendung: ATM aufgefüllt => Count 1 höher => next ATM => Count höhr => Count == max == fertig.
        {
            var myPlayer = (ClassicPlayer)player;
            if (myPlayer == null || !myPlayer.Exists) return;
            myPlayer.CurrentMinijobActionCount = (int)count;
        }

        public static ulong GetPlayerCurrentMinijobActionCount(this IPlayer player)
        {
            var myPlayer = (ClassicPlayer)player;
            if (myPlayer == null || !myPlayer.Exists) return 0;
            return (ulong)myPlayer.CurrentMinijobActionCount;
        }

        //Miniob: Müllmann
        public static void SetPlayerCurrentMinijobRouteId(this IPlayer player, ulong routeId)
        {
            var myPlayer = (ClassicPlayer)player;
            if (myPlayer == null || !myPlayer.Exists) return;
            myPlayer.CurrentMinijobRouteId = (int)routeId;
        }

        public static ulong GetPlayerCurrentMinijobRouteId(this IPlayer player)
        {
            var myPlayer = (ClassicPlayer)player;
            if (myPlayer == null || !myPlayer.Exists) return 0;
            return (ulong)myPlayer.CurrentMinijobRouteId;
        }

        public static Position getPositionInBackOfPosition(this Position pos, float rotation, float distance)
        {
            Position position = pos;
            float rot = rotation;
            var radius = rot * Math.PI / 180;
            Position newPos = new Position(position.X + (float)(distance * Math.Sin(-radius)), position.Y + (float)(distance * Math.Cos(-radius)), position.Z);
            return newPos;
        }
    }
}