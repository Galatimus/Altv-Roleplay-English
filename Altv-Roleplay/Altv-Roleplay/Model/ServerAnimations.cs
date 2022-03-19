using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using Altv_Roleplay.models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altv_Roleplay.Model
{
    class ServerAnimations
    {
        public static List<Server_Animations> ServerAnimations_ = new List<Server_Animations>();

        public static string GetAllServerAnimations()
        {
            return JsonConvert.SerializeObject(ServerAnimations_);
        }

        public static void RequestAnimationMenuContent(IPlayer player)
        {
            try
            {
                var items = ServerAnimations_.Select(x => new
                {
                    x.animId,
                    x.animName,
                    x.displayName,
                    x.animFlag,
                    x.animDict,
                    x.duration,
                }).ToList();
                var itemCount = (int)items.Count;
                var iterations = Math.Floor((decimal)(itemCount / 20));
                var rest = itemCount % 20;
                for(var i = 0; i < iterations; i++)
                {
                    var skip = i * 20;
                    player.EmitLocked("Client:Animations:setupItems", JsonConvert.SerializeObject(items.Skip(skip).Take(20).ToList()));
                }

                if (rest != 0) player.EmitLocked("Client:Animations:setupItems", JsonConvert.SerializeObject(items.Skip((int)iterations * 20).ToList()));
            }
            catch(Exception e)
            {
                Alt.Log($"{e}");
            }
        }
    }
}
