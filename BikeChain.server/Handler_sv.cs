using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CitizenFX.Core;

using static BikeChain.server.Main_sv;

namespace BikeChain.server
{
    public class Handler_sv : BaseScript
    {
        [EventHandler("BikeChain:server:IncreasePoppingRate")]
        private void IncreasePopping(int netId, int value)
        {
            //value = MathUtil.Clamp(value, 0, 100);
            bikes.First(x => x.NetId == netId).PoppingRate += value;
            if (bikes.First(x => x.NetId == netId).PoppingRate > 100) bikes.First(x => x.NetId == netId).PoppingRate = 100;
            TriggerClientEvent("BikeChain:client:IncreasePoppingRate", netId, value);
        }

        [EventHandler("BikeChain:server:ChainOut")]
        private void ChainOut(int netId)
        {
            bikes.First(x => x.NetId == netId).IsChainOut = true;
            TriggerClientEvent("BikeChain:client:ChainOut", netId);
        }

        [EventHandler("BikeChain:server:PutChainBack")]
        private void PutChainBack(int netId)
        {
            bikes.First(x => x.NetId == netId).IsChainOut = false;
            TriggerClientEvent("BikeChain:client:PutChainBack", netId);
        }

        [EventHandler("BikeChain:server:SprayChain")]
        private void SprayChain(int netId, int value)
        {
            //value = MathUtil.Clamp(value, 0, 100);
            bikes.First(x => x.NetId == netId).PoppingRate -= value;
            bikes.First(x => x.NetId == netId).PoppingIncreaseDisabledUntil = DateTime.Now.AddSeconds((int)jconfig["SpraySuspendPoppingIncreasing"]);
            if (bikes.First(x => x.NetId == netId).PoppingRate < 0) bikes.First(x => x.NetId == netId).PoppingRate = 0;
            TriggerClientEvent("BikeChain:client:SprayChain", netId, value);
        }
    }
}
