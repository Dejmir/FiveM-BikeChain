using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeChain.server
{
    public class Bike
    {
        public Bike(int netId, int poppingRate, bool chainOut)
        {
            NetId = netId;
            PoppingRate = poppingRate;
            IsChainOut = chainOut;
        }

        public int NetId { get; private set; }
        public bool IsChainOut { get; set; }
        public int PoppingRate { get; set; } //0-100
        public DateTime PoppingIncreaseDisabledUntil { get; set; } = DateTime.Now;
    }
}
