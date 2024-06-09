using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CitizenFX.Core;
using CitizenFX.Core.Native;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BikeChain.server
{
    public class Main_sv : BaseScript
    {
        public static JObject jconfig = null;
        public static List<Bike> bikes = new List<Bike>();

        [EventHandler("onResourceStart")]
        private void Main(string resName)
        {
            if (API.GetCurrentResourceName() != resName) return;

            string configText = Function.Call<string>(Hash.LOAD_RESOURCE_FILE, resName, "config.json");
            jconfig = JObject.Parse(configText);

            //Tick += InventoryLoop;
        }

       /* private async Task InventoryLoop()
        {
            await Delay(5000);
            foreach (var player in Players)
            {
                dynamic items = Exports["ox_inventory"].Search(int.Parse(player.Handle), 1, "wd40");
                foreach (var item in items)
                {
                    JObject jitem = JObject.FromObject(item);
                    if (jitem.SelectToken("metadata").SelectToken("amount", false) != null) continue;
                    JObject jobject = new JObject
                    {
                        { "amount", (int)jconfig["SprayDefaultAmount"] }
                    };
                    Exports["ox_inventory"].SetMetadata(int.Parse(player.Handle), (int)item.slot, jobject.ToObject<Dictionary<string, dynamic>>());
                }
            }
        }*/

        
        [EventHandler("BikeChain:server:LoadFromNetId")]
        private void LoadBikeToClient([FromSource]Player player, string guid, int netId)
        {
            Bike bike = null;
            if (bikes.FirstOrDefault(x => x.NetId == netId) != null) bike = bikes.First(x => x.NetId == netId);
            else
            {
                bike = new Bike(netId, (int)jconfig["DefaultPoppingRate"], false);
                bikes.Add(bike);
            }

            player.TriggerEvent("BikeChain:client:LoadFromNetIdResponse", guid, bike.NetId, bike.IsChainOut, bike.PoppingRate, bike.PoppingIncreaseDisabledUntil.ToBinary());
        }

        [EventHandler("BikeChain:server:SprayResult")]
        private void SprayResult([FromSource]Player player, int netVeh)
        {
            int result = 0;
            int reduced = 0;
            Bike bike = bikes.First(x => x.NetId == netVeh);

            PrepareSpray(player);

            dynamic items = Exports["ox_inventory"].Search(int.Parse(player.Handle), 1, "wd40");
            dynamic item = null;
            foreach (var _item in items)
            {
                JObject jitem = JObject.FromObject(_item);
                if ((int)jitem["count"] > 0 && (int)jitem["metadata"]["amount"] > 0) { item = _item; break; }
            }
            int amount = item != null ? item.metadata.amount : 0;
            while(amount - (int)jconfig["SprayConsume"] >= 0)
            {
                reduced += 10;
                amount -= (int)jconfig["SprayConsume"];
                bike.PoppingRate -= 10;

                if (bike.PoppingRate <= 0) { result = 1; break; }
            }
            if (bike.PoppingRate < 0) bike.PoppingRate = 0;

            JObject jobject = new JObject
            {
                { "amount", amount }
            };
            if(item != null && reduced > 0) Exports["ox_inventory"].SetMetadata(int.Parse(player.Handle), (int)item.slot, jobject.ToObject<Dictionary<string, dynamic>>());

            if (reduced == 0) result = 2;
            player.TriggerEvent("BikeChain:client:SprayResult", result, reduced);
        }

        private void PrepareSpray(Player player)
        {
            dynamic items = Exports["ox_inventory"].Search(int.Parse(player.Handle), 1, "wd40");
            foreach (var item in items)
            {
                JObject jitem = JObject.FromObject(item);
                if (jitem.SelectToken("metadata").SelectToken("amount", false) != null) continue;
                JObject jobject = new JObject
                    {
                        { "amount", (int)jconfig["SprayDefaultAmount"] }
                    };
                Exports["ox_inventory"].SetMetadata(int.Parse(player.Handle), (int)item.slot, jobject.ToObject<Dictionary<string, dynamic>>());
            }
        }
    }
}
