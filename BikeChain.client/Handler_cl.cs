using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CitizenFX.Core;

using static BikeChain.client.Main_cl;

namespace BikeChain.client
{
    public class Handler_cl : BaseScript
    {
        [EventHandler("BikeChain:client:IncreasePoppingRate")]
        private void IncreasePopping(int netId, int value)
        {
            if(bikes.FirstOrDefault(x => x.NetId == netId) != null) bikes.First(x => x.NetId == netId).PoppingRate += value;
            else
            {
                BaseScript.TriggerEvent("BikeChain:client:LoadFromNetId", netId, new Action<dynamic>((arg) =>
                {
                    Bike bike = new Bike((int)arg.NetId, (bool)arg.IsChainOut, (int)arg.PoppingRate, DateTime.FromBinary((long)arg.PoppingIncreaseDisabledUntilBinary));
                    bikes.Add(bike);
                }));
            }
            if (bikes.First(x => x.NetId == netId).PoppingRate > 100) bikes.First(x => x.NetId == netId).PoppingRate = 100;
        }

        [EventHandler("BikeChain:client:ChainOut")]
        private void ChainOut(int netId)
        {
            if(bikes.FirstOrDefault(x => x.NetId == netId) != null) bikes.First(x => x.NetId == netId).IsChainOut = true;
            else
            {
                BaseScript.TriggerEvent("BikeChain:client:LoadFromNetId", netId, new Action<dynamic>((arg) =>
                {
                    Bike bike = new Bike((int)arg.NetId, (bool)arg.IsChainOut, (int)arg.PoppingRate, DateTime.FromBinary((long)arg.PoppingIncreaseDisabledUntilBinary));
                    bikes.Add(bike);
                }));
            }
        }

        [EventHandler("BikeChain:client:PutChainBack")]
        private void PutChainBack(int netId)
        {
            if(bikes.FirstOrDefault(x => x.NetId == netId) != null) bikes.First(x => x.NetId == netId).IsChainOut = false;
            else
            {
                BaseScript.TriggerEvent("BikeChain:client:LoadFromNetId", netId, new Action<dynamic>((arg) =>
                {
                    Bike bike = new Bike((int)arg.NetId, (bool)arg.IsChainOut, (int)arg.PoppingRate, DateTime.FromBinary((long)arg.PoppingIncreaseDisabledUntilBinary));
                    bikes.Add(bike);
                }));
            }
        }

        [EventHandler("BikeChain:client:SprayChain")]
        private void SprayChain(int netId, int value)
        {
            if (bikes.FirstOrDefault(x => x.NetId == netId) != null) bikes.First(x => x.NetId == netId).PoppingRate -= value;
            else
            {
                BaseScript.TriggerEvent("BikeChain:client:LoadFromNetId", netId, new Action<dynamic>((arg) =>
                {
                    Bike bike = new Bike((int)arg.NetId, (bool)arg.IsChainOut, (int)arg.PoppingRate, DateTime.FromBinary((long)arg.PoppingIncreaseDisabledUntilBinary));
                    bikes.Add(bike);
                }));
            }
            bikes.First(x => x.NetId == netId).PoppingIncreaseDisabledUntil = DateTime.Now.AddSeconds((int)jconfig["SpraySuspendPoppingIncreasing"]);
            if (bikes.First(x => x.NetId == netId).PoppingRate < 0) bikes.First(x => x.NetId == netId).PoppingRate = 0;
        }
    }
}
