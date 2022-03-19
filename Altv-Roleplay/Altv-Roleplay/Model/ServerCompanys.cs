using AltV.Net;
using AltV.Net.Data;
using Altv_Roleplay.models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altv_Roleplay.Model
{
    class ServerCompanys
    {
        public static List<Server_Companys> ServerCompanysData_ = new List<Server_Companys>();
        public static List<Server_Company_Members> ServerCompanysMember_ = new List<Server_Company_Members>();
        public static List<Logs_Company> LogsCompany_ = new List<Logs_Company>();

        public static void CreateServerCompany(string companyName, int companyOwnerId, Position pos, string givenLicense)
        {
            if (companyName == "" || companyOwnerId == 0) return;
            var companyData = new Server_Companys
            {
                companyName = companyName,
                companyOwnerId = companyOwnerId,
                posX = pos.X,
                posY = pos.Y,
                posZ = pos.Z,
                givenLicense = givenLicense,
                createdTimestamp = DateTime.Now,
                companyMoney = 0
            };

            try
            {
                ServerCompanysData_.Add(companyData);
                using (gtaContext db = new gtaContext())
                {
                    db.Server_Companys.Add(companyData);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static void CreateServerCompanyMember(int companyId, int charId, int rank)
        {
            try
            {
                if (companyId == 0 || charId == 0) return;
                var companyData = ServerCompanysData_.FirstOrDefault(x => x.id == companyId);
                if(companyData != null)
                {
                    var companyMemberList = ServerCompanysMember_.FirstOrDefault(x => x.charId == charId);
                    if (companyMemberList != null) return;
                    var companyMemberData = new Server_Company_Members
                    {
                        companyId = companyId,
                        charId = charId,
                        rank = rank,
                        invitedTimestamp = DateTime.Now
                    };

                    ServerCompanysMember_.Add(companyMemberData);
                    using (gtaContext db = new gtaContext())
                    {
                        db.Server_Company_Members.Add(companyMemberData);
                        db.SaveChanges();
                    }

                    CharactersTablet.ChangeCharacterTabletAppInstallState(charId, "company", true);
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static void RemoveServerCompanyMember(int companyId, int charId)
        {
            try
            {
                if (companyId == 0 || charId == 0) return;
                var companyMemberData = ServerCompanysMember_.FirstOrDefault(x => x.companyId == companyId && x.charId == charId);
                if(companyMemberData != null)
                {
                    ServerCompanysMember_.Remove(companyMemberData);
                    using (gtaContext db = new gtaContext())
                    {
                        db.Server_Company_Members.Remove(companyMemberData);
                        db.SaveChanges();
                    }
                    CharactersTablet.ChangeCharacterTabletAppInstallState(charId, "company", false);
                }
            }
            catch(Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static void ChangeServerCompanyOwner(int companyId, int newOwner)
        {
            try
            {
                if (companyId == 0 || newOwner == 0) return;
                var companyData = ServerCompanysData_.FirstOrDefault(x => x.id == companyId);
                if(companyData != null)
                {
                    companyData.companyOwnerId = newOwner;
                    using (gtaContext db = new gtaContext())
                    {
                        db.Server_Companys.Update(companyData);
                        db.SaveChanges();
                    }
                }
            }
            catch(Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static void ChangeServerCompanyMemberRank(int companyId, int charId, int newRank)
        {
            try
            {
                if (companyId == 0 || charId == 0) return;
                var companyMemberData = ServerCompanysMember_.FirstOrDefault(x => x.companyId == companyId && x.charId == charId);
                if(companyMemberData != null)
                {
                    companyMemberData.rank = newRank;
                    using (gtaContext db = new gtaContext())
                    {
                        db.Server_Company_Members.Update(companyMemberData);
                        db.SaveChanges();
                    }
                }
            }
            catch(Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static bool IsCharacterInAnyServerCompany(int charId)
        {
            try
            {
                if (charId == 0) return false;
                var companyMemberData = ServerCompanysMember_.FirstOrDefault(x => x.charId == charId);
                if(companyMemberData != null)
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

        public static int GetCharacterServerCompanyId(int charId)
        {
            try
            {
                if (charId == 0) return 0;
                var companyMemberData = ServerCompanysMember_.FirstOrDefault(x => x.charId == charId);
                if(companyMemberData != null)
                {
                    return companyMemberData.companyId;
                }
            }
            catch(Exception e)
            {
                Alt.Log($"{e}");
            }
            return 0;
        }

        public static string GetServerCompanyMembers(int companyId)
        {
            if (companyId == 0) return "";

            var items = ServerCompanysMember_.Where(x => x.companyId == companyId).Select(x => new
            {
                x.companyId,
                x.charId,
                charName = Characters.GetCharacterName(x.charId),
                x.rank,
                date = x.invitedTimestamp.ToString("dd.MM.yyyy"),
                time = x.invitedTimestamp.ToString("HH.mm"),
            }).OrderBy(x => x.charName).ToList();            

            return JsonConvert.SerializeObject(items);
        }

        public static string GetServerCompanyInfos(int companyId)
        {
            if (companyId == 0) return "";

            var items = ServerCompanysData_.Where(x => x.id == companyId).Select(x => new
            {
                x.companyName,
                ownerName = Characters.GetCharacterName(x.companyOwnerId),
                memberAmount = GetServerCompanyMemberCount(companyId),
            }).ToList();

            return JsonConvert.SerializeObject(items);
        }

        public static int GetServerCompanyMemberCount(int companyId)
        {
            int Count = 0;
            if (companyId == 0) return Count;
            Count = ServerCompanysMember_.Where(x => x.companyId == companyId).Count();
            return Count;
        }

        public static int GetCharacterServerCompanyRank(int charId)
        {
            if (charId == 0) return 0;
            var memberData = ServerCompanysMember_.FirstOrDefault(x => x.charId == charId);
            if(memberData != null)
            {
                return memberData.rank;
            }
            return 0;
        }

        public static int GetServerCompanyMoney(int companyId)
        {
            if (companyId <= 0) return 0;
            var companyData = ServerCompanysData_.FirstOrDefault(x => x.id == companyId);
            if(companyData != null)
            {
                return companyData.companyMoney;
            }
            return 0;
        }

        public static void SetServerCompanyMoney(int companyId, int money)
        {
            try
            {
                if (companyId <= 0) return;
                var companyData = ServerCompanysData_.FirstOrDefault(x => x.id == companyId);
                if(companyData != null)
                {
                    companyData.companyMoney = money;
                    using (gtaContext db = new gtaContext())
                    {
                        db.Server_Companys.Update(companyData);
                        db.SaveChanges();
                    }
                }
            }
            catch(Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static Position GetServerCompanyPosition(int companyId)
        {
            Position pos = new Position(0, 0, 0);
            if (companyId == 0) return pos;
            var companyData = ServerCompanysData_.FirstOrDefault(x => x.id == companyId);
            if(companyData != null)
            {
                pos = new Position(companyData.posX, companyData.posY, companyData.posZ);
            }
            return pos;
        }

        public static string GetServerCompanyGivenLicense(int companyId)
        {
            string License = "None";
            if (companyId <= 0) return License;
            var companyData = ServerCompanysData_.FirstOrDefault(x => x.id == companyId);
            if(companyData != null)
            {
                return companyData.givenLicense;
            }
            return License;
        }

        public static string GetServerCompanyName(int companyId)
        {
            if (companyId == 0) return "";
            var companyData = ServerCompanysData_.FirstOrDefault(x => x.id == companyId);
            if(companyData != null)
            {
                return companyData.companyName;
            }
            return "";
        }

        public static string GetServerCompanyRankName(int rankId)
        {
            switch(rankId)
            {
                case 0: return "Mitarbeiter";
                case 1: return "Stelv. Geschäftsführer";
                case 2: return "Geschäftsführer";
            }
            return "";
        }
    }
}
