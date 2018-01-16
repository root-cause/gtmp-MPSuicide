using System;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared;

namespace Suicide
{
    public class Main : Script
    {
        public Main()
        {
            API.onPlayerDeath += Suicide_PlayerDeath;
            API.onClientEventTrigger += Suicide_EventTrigger;
            API.onResourceStop += Suicide_Exit;
        }

        public void ResetSuicide(Client player)
        {
            if (player.hasData("Suicide_Active"))
            {
                player.triggerEvent("SuicideAnimReporter", false, string.Empty, 1.0);
                player.resetData("Suicide_Active");
                player.stopAnimation();
            }
        }

        #region Events
        public void Suicide_PlayerDeath(Client player, NetHandle killer, int weapon)
        {
            ResetSuicide(player);
        }

        public void Suicide_EventTrigger(Client player, string eventName, params object[] args)
        {
            switch (eventName)
            {
                case "Suicide_Begin":
                {
                    if (args.Length < 1 || player.hasData("Suicide_Active")) return;

                    if (Convert.ToBoolean(args[0]))
                    {
                        player.playAnimation("MP_SUICIDE", "PISTOL", 270540800);
                        player.triggerEvent("SuicideAnimReporter", true, "PISTOL", 0.365);
                    }
                    else
                    {
                        player.giveWeapon(WeaponHash.Unarmed, 1, true, true);
                        player.playAnimation("MP_SUICIDE", "PILL", 270540800);
                        player.triggerEvent("SuicideAnimReporter", true, "PILL", 0.536);
                    }

                    player.setData("Suicide_Active", true);
                    break;
                }

                case "Suicide_Shoot":
                {
                    if (!player.hasData("Suicide_Active")) return;
                    API.sendNativeToPlayersInRangeInDimension(player.position, 150f, player.dimension, Hash.SET_PED_SHOOTS_AT_COORD, player.handle, 0f, 0f, 0f, false);
                    break;
                }

                case "Suicide_Cancel":
                {
                    ResetSuicide(player);
                    break;
                }
            }
        }

        public void Suicide_Exit()
        {
            foreach (Client player in API.getAllPlayers()) ResetSuicide(player);
        }
        #endregion
    }
}