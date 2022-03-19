using AltV.Net;
using Altv_Roleplay.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altv_Roleplay.Model
{
    class CharactersMinijobs
    {
        public static List<Characters_Minijobs> CharactersMinijobsData_ = new List<Characters_Minijobs>();

        public static void CreateCharacterMinijobEntry(int charId, string job)
        {
            try
            {
                if (charId <= 0) return;
                var jobData = new Characters_Minijobs
                {
                    charId = charId,
                    jobName = job,
                    exp = 0
                };
                CharactersMinijobsData_.Add(jobData);
                using (gtaContext db = new gtaContext())
                {
                    db.Characters_Minijobs.Add(jobData);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static bool ExistCharacterMinijobEntry(int charId, string job)
        {
            if (charId <= 0) return false;
            var jobEntry = CharactersMinijobsData_.FirstOrDefault(x => x.charId == charId && x.jobName == job);
            return jobEntry != null;
        }

        public static int GetCharacterMinijobEXP(int charId, string job)
        {
            try
            {
                var jobEntry = CharactersMinijobsData_.FirstOrDefault(x => x.charId == charId && x.jobName == job);
                if(jobEntry != null)
                {
                    return jobEntry.exp;
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return 0;
        }

        public static void IncreaseCharacterMinijobEXP(int charId, string job, int amount)
        {
            try
            {
                if (charId <= 0 || job == "") return;
                var jobEntry = CharactersMinijobsData_.FirstOrDefault(x => x.charId == charId && x.jobName == job);
                if(jobEntry != null)
                {
                    jobEntry.exp += amount;
                    using (gtaContext db = new gtaContext())
                    {
                        db.Characters_Minijobs.Update(jobEntry);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }
    }
}
