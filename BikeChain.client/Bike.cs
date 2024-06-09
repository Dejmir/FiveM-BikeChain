using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using static BikeChain.client.Main_cl;

namespace BikeChain.client
{
    public class Bike
    {
        public Bike(int netId, bool isChainOut, int poppingRate, DateTime poppingIncreaseDisabledUntil)
        {
            NetId = netId;
            IsChainOut = isChainOut;
            PoppingRate = poppingRate;
            PoppingIncreaseDisabledUntil = poppingIncreaseDisabledUntil;
        }

        public int NetId { get; private set; }
        public bool IsChainOut { get; set; }
        public int PoppingRate { get; set; } //0-100
        public DateTime PoppingIncreaseDisabledUntil { get; set; }
        public long PoppingIncreaseDisabledUntilBinary { get; set; }

        public void IncreasePoppingRate()
        {
            int rate = rng.Next((int)jconfig["IncreasePoppingRate"].First, (int)jconfig["IncreasePoppingRate"].Last+1);
            BaseScript.TriggerServerEvent("BikeChain:server:IncreasePoppingRate", this.NetId, rate);
        } 
        public void ChainPopOut()
        {
            BaseScript.TriggerServerEvent("BikeChain:server:ChainOut", this.NetId);
        }
        
        public void PutChainBack()
        {
            BaseScript.TriggerServerEvent("BikeChain:server:PutChainBack", this.NetId);
        }
        public void SprayChain(int amount)
        {
            BaseScript.TriggerServerEvent("BikeChain:server:SprayChain", this.NetId, amount);
        }

        /*public static void Get(int netId, ref Bike bike)
        {
            bool finised = false;

            Bike _bike = null;
            BaseScript.TriggerEvent("BikeChain:client:LoadFromNetId", netId, new Action<dynamic>((arg) =>
            {
                _bike = new Bike((int)arg.NetId, (bool)arg.IsChainOut, (int)arg.PoppingRate, (DateTime)arg.PoppingIncreaseDisabledUntil);
                finised = true;
                Debug.WriteLine(_bike.IsChainOut.ToString());
            }));
            while (!finised) Delay(50).GetAwaiter().GetResult();
            bike = _bike;
        }*/

        /*public static async Task<Bike> LoadFromNetId(int netId)
        {
            Debug.WriteLine("-1");
            bool finised = false;

            Bike bike = new Bike();
            bike.NetId = netId;

            Debug.WriteLine("0");
            BaseScript.TriggerEvent("BikeChain:client:LoadFromNetId", new Action<dynamic>((arg) =>
            {
                Debug.WriteLine("1");
                bike.PoppingRate = (int)arg.PoppingRate;
                bike.IsChainOut = (bool)arg.IsChainOut;
                finised = true;
            }));
            Debug.WriteLine("2");
            while (!finised) await BaseScript.Delay(50);
            Debug.WriteLine("3");

            return bike;
        }*/
    }
}
