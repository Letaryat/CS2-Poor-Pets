using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using CS2_Poor_Pets.Core;

namespace CS2_Poor_Pets
{
    public class CommandManager(CS2_Poor_PetsPlugin plugin)
    {
        private readonly CS2_Poor_PetsPlugin _plugin = plugin;
        public void RegisterCommands()
        {
            _plugin.AddCommand("css_pets", "Pet menu", OnPetsCommand);
        }
        private void OnPetsCommand(CCSPlayerController? player, CommandInfo commandInfo)
        {
            if (player == null) return;
            var pawn = player.PlayerPawn.Value;
            if (pawn == null || !pawn.IsValid) return;

            _plugin.MenuManager!.CreateMainMenu(player);

            return;
        }


    }
}