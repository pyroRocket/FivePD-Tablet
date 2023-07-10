using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using FivePD.API;
using FivePD.API.Utils;

namespace FivePDTablet
{
    public class Tablet : FivePD.API.Plugin
    {
        public Tablet()
        {
            Tick += OnTick;
        }

        private async Task OnTick()
        {
            //TODO: On duty check
            if (!(API.IsPedInAnyPoliceVehicle(API.GetPlayerPed(-1))))
            {
                if (API.IsControlJustPressed(0, 305)) //305 is B but this can be set to any FiveM keybind
                {
                    // To Open Computer
                    API.SendNuiMessage("{\"type\":\"FIVEPD::Computer::UI\",\"display\":true}");
                    //Set NUI Focus for mouse use
                    API.SetNuiFocus(true, true);
                }
                else if (API.IsControlJustPressed(0, 305))
                {
                    // To disable
                    API.SendNuiMessage("{\"type\":\"FIVEPD::Computer::UI\",\"display\":false}");
                    API.SetNuiFocus(false, false);
                }                
            }
        }
        
    }
}