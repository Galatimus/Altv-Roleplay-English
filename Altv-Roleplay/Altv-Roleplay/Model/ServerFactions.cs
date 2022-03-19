using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using Altv_Roleplay.Handler;
using Altv_Roleplay.models;
using Altv_Roleplay.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altv_Roleplay.Model
{
    class ServerFactions
    {
        public static List<Server_Factions> ServerFactions_ = new List<Server_Factions>();
        public static List<Server_Faction_Ranks> ServerFactionRanks_ = new List<Server_Faction_Ranks>();
        public static List<Server_Faction_Members> ServerFactionMembers_ = new List<Server_Faction_Members>();
        public static List<Server_Faction_Storage_Items> ServerFactionStorageItems_ = new List<Server_Faction_Storage_Items>();
        public static List<Server_Faction_Positions> ServerFactionPositions_ = new List<Server_Faction_Positions>();
        public static List<Server_Faction_Labor_Items> ServerFactionLaborItems_ = new List<Server_Faction_Labor_Items>();
        public static List<ServerFaction_Dispatch> ServerFactionDispatches_ = new List<ServerFaction_Dispatch>();
        public static List<Logs_Faction> LogsFaction_ = new List<Logs_Faction>();
        // 1  = DoJ, 2 = LSPD, 3 = LSFD, 4 = ACLS, 5 = Fahrschule

        #region Labor Functions
        public static bool IsLaborLocked(int factionId)
        {
            Server_Factions x = ServerFactions_.ToList().FirstOrDefault(y => y.id == factionId);
            if (x != null) return x.isLaborLocked;
            return false;
        }

        public static bool ExistFaction(int factionId)
        {
            return ServerFactions_.ToList().Exists(x => x.id == factionId);
        }

        public static string GetLaborItemsFromPlayer(int factionId, int accId)
        {
            return System.Text.Json.JsonSerializer.Serialize(ServerFactionLaborItems_.ToList().Where(x => x.factionId == factionId && x.accountId == accId).Select(x => new
            {
                x.itemName,
                x.itemAmount,
                pic = ServerItems.ReturnItemPicSRC(x.itemName),
            }).ToList());
        }

        public static int GetLaborItemAmount(int factionId, int accountId, string itemName)
        {
            Server_Faction_Labor_Items x = ServerFactionLaborItems_.ToList().FirstOrDefault(y => y.factionId == factionId && y.accountId == accountId && y.itemName == itemName);
            if (x != null) return x.itemAmount;
            return 0;
        }

        public static void AddLaborItem(int factionId, int accountId, string itemName, int itemAmount)
        {
            try
            {
                if (factionId <= 0 || accountId <= 0 || string.IsNullOrWhiteSpace(itemName) || itemAmount <= 0) return;
                Server_Faction_Labor_Items existItem = ServerFactionLaborItems_.FirstOrDefault(x => x.factionId == factionId && x.accountId == accountId && x.itemName == itemName);
                if (existItem != null)
                {
                    existItem.itemAmount += itemAmount;
                    using (var db = new gtaContext())
                    {
                        db.Server_Faction_Labor_Items.Update(existItem);
                        db.SaveChanges();
                    }
                }
                else
                {
                    Server_Faction_Labor_Items newItem = new Server_Faction_Labor_Items
                    {
                        factionId = factionId,
                        accountId = accountId,
                        itemAmount = itemAmount,
                        itemName = itemName
                    };

                    ServerFactionLaborItems_.Add(newItem);
                    using (var db = new gtaContext())
                    {
                        db.Server_Faction_Labor_Items.Add(newItem);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static void RemoveLaborItemAmount(int factionId, int accountId, string itemName, int itemAmount)
        {
            try
            {
                if (factionId <= 0 || accountId <= 0 || string.IsNullOrWhiteSpace(itemName) || itemAmount <= 0) return;
                Server_Faction_Labor_Items item = ServerFactionLaborItems_.FirstOrDefault(x => x.factionId == factionId && x.accountId == accountId && x.itemName == itemName);
                if (item == null) return;
                item.itemAmount -= itemAmount;
                if (item.itemAmount > 0)
                {
                    using (var db = new gtaContext())
                    {
                        db.Server_Faction_Labor_Items.Update(item);
                        db.SaveChanges();
                    }
                }
                else
                {
                    using (var db = new gtaContext())
                    {
                        db.Server_Faction_Labor_Items.Remove(item);
                        db.SaveChanges();
                    }
                    ServerFactionLaborItems_.Remove(item);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static Position GetLaborExitPosition(int factionId)
        {
            Server_Factions x = ServerFactions_.ToList().FirstOrDefault(y => y.id == factionId);
            if (x != null)
            {
                return Utils.Constants.Positions.weedLabor_ExitPosition;
                /*switch (x.type)
                {
                    case 2: return Utils.Constants.Positions.weedLabor_ExitPosition;
                    case 3: return Utils.Constants.Positions.methLabor_ExitPosition;
                }*/
            }
            return new Position(0, 0, 0);
        }

        public static void SetLaborLocked(int factionId, bool state)
        {
            Server_Factions x = ServerFactions_.FirstOrDefault(y => y.id == factionId);
            if (x == null) return;
            x.isLaborLocked = state;
        }
        #endregion

        public static void CreateServerFactionMember(int factionId, int charId, int rank, int dienstnummer)
        {
            try
            {
                if (factionId == 0 || charId == 0) return;
                var factionMemberData = ServerFactionMembers_.FirstOrDefault(x => x.charId == charId);
                if (factionMemberData != null) return;
                var phone = Characters.GetCharacterPhonenumber(charId);
                var charname = Characters.GetCharacterName(charId);
                var RankName = ServerFactions.GetFactionRankName(factionId, rank);
                var factionname = ServerFactions.GetFactionShortName(factionId);
                var factionInviteData = new Server_Faction_Members
                {
                    charId = charId,
                    factionId = factionId,
                    rank = rank,
                    rankname = RankName,
                    serviceNumber = dienstnummer,
                    isDuty = false,
                    lastChange = DateTime.Now,
                    phone = phone,
                    charname = charname,
                    factionname = factionname
                };
                ServerFactionMembers_.Add(factionInviteData);
                using (gtaContext db = new gtaContext())
                {
                    db.Server_Faction_Members.Add(factionInviteData);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static bool existFaction(int factionid)
        {
            try
            {
                if (ServerFactions_.FirstOrDefault(x => x.id == factionid) != null) return true;
                return false;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return false;
        }


        public static void sendMsg(int fId, string msg)
        {
            try
            {
                foreach (var p in Alt.GetAllPlayers().Where(x => x != null && x.Exists && x.GetCharacterMetaId() > 0 && IsCharacterInAnyFaction((int)x.GetCharacterMetaId()) && IsCharacterInFactionDuty((int)x.GetCharacterMetaId()) && GetCharacterFactionId((int)x.GetCharacterMetaId()) == fId).ToList())
                {
                    if (p == null || !p.Exists) continue;
                    HUDHandler.SendNotification(p, 1, 2500, $"{msg}");
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static int GetFactionIdByServiceNumber(int number)
        {
            try
            {
                var faction = ServerFactions_.ToList().FirstOrDefault(x => x.phoneNumber == number);
                if (faction != null) return faction.id;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return 0;
        }

        internal static void UpdateCurrent(int factionId, int v)
        {
            throw new NotImplementedException();
        }

        public static int GetCurrentServicePhoneOwner(int factionId)
        {
            try
            {
                var faction = ServerFactions_.ToList().FirstOrDefault(x => x.id == factionId);
                if (faction != null) return faction.currentPhoneOwnerId;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return 0;
        }

        public static bool IsNumberAFactionNumber(int number)
        {
            try
            {
                var faction = ServerFactions_.ToList().FirstOrDefault(x => x.phoneNumber == number);
                if (faction != null) return true;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return false;
        }

        public static void UpdateCurrentServicePhoneOwner(int factionId, int newid)
        {
            try
            {
                if (factionId <= 0 || newid < 0) return;
                var factiondata = ServerFactions_.FirstOrDefault(x => x.id == factionId);
                if (factiondata == null) return;
                factiondata.currentPhoneOwnerId = newid;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static void AddNewFactionDispatch(int senderId, int factionId, string message, Position pos)
        {
            try
            {
                if (senderId <= 0 || factionId <= 0) return;
                var data = new ServerFaction_Dispatch
                {
                    senderCharId = senderId,
                    factionId = factionId,
                    message = message,
                    Date = DateTime.Now.ToString("d", CultureInfo.CreateSpecificCulture("de-DE")),
                    Destination = pos
                };

                ServerFactionDispatches_.Add(data);
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static bool ExistDispatchBySender(int senderId)
        {
            try
            {
                if (senderId <= 0) return false;
                var dispatch = ServerFactionDispatches_.FirstOrDefault(x => x.senderCharId == senderId);
                if (dispatch != null) return true;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return false;
        }

        public static bool ExistDispatch(int factionId, int senderId)
        {
            try
            {
                if (factionId <= 0 || senderId <= 0) return false;
                var dispatch = ServerFactionDispatches_.FirstOrDefault(x => x.senderCharId == senderId && x.factionId == factionId);
                if (dispatch != null) return true;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return false;
        }

        public static void RemoveDispatch(int factionId, int senderId)
        {
            try
            {
                if (factionId <= 0 || senderId <= 0) return;
                var dispatch = ServerFactionDispatches_.FirstOrDefault(x => x.factionId == factionId && x.senderCharId == senderId);
                if(dispatch != null)
                {
                    ServerFactionDispatches_.Remove(dispatch);
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static void RemoveDispatchWithoutFactionId(int senderId)
        {
            try
            {
                if (senderId <= 0) return;
                var dispatch = ServerFactionDispatches_.FirstOrDefault(x => x.senderCharId == senderId);
                if(dispatch != null)
                {
                    ServerFactionDispatches_.Remove(dispatch);
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static void createFactionDispatch(IPlayer player, int factionId, string msg, string notificationMsg)
        {
            try
            {
                if (player == null || !player.Exists || factionId <= 0 || msg.Length <= 0) return;
                int charId = (int)player.GetCharacterMetaId();
                if (charId <= 0) return;
                if (ExistDispatchBySender(charId)) { RemoveDispatchWithoutFactionId(charId); }
                AddNewFactionDispatch(charId, factionId, msg, player.Position);
                foreach (var p in Alt.GetAllPlayers().Where(x => x != null && x.Exists && x.GetCharacterMetaId() > 0).ToList())
                {
                    if (p == null || !p.Exists) continue;
                    if (!IsCharacterInAnyFaction((int)p.GetCharacterMetaId()) || !IsCharacterInFactionDuty((int)p.GetCharacterMetaId()) || GetCharacterFactionId((int)p.GetCharacterMetaId()) != factionId) continue;
                    HUDHandler.SendNotification(p, 1, 3500, notificationMsg);
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static void RemoveServerFactionMember(int factionId, int charId)
        {
            try
            {
                if (factionId <= 0 || charId <= 0) return;
                var factionMemberData = ServerFactionMembers_.FirstOrDefault(x => x.factionId == factionId && x.charId == charId);
                if (factionMemberData != null)
                {
                    ServerFactionMembers_.Remove(factionMemberData);
                    using (gtaContext db = new gtaContext())
                    {
                        db.Server_Faction_Members.Remove(factionMemberData);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static void AddServerFactionStorageItem(int factionId, int charId, string itemName, int itemAmount)
        {
            if (charId <= 0 || factionId <= 0 || itemName == "" || itemAmount <= 0) return;

            var itemData = new Server_Faction_Storage_Items
            {
                charId = charId,
                factionId = factionId,
                itemName = itemName,
                amount = itemAmount
            };

            try
            {
                var hasItem = ServerFactionStorageItems_.FirstOrDefault(i => i.charId == charId && i.itemName == itemName && i.factionId == factionId);
                if (hasItem != null)
                {
                    //Item existiert, itemAmount erhöhen
                    hasItem.amount += itemAmount;
                    using (gtaContext db = new gtaContext())
                    {
                        var dbitem = db.Server_Faction_Storage_Items.FirstOrDefault(i => i.charId == charId && i.itemName == itemName && i.factionId == factionId);
                        if (dbitem != null)
                        {
                            dbitem.amount = dbitem.amount += itemAmount;
                        }
                        db.SaveChanges();
                    }
                }
                else
                {
                    //Existiert nicht, Item neu adden
                    ServerFactionStorageItems_.Add(itemData);
                    using (gtaContext db = new gtaContext())
                    {
                        db.Server_Faction_Storage_Items.Add(itemData);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static int GetServerFactionStorageItemAmount(int factionId, int charId, string itemName)
        {
            try
            {
                if (factionId <= 0 || charId <= 0 || itemName == "") return 0;
                var item = ServerFactionStorageItems_.FirstOrDefault(x => x.factionId == factionId && x.charId == charId && x.itemName == itemName);
                if (item != null) return item.amount;
            }
            catch(Exception e)
            {
                Alt.Log($"{e}");
            }
            return 0;
        }

        public static void RemoveServerFactionStorageItemAmount(int factionId, int charId, string itemName, int itemAmount)
        {
            try
            {
                if (charId <= 0 || itemName == "" || itemAmount == 0 || factionId <= 0) return;
                var item = ServerFactionStorageItems_.FirstOrDefault(i => i.charId == charId && i.itemName == itemName && i.factionId == factionId);
                if (item != null)
                {
                    using (gtaContext db = new gtaContext())
                    {
                        int prevAmount = item.amount;
                        item.amount -= itemAmount;
                        if (item.amount > 0)
                        {
                            db.Server_Faction_Storage_Items.Update(item);
                            db.SaveChanges();
                        }
                        else
                            RemoveServerFactionStorageItem(factionId, charId, itemName);
                    }
                }
            }
            catch (Exception _) { Alt.Log($"{_}"); }
        }

        public static void RemoveServerFactionStorageItem(int factionId, int charId, string itemName)
        {
            try
            {
                var item = ServerFactionStorageItems_.FirstOrDefault(i => i.charId == charId && i.itemName == itemName && i.factionId == factionId);
                if (item != null)
                {
                    ServerFactionStorageItems_.Remove(item);
                    using (gtaContext db = new gtaContext())
                    {
                        db.Server_Faction_Storage_Items.Remove(item);
                        db.SaveChanges();
                    }
                }
            }
            catch(Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static string GetFactionFullName(int factionId)
        {
            if (factionId <= 0) return "Zivilist";
            string fullName = "Zivilist";
            var factionData = ServerFactions_.FirstOrDefault(x => x.id == factionId);
            if(factionData != null)
            {
                fullName = factionData.factionName;
            }
            return fullName;
        }

        public static string GetFactionShortName(int factionid)
        {
            string shortCut = "Zivilist";
            if (factionid == 0) return shortCut;
            var factionData = ServerFactions_.FirstOrDefault(x => x.id == factionid);
            if(factionData != null)
            {
                shortCut = factionData.factionShort;
            }
            return shortCut;
        }

        public static string GetFactionRankName(int factionId, int rankId)
        {
            if (factionId == 0 || rankId == 0) return "";
            var factionRankData = ServerFactionRanks_.FirstOrDefault(x => x.factionId == factionId && x.rankId == rankId);
            if(factionRankData != null)
            {
                return factionRankData.name;
            }
            return "";
        }

        public static int GetFactionRankPaycheck(int factionId, int rankId)
        {
            if (factionId == 0 || rankId == 0) return 0;
            var factionRankData = ServerFactionRanks_.FirstOrDefault(x => x.factionId == factionId && x.rankId == rankId);
            if(factionRankData != null)
            {
                return factionRankData.paycheck;
            }
            return 0;
        }

        public static void SetFactionRankPaycheck(int factionId, int rankId, int money)
        {
            try
            {
                if (factionId <= 0 || rankId <= 0 || money <= 0) return;
                var factionRankData = ServerFactionRanks_.FirstOrDefault(x => x.factionId == factionId && x.rankId == rankId);
                if (factionRankData != null)
                {
                    factionRankData.paycheck = money;
                    using (gtaContext db = new gtaContext())
                    {
                        db.Server_Faction_Ranks.Update(factionRankData);
                        db.SaveChanges();
                    }
                }
            }
            catch(Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static bool IsCharacterInAnyFaction(int charId)
        {
            try
            {
                if (charId == 0) return false;
                var factionMemberData = ServerFactionMembers_.FirstOrDefault(x => x.charId == charId);
                if(factionMemberData != null)
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return false;
        }

        public static int GetCharacterFactionId(int charId)
        {
            try
            {
                if (charId == 0) return 0;
                var factionMemberData = ServerFactionMembers_.FirstOrDefault(x => x.charId == charId);
                if(factionMemberData != null)
                {
                    return factionMemberData.factionId;
                }
            }
            catch(Exception e)
            {
                Alt.Log($"{e}");
            }
            return 0;
        }

        public static int GetCharacterFactionRank(int charId)
        {
            try
            {
                if (charId == 0) return 0;
                var factionMemberData = ServerFactionMembers_.FirstOrDefault(x => x.charId == charId);
                if(factionMemberData != null)
                {
                    return factionMemberData.rank;
                }
            }
            catch(Exception e)
            {
                Alt.Log($"{e}");
            }
            return 0;
        }

        public static void SetCharacterFactionRank(int charId, int newRank)
        {
            try
            {
                if (charId <= 0 || newRank <= 0) return;
                var factionMemberData = ServerFactionMembers_.FirstOrDefault(x => x.charId == charId);
                if(factionMemberData != null)
                {
                    factionMemberData.rank = newRank;
                    using (gtaContext db = new gtaContext())
                    {
                        db.Server_Faction_Members.Update(factionMemberData);
                        db.SaveChanges();
                    }
                }
            }
            catch(Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static int GetCharacterFactionServiceNumber(int charId) //Dienstnummer
        {
            try
            {
                if (charId == 0) return 0;
                var factionMemberData = ServerFactionMembers_.FirstOrDefault(x => x.charId == charId);
                if(factionMemberData != null)
                {
                    return factionMemberData.serviceNumber;
                }
            }
            catch(Exception e)
            {
                Alt.Log($"{e}");
            }
            return 0;
        }

        public static DateTime GetCharacterFactionLastChange(int charId)
        {
            DateTime dt = new DateTime(0, 0, 0, 0, 0, 0);
            try
            {
                if (charId == 0) return dt;
                var factionMemberData = ServerFactionMembers_.FirstOrDefault(x => x.charId == charId);
                if(factionMemberData != null)
                {
                    dt = factionMemberData.lastChange;
                }
            }
            catch(Exception e)
            {
                Alt.Log($"{e}");
            }
            return dt;
        }

        public static bool ExistFactionServiceNumber(int factionId, int serviceNumber) //Existiert Dienstnummer bereits?
        {
            try
            {
                if (factionId == 0 || serviceNumber == 0) return false;
                var factionMemberData = ServerFactionMembers_.FirstOrDefault(x => x.factionId == factionId && x.serviceNumber == serviceNumber);
                if (factionMemberData != null) return true;
            }
            catch(Exception e)
            {
                Alt.Log($"{e}");
            }
            return false;
        }

        public static int GetFactionMaxRankCount(int factionId)
        {
            int rankCount = 0;
            try
            {
                if (factionId == 0) return rankCount;
                rankCount = ServerFactionRanks_.Where(x => x.factionId == factionId).Count();
            }
            catch(Exception e)
            {
                Alt.Log($"{e}");
            }
            return rankCount;
        }

        public static bool IsCharacterInFactionDuty(int charId)
        {
            try
            {
                if (charId == 0) return false;
                var factionMemberData = ServerFactionMembers_.FirstOrDefault(x => x.charId == charId);
                if(factionMemberData != null)
                {
                    return factionMemberData.isDuty;
                }
            }
            catch(Exception e)
            {
                Alt.Log($"{e}");
            }
            return false;
        }

        public static int GetFactionDutyMemberCount(int factionId)
        {
            int memberCount = 0;
            try
            {
                if (factionId <= 0) return memberCount;
                memberCount = ServerFactionMembers_.Where(x => x.factionId == factionId && x.isDuty == true && Alt.GetAllPlayers().ToList().FirstOrDefault(p => p.GetCharacterMetaId() == (ulong)x.charId) != null).Count();
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return memberCount;
        }

        public static void SetCharacterInFactionDuty(int charId, bool state)
        {
            try
            {
                if (charId <= 0) return;
                var factionMemberData = ServerFactionMembers_.FirstOrDefault(x => x.charId == charId);
                if(factionMemberData != null)
                {
                    factionMemberData.isDuty = state;
                    using (gtaContext db = new gtaContext())
                    {
                        db.Server_Faction_Members.Update(factionMemberData);
                        db.SaveChanges();
                    }
                }
            }
            catch(Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static int GetFactionBankMoney(int factionId)
        {
            try
            {
                if (factionId == 0) return 0;
                var factionData = ServerFactions_.FirstOrDefault(x => x.id == factionId);
                if(factionData != null)
                {
                    return factionData.factionMoney;
                }
            }
            catch(Exception e)
            {
                Alt.Log($"{e}");
            }
            return 0;
        }

        public static void SetFactionBankMoney(int factionId, int money)
        {
            try
            {
                if (factionId <= 0 || money < 1) return;
                var factionData = ServerFactions_.FirstOrDefault(x => x.id == factionId);
                if(factionData != null)
                {
                    factionData.factionMoney = money;
                    using (gtaContext db = new gtaContext())
                    {
                        db.Server_Factions.Update(factionData);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static int GetServerFactionMemberCount(int factionId)
        {
            int Count = 0;
            if (factionId == 0) return Count;
            Count = ServerFactionMembers_.Where(x => x.factionId == factionId).Count();
            return Count;
        }

        public static int GetServerFactionLeader(int factionId)
        {
            if (factionId == 0) return 0;
            var factionMemberData = ServerFactionMembers_.FirstOrDefault(x => x.factionId == factionId && x.rank == GetFactionMaxRankCount(factionId));
            if(factionMemberData != null)
            {
                return factionMemberData.charId;
            }
            return 0;
        }

        public static string GetServerFactionManagerInfos(int factionId)
        {
            if (factionId == 0) return "";

            var items = ServerFactions_.Where(x => x.id == factionId).Select(x => new
            {
                factionId,
                x.factionName,
                factionOwner = Characters.GetCharacterName(GetServerFactionLeader(factionId)),
                factionBalance = x.factionMoney,
                factionMemberCount = GetServerFactionMemberCount(factionId),
            }).ToList();

            return JsonConvert.SerializeObject(items);
        }

        public static string GetFactionDispatches(int factionId)
        {
            if (factionId <= 0) return "[]";

            var items = ServerFactionDispatches_.Where(x => x.factionId == factionId).Select(x => new
            {
                x.factionId,
                x.senderCharId,
                senderName = Characters.GetCharacterName(x.senderCharId),
                x.message,
                x.Date,
                posX = x.Destination.X,
                posY = x.Destination.Y,
                posZ = x.Destination.Z,
            }).ToList();

            return JsonConvert.SerializeObject(items);
        }

        public static string GetServerFactionMembers(int factionId)
        {
            if (factionId == 0) return "";

            var items = ServerFactionMembers_.Where(x => x.factionId == factionId).Select(x => new
            {
                x.factionId,
                x.charId,
                charName = Characters.GetCharacterName(x.charId),
                x.rank,
                x.serviceNumber,
                date = x.lastChange.ToString("dd.MM.yyy"),
            }).OrderByDescending(x => x.rank).ToList();

            return JsonConvert.SerializeObject(items);
        }

        public static string GetServerFactionRanks(int factionId)
        {
            if (factionId == 0) return "";

            var items = ServerFactionRanks_.Where(x => x.factionId == factionId).Select(x => new
            {
                x.factionId,
                x.rankId,
                rankName = x.name,
                rankCurPaycheck = x.paycheck,
            }).OrderBy(x => x.rankId).ToList();

            return JsonConvert.SerializeObject(items);
        }

        public static string GetServerFactionStorageItems(int factionId, int charId)
        {
            if (factionId <= 0 || charId <= 0) return "[]";
            var items = ServerFactionStorageItems_.Where(x => x.factionId == factionId && x.charId == charId).Select(x => new
            {
                x.id,
                x.charId,
                x.factionId,
                x.itemName,
                x.amount,
                itemPicName = ServerItems.ReturnItemPicSRC(x.itemName),
            }).ToList();
            return JsonConvert.SerializeObject(items);
        }

        public static bool ExistServerFactionStorageItem(int factionId, int charId, string itemName)
        {
            try
            {
                if (factionId <= 0 || charId <= 0 || itemName == "") return false;
                var storageData = ServerFactionStorageItems_.FirstOrDefault(x => x.factionId == factionId && x.charId == charId && x.itemName == itemName);
                if (storageData != null) return true;
            }
            catch(Exception e)
            {
                Alt.Log($"{e}");
            }
            return false;
        }

        public static bool ExistServerFactionRankOnId(int factionid, int rankId)
        {
            try
            {
                if (factionid <= 0 || rankId <= 0) return false;
                var factionRankData = ServerFactionRanks_.FirstOrDefault(x => x.factionId == factionid && x.rankId == rankId);
                if(factionRankData != null)
                {
                    return true;
                }
            }
            catch(Exception e)
            {
                Alt.Log($"{e}");
            }
            return false;
        }
    }
}
