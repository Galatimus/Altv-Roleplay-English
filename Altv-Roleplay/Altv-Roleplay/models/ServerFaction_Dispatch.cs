using AltV.Net.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Altv_Roleplay.models
{
    public partial class ServerFaction_Dispatch
    {
        public int senderCharId { get; set; }
        public int factionId { get; set; }
        public string message { get; set; }
        public string Date { get; set; }
        public Position Destination { get; set; }
    }
}
