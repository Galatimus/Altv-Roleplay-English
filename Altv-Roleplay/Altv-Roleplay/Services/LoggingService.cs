using AltV.Net;
using Altv_Roleplay.Model;
using Altv_Roleplay.models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Altv_Roleplay.Services
{
    public static class LoggingService
    {
        public static void NewLoginLog(string name, ulong socialclub, string ip, ulong hwid, bool success, string text)
        {
            using (gtaContext db = new gtaContext())
            {
                db.LogsLogin.Add(new LogsLogin
                {
                    username = name,
                    socialclub = socialclub,
                    text = text,
                    address = ip,
                    hwid = hwid,
                    success = success
                });
                db.SaveChanges();
            }
        }

        public static void NewFactionLog(int factionId, int charId, int targetCharId, string type, string text)
        {
            try
            {
                if (factionId == 0 || charId == 0) return;
                var logData = new Logs_Faction
                {
                    factionId = factionId,
                    charId = charId,
                    targetCharId = targetCharId,
                    type = type,
                    text = text,
                    timestamp = DateTime.Now
                };

                ServerFactions.LogsFaction_.Add(logData);
                
                using (gtaContext db = new gtaContext())
                {
                    db.LogsFaction.Add(logData);
                    db.SaveChanges();
                }
            }
            catch(Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static void NewCompanyLog(int companyId, int charId, int targetCharId, string type, string text)
        {
            try
            {
                if (companyId <= 0 || charId <= 0) return;
                var logData = new Logs_Company
                {
                    companyId = companyId,
                    charId = charId,
                    targetCharId = targetCharId,
                    type = type,
                    text = text,
                    timestamp = DateTime.Now
                };

                ServerCompanys.LogsCompany_.Add(logData);
                using (gtaContext db = new gtaContext())
                {
                    db.LogsCompany.Add(logData);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }
    }
}
