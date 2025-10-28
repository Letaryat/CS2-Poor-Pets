using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CS2_Poor_Pets.Core;
using CS2MenuManager.API.Menu;

namespace CS2_Poor_Pets;

public class MenuManager(CS2_Poor_PetsPlugin plugin)
{
    private readonly CS2_Poor_PetsPlugin _plugin = plugin;
    public void CreateMainMenu(CCSPlayerController player)
    {
        WasdMenu menu = new(_plugin.Localizer["ChooseCategory"], _plugin);

        if (string.IsNullOrEmpty(_plugin.Config.vipFlag))
        {
            menu.AddItem(_plugin.Localizer["Pets4AllMenu"], (p, o) =>
            {
                CreatePetsMenu(p, null, _plugin.Localizer["Pets4AllMenu"], menu);
            });
        }
        else
        {
            menu.AddItem(_plugin.Localizer["Pets4AllMenu"], (p, o) =>
            {
                CreatePetsMenu(p, false, _plugin.Localizer["Pets4AllMenu"], menu);
            });

            menu.AddItem(_plugin.Localizer["Pets4VipsMenu"], (p, o) =>
            {
                CreatePetsMenu(p, true, _plugin.Localizer["Pets4VipsMenu"], menu);
            },
            disableOption: AdminManager.PlayerHasPermissions(player, _plugin.Config.vipFlag)
                ? CS2MenuManager.API.Enum.DisableOption.None
                : CS2MenuManager.API.Enum.DisableOption.DisableHideNumber);
        }

        menu.AddItem(_plugin.Localizer["RemovePetMenu"], (p, o) =>
        {
            _plugin.PetManager!.RemovePlayerPet(p);
        },
        disableOption: PetManager.PlayerChosenPet.ContainsKey(player)
                ? CS2MenuManager.API.Enum.DisableOption.None
                : CS2MenuManager.API.Enum.DisableOption.DisableHideNumber);

        menu.Display(player, 0);
    }

    private void CreatePetsMenu(CCSPlayerController player, bool? isVipMenu, string title, WasdMenu? parentMenu)
    {
        WasdMenu menu = new(title, _plugin);
        menu.PrevMenu = parentMenu;

        foreach (var tuple in _plugin.Config.Pets.Select((pet, idx) => new { pet, idx }))
        {
            if (isVipMenu == true && !tuple.pet.isVipOnly) continue;
            if (isVipMenu == false && tuple.pet.isVipOnly) continue;

            menu.AddItem(tuple.pet.petName, (p, o) =>
            {
                CreatePetsOptions(p, tuple.idx);
                o.PostSelectAction = CS2MenuManager.API.Enum.PostSelectAction.Nothing;
            });
        }

        menu.Display(player, 0);
    }


    private void CreatePetsOptions(CCSPlayerController player, int petId)
    {
        _plugin.PetManager!.CreatePetForPlayer(player, petId);
        player.PrintToChat(_plugin.Localizer["PlayerSetPetNotification"]);
    }

}