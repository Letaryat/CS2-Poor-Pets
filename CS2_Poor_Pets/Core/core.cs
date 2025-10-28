using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.Logging;

namespace CS2_Poor_Pets.Core;

public class CS2_Poor_PetsPlugin : BasePlugin, IPluginConfig<PluginConfig>
{
    public override string ModuleName => "CS2 Poor Pets";
    public override string ModuleAuthor => "Letaryat | https://github.com/letaryat/";
    public override string ModuleDescription => "Makes a pet that follows the player around.";
    public override string ModuleVersion => "1.1";

    public required PluginConfig Config { get; set; }
    public EventManager? EventManager { get; private set; }
    public PetManager? PetManager { get; private set; }
    public new CommandManager? CommandManager { get; private set; }
    public MenuManager? MenuManager { get; private set; }

    public DatabaseManager? DatabaseManager { get; private set; }
    public static CS2_Poor_PetsPlugin? Instance { get; private set; }
    public override void Load(bool hotReload)
    {
        EventManager = new EventManager(this);
        PetManager = new PetManager(this);
        CommandManager = new CommandManager(this);
        MenuManager = new MenuManager(this);
        DatabaseManager = new DatabaseManager(this);


        EventManager.RegisterEvents();
        CommandManager.RegisterCommands();
        DatabaseManager.InitializeConnection();

        Console.WriteLine("CS2 Poor Pets Loaded!");
    }
    public override void Unload(bool hotReload)
    {
        Task.Run(PetManager!.SaveAllPlayersPetsAsync).Wait();
        Console.WriteLine("CS2 Poor Pets Unloaded!");
    }


    public void OnConfigParsed(PluginConfig config)
    {
        Config = config;
    }
    public void DebugLog(string message)
    {
        Logger.LogInformation($"[PoorPets] {message}");
    }
}
