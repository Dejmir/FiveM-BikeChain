using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CitizenFX.Core;

namespace BikeChain.client
{
    public class Callbacks_cl : BaseScript
    {
        private Dictionary<string, dynamic> callbacks = new Dictionary<string, dynamic>();

        [EventHandler("BikeChain:client:LoadFromNetId")]
        private async void LoadBikeServerQuery(int netId, CallbackDelegate cb)
        {
            var guid = Guid.NewGuid().ToString();
            callbacks.Add(guid, null);
            BaseScript.TriggerServerEvent("BikeChain:server:LoadFromNetId", guid, netId);
            while (callbacks[guid] == null) await Delay(50);
            cb.Invoke(callbacks[guid]);
        }
        
        [EventHandler("BikeChain:client:LoadFromNetIdResponse")]
        private void LoadBikeServerResponse(string guid, int netId, bool chainOut, int poppingRate, long poppingDisabledUntil)
        {
            DateTime date = DateTime.FromBinary(poppingDisabledUntil);
            Bike bike = new Bike(netId, chainOut, poppingRate, date);
            bike.PoppingIncreaseDisabledUntilBinary = poppingDisabledUntil;
            //Debug.WriteLine(bike.PoppingIncreaseDisabledUntil.ToString());
            callbacks[guid] = bike;
        }
    }
}
