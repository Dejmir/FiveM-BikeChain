using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using CitizenFX.Core;
using CitizenFX.Core.Native;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BikeChain.client
{
    public class Main_cl : BaseScript
    {
        public static dynamic ESX;

        public static JObject jconfig = null;
        public static HashSet<Bike> bikes = new HashSet<Bike>();

        public static Random rng = new Random();

        static bool puttingBack = false;
        static bool spraying = false;

        static int sprayResult = -1;
        static int poppingReduced = -1;

        [EventHandler("onClientResourceStart")]
        private void Main(string resName)
        {
            if (API.GetCurrentResourceName() != resName) return;

            ESX = Exports["es_extended"].getSharedObject();
            string configText = Function.Call<string>(Hash.LOAD_RESOURCE_FILE, resName, "config.json");
            jconfig = JObject.Parse(configText);

            API.RequestAnimDict((string)jconfig["AnimationSpraying"].Last);
            API.RequestAnimDict((string)jconfig["AnimationPuttingChain"].Last);
            API.RequestModel((uint)API.GetHashKey((string)jconfig["SprayProp"]));

            Tick += MainLoop;
            Tick += CheckLoop;
            Tick += IncreaseLoop;
            //Tick += AddBikesLoop;
        }

        private async Task MainLoop()
        {
            if (!LocalPlayer.Character.IsOnBike) return;
            Bike bike = bikes.FirstOrDefault(x => x.NetId == LocalPlayer.Character.CurrentVehicle.NetworkId);
            if (bike != null && bike.IsChainOut)
            {
                API.DisableControlAction(0, 102, true); //Jumping [space]
                API.DisableControlAction(0, 136, true); //Driving forward [W]
                API.DisableControlAction(0, 137, true); //Driving fast [caps]
            }
        }

        private async Task CheckLoop()
        {
            await Delay(100);
            if (!LocalPlayer.Character.IsOnBike) return;
            Bike bike = bikes.FirstOrDefault(x => x.NetId == LocalPlayer.Character.CurrentVehicle.NetworkId);
            if(bike == null)
            {
                bool finished = false;
                TriggerEvent("BikeChain:client:LoadFromNetId", LocalPlayer.Character.CurrentVehicle.NetworkId, new Action<dynamic>((arg) =>
                {
                    TriggerEvent("BikeChain:client:AddQtargetSpray", LocalPlayer.Character.CurrentVehicle.Handle);
                    bike = new Bike((int)arg.NetId, (bool)arg.IsChainOut, (int)arg.PoppingRate, DateTime.FromBinary((long)arg.PoppingIncreaseDisabledUntilBinary));
                    bikes.Add(bike);
                    finished = true;
                }));
                while (!finished) await Delay(100);
                return;
            }

            int i = (int)jconfig["PoppingCheckInterval"];
            while(i > 0)
            {
                if (!LocalPlayer.Character.IsOnBike) break;
                i--;
                await Delay(1000);
            }
            //await Delay((int)jconfig["PoppingCheckInterval"] *1000);
            if(bike.PoppingRate >= rng.Next(1, 101) && !bike.IsChainOut)
            {
                ESX.ShowNotification((string)jconfig["Notifications"]["ChainOut"], "error", 7500);
                TriggerEvent("BikeChain:client:AddQtargetChain", API.NetToVeh(bike.NetId));
                bike.ChainPopOut();
            }
        }
        
        private async Task IncreaseLoop()
        {
            await Delay(100);
            if (!LocalPlayer.Character.IsOnBike) return;
            Bike bike = bikes.FirstOrDefault(x => x.NetId == LocalPlayer.Character.CurrentVehicle.NetworkId);
            if (bike == null || bike.IsChainOut || bike.PoppingIncreaseDisabledUntil > DateTime.Now) return;

            int i = (int)jconfig["PoppingIncreasingInterval"];
            while (i > 0)
            {
                if (!LocalPlayer.Character.IsOnBike) break;
                i--;
                await Delay(1000);
            }
            //await Delay((int)jconfig["PoppingIncreasingInterval"] *1000);
            if (Math.Abs(i - ((int)jconfig["PoppingIncreasingInterval"] * 1000)) < 30) return;
            if (!bike.IsChainOut && bike.PoppingRate+(int)jconfig["IncreasePoppingRate"].Last >= (int)jconfig["RustingNotificationWhen"]) 
                ESX.ShowNotification((string)jconfig["Notifications"]["Rusting"], "info", 5000);
            if(!bike.IsChainOut) bike.IncreasePoppingRate();
        }
        
        /*private async Task AddBikesLoop()
        {
            await Delay(5000);
            foreach (var veh in World.GetAllVehicles().Where(x => API.IsThisModelABike((uint)x.Model.Hash)))
            {
                if (bikes.FirstOrDefault(x => x.NetId == veh.NetworkId) != null) continue;
                TriggerEvent("BikeChain:client:LoadFromNetId", veh.NetworkId, new Action<dynamic>((arg) =>
                {
                    TriggerEvent("BikeChain:client:AddQtargetSpray", veh.Handle);
                    Bike bike = new Bike((int)arg.NetId, (bool)arg.IsChainOut, (int)arg.PoppingRate, DateTime.FromBinary((long)arg.PoppingIncreaseDisabledUntilBinary));
                    bikes.Add(bike);
                }));
                await Delay(100);
            }
        }*/

        [EventHandler("BikeChain:client:PutChainBackFromQtarget")]
        private async void PutChainBack(int veh)
        {
            if (puttingBack) return;
            puttingBack = true;
            int seconds = rng.Next((int)jconfig["PuttingChainTime"].First, (int)jconfig["PuttingChainTime"].Last+1);
            /*var dir = API.GetEntityCoords(veh, true) - LocalPlayer.Character.Position;
            LocalPlayer.Character.Task.AchieveHeading((float)(Math.Atan2(dir.X, dir.Y) * (180.0 / Math.PI)));
            await Delay(750);*/
            LocalPlayer.Character.Task.ClearAll();
            bool cancelled = false;
            string[] anim = { (string)jconfig["AnimationPuttingChain"].First, (string)jconfig["AnimationPuttingChain"].Last };
            LocalPlayer.Character.Task.PlayAnimation(anim[1], anim[0], 8f, seconds*1000, AnimationFlags.Loop);
            await Task.Factory.StartNew(async () =>
            {
                await Delay(1000);
                DateTime until = DateTime.Now.AddSeconds(seconds-2);
                while (until > DateTime.Now) {
                    if (API.IsControlPressed(0, 32) || API.IsControlPressed(0, 33) || API.IsControlPressed(0, 34) || API.IsControlPressed(0, 35))
                        LocalPlayer.Character.Task.ClearAll();
                    if (!API.IsEntityPlayingAnim(LocalPlayer.Character.Handle, anim[1], anim[0], 3) && !cancelled) 
                    {
                        cancelled = true;
                        puttingBack = false;
                        StopUITimer();
                    }
                    await Delay(500);
                }
            });
            RunUITimer(seconds);
            await Delay(seconds*1000);
            puttingBack = false;
            if (cancelled) return;
            ESX.ShowNotification((string)jconfig["Notifications"]["ChainBack"], "success", 5000);
            TriggerEvent("BikeChain:client:RemoveQtargetChain", veh);
            bikes.First(x => x.NetId == API.VehToNet(veh)).PutChainBack();
        }
        
        [EventHandler("BikeChain:client:SprayFromQtarget")]
        private async void Spray(int veh)
        {
            if(spraying) return;
            spraying = true;
            int seconds = rng.Next((int)jconfig["SprayingTime"].First, (int)jconfig["SprayingTime"].Last + 1);
            LocalPlayer.Character.Task.ClearAll();
            bool cancelled = false;
            string[] anim = { (string)jconfig["AnimationSpraying"].First, (string)jconfig["AnimationSpraying"].Last };
            LocalPlayer.Character.Task.PlayAnimation(anim[1], anim[0], 1f, seconds * 1000, AnimationFlags.Loop);
            var prop = await World.CreateProp(new Model((string)jconfig["SprayProp"]), LocalPlayer.Character.Position, false, false);
            prop.AttachTo(LocalPlayer.Character.Bones[Bone.IK_R_Hand], new Vector3(0f, -0.05f, -0.02f), new Vector3(-90f, 0f, 0f));
            await Task.Factory.StartNew(async () =>
            {
                await Delay(1000);
                DateTime until = DateTime.Now.AddSeconds(seconds - 2);
                while (until > DateTime.Now)
                {
                    if (API.IsControlPressed(0, 32) || API.IsControlPressed(0, 33) || API.IsControlPressed(0, 34) || API.IsControlPressed(0, 35))
                        LocalPlayer.Character.Task.ClearAll();
                    if (!API.IsEntityPlayingAnim(LocalPlayer.Character.Handle, anim[1], anim[0], 3) && !cancelled)
                    {
                        prop.Delete();
                        cancelled = true;
                        spraying = false;
                        StopUITimer();
                    }
                    await Delay(500);
                }
            });
            RunUITimer(seconds);
            await Delay(seconds * 1000);
            spraying = false;
            if (cancelled) return;
            prop.Delete();
            TriggerServerEvent("BikeChain:server:SprayResult", API.VehToNet(veh));
            while (sprayResult == -1 || poppingReduced == -1) await Delay(50);
            if(sprayResult == 1) ESX.ShowNotification((string)jconfig["Notifications"]["ChainSpray"], "success", 5000);
            else if(sprayResult == 0) ESX.ShowNotification((string)jconfig["Notifications"]["ChainSprayNotEnough"], "info", 5000);
            else if(sprayResult == 2) ESX.ShowNotification((string)jconfig["Notifications"]["NoSpray"], "error", 5000);
            bikes.First(x => x.NetId == API.VehToNet(veh)).SprayChain(poppingReduced);
            sprayResult = -1;
            poppingReduced = -1;
        }

        [EventHandler("BikeChain:client:SprayResult")]
        private void SprayResult(int _sprayResult, int _poppingReduced)
        {
            sprayResult = _sprayResult;
            poppingReduced = _poppingReduced;
        }




        private void RunUITimer(int seconds)
        {
            API.SendNuiMessage($"{{\"action\": \"AddTimer\", \"seconds\": {seconds}}}");
        }
        private void StopUITimer()
        {
            API.SendNuiMessage($"{{\"action\": \"RemoveTimer\"}}");
        }
    }
}
