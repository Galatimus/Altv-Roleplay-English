using Altv_Roleplay.models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altv_Roleplay.Model
{
    class ServerJobs
    {
        public static List<Server_Jobs> ServerJobs_ = new List<Server_Jobs>();

        public static string GetAllServerJobs()
        {
            var items = ServerJobs_.Select(x => new
            {
                x.jobName,
                x.jobPaycheck,
                x.jobNeededHours,
                x.jobPic,
            }).ToList();

            return JsonConvert.SerializeObject(items);
        }

        public static int GetJobPaycheck(string jobName)
        {
            if (jobName == "") return 0;
            var job = ServerJobs_.FirstOrDefault(x => x.jobName == jobName);
            if(job != null)
            {
                return job.jobPaycheck;
            }
            return 0;
        }

        public static bool ExistServerJob(string jobName)
        {
            if (jobName == "") return false;
            var job = ServerJobs_.FirstOrDefault(x => x.jobName == jobName);
            if(job != null) { return true; }
            return false;
        }

        public static int GetJobNeededHours(string jobName)
        {
            if (jobName == "") return 0;
            var job = ServerJobs_.FirstOrDefault(x => x.jobName == jobName);
            if(job != null) { return job.jobNeededHours; }
            return 0;
        }
    }
}
