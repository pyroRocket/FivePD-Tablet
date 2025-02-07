using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using FivePD.API;
using FivePD.API.Utils;

namespace FivePDTablet
{
    public class Tablet : FivePD.API.Plugin
    {
        private int propHandle = 0; // To track the prop entity
        public bool mdtOpen;

        public Tablet()
        {
            Debug.WriteLine("FivePD Tablet v1.1");
            Tick += OnTick;
        }

        private async Task OnTick()
        {
            //Checks if the player is in a police vehicle
            if (!(API.IsPedInAnyPoliceVehicle(API.GetPlayerPed(-1))))
            {
                if (!mdtOpen && API.IsControlJustPressed(0, 305)) //305 is B but this can be set to any FiveM keybind
                {
                    //Opens Tablet
                    API.SendNuiMessage("{\"type\":\"FIVEPD::Computer::UI\",\"display\":true}");

                    //Set NUI Focus for mouse use
                    API.SetNuiFocus(true, true);

                    mdtOpen = true;

                    //True to start animation and spawn prop
                    TabletAnimation(Player.Local, true);
                }

                //If ESC is pressed in game when MDT is opened then MDT goes off and so does the animation
                if (!API.IsNuiFocused() && mdtOpen)
                {
                    mdtOpen = false;
                    TabletAnimation(Player.Local, false);
                }
            }
        }

        private async void TabletAnimation([FromSource] Player player, bool status)
        {
            int playerPed = API.GetPlayerPed(player.Handle);

            if (status)
            {
                string animDict = "amb@code_human_in_bus_passenger_idles@female@tablet@base";

                // Load animation dictionary
                if (!API.HasAnimDictLoaded(animDict))
                {
                    API.RequestAnimDict(animDict);
                    while (!API.HasAnimDictLoaded(animDict))
                    {
                        await BaseScript.Delay(100);
                    }
                }

                // Play animation
                API.TaskPlayAnim(playerPed, "amb@code_human_in_bus_passenger_idles@female@tablet@base", "base", 8.0f, -8.0f, -1, 49, 0, false, false, false);

                // Load and attach prop
                uint modelHash = (uint)API.GetHashKey("prop_cs_tablet");

                if (!API.HasModelLoaded(modelHash))
                {
                    API.RequestModel(modelHash);
                    while (!API.HasModelLoaded(modelHash))
                    {
                        await BaseScript.Delay(100);
                    }
                }

                // Create prop
                Vector3 playerPos = API.GetEntityCoords(playerPed, true);
                propHandle = API.CreateObject((int)modelHash, playerPos.X, playerPos.Y, playerPos.Z, true, true, false);

                API.AttachEntityToEntity(propHandle, playerPed, API.GetPedBoneIndex(playerPed, 18905),
                     0.12f, 0.02f, 0.12f,  // Position Offset (X, Y, Z) - adjust these if needed
                     0, 15f, 180f,        // Rotation (Pitch, Roll, Yaw) - Adjust pitch here
                true, true, false, true, 1, true);
            }
            else if(!status)
            {
                //Deletes Prop
                API.DeleteObject(ref propHandle);
                //Clears Animation
                API.ClearPedTasks(playerPed);
            }
        }
    }
}