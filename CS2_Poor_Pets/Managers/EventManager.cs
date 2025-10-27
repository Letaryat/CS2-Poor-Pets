using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using CS2_Poor_Pets.Core;
using CS2_Poor_Pets.Models;
using CS2_Poor_Pets.Utils;


namespace CS2_Poor_Pets
{
    public class EventManager(CS2_Poor_PetsPlugin plugin)
    {
        private readonly CS2_Poor_PetsPlugin _plugin = plugin;
        public void RegisterEvents()
        {
            _plugin.RegisterEventHandler<EventPlayerConnectFull>(OnPlayerConnectFull);
            _plugin.RegisterEventHandler<EventPlayerSpawn>(OnPlayerSpawn);
            _plugin.RegisterEventHandler<EventPlayerDeath>(OnPlayerDeath);
            _plugin.RegisterEventHandler<EventPlayerDisconnect>(OnPlayerDisconnect);

            //Listeners:
            _plugin.RegisterListener<Listeners.OnServerPrecacheResources>(OnServerPrecacheResources);
            _plugin.RegisterListener<Listeners.OnTick>(OnTick);

            //HooksEntity:
            _plugin.HookEntityOutput("func_movelinear", "OnFullyOpen", OnFullyOpen);
            _plugin.HookEntityOutput("func_movelinear", "OnFullyClosed", OnFullyClosed);

        }

        private HookResult OnPlayerConnectFull(EventPlayerConnectFull @event, GameEventInfo info)
        {
            var player = @event.Userid;
            if (player == null) return HookResult.Continue;

            var steamId = player.SteamID;

            Task.Run(async () =>
            {
                try
                {
                    var pet = await _plugin.DatabaseManager!.GetPlayerPet(steamId);
                    if (pet == null) return;
                    PetManager.PlayerChosenPet[player] = (int)pet;
                }
                catch (Exception error)
                {
                    _plugin.DebugLog($"Error ConnectFull: {error}");
                }
            });


            return HookResult.Continue;
        }

        private HookResult OnPlayerSpawn(EventPlayerSpawn @event, GameEventInfo info)
        {
            var player = @event.Userid;
            if (player == null) return HookResult.Continue;
            var pawn = player.PlayerPawn.Value;
            if (pawn == null || !pawn.IsValid) return HookResult.Continue;

            var pet = _plugin.PetManager!.CreateSimplePet(player, _plugin.Config.Pets[PetManager.PlayerChosenPet[player]]);
            if (pet != null)
            {
                if (!PetManager.PlayerPetEntities.ContainsKey(player))
                    PetManager.PlayerPetEntities[player] = new Dictionary<int, PetModel>();

                PetManager.PlayerPetEntities[player][0] = pet;
            }

            return HookResult.Continue;
        }

        private HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
        {
            var player = @event.Userid;
            if (player == null) return HookResult.Continue;

            _plugin.PetManager!.RemovePetEntityOnPlayerDeath(player);

            return HookResult.Continue;
        }

        private HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
        {
            var player = @event.Userid;
            if (player == null) return HookResult.Continue;

            var steamId = player.SteamID;

            if (PetManager.PlayerChosenPet.ContainsKey(player))
            {
                var pet = PetManager.PlayerChosenPet[player];
                Task.Run(async () =>
                {
                    try
                    {
                        await _plugin.DatabaseManager!.SavePlayerPetAsync(steamId, pet);
                    }
                    catch (Exception error)
                    {
                        _plugin.DebugLog($"Error PlayerDisconnect: {error}");
                    }
                    finally
                    {
                        if (PetManager.PlayerPetEntities.ContainsKey(player))
                        {
                            foreach (var pet in PetManager.PlayerPetEntities[player].Values)
                            {
                                pet.physbox?.Remove();
                                pet.entity?.Remove();
                            }
                            PetManager.PlayerPetEntities.Remove(player);
                        }
                    }
                });

            }
            return HookResult.Continue;
        }

        /* Listeners */
        private void OnServerPrecacheResources(ResourceManifest manifest)
        {
            foreach (var pets in _plugin.Config.Pets)
            {
                manifest.AddResource(pets.PetModel);
            }
        }

        private void OnTick()
        {

            foreach (var kv in PetManager.PlayerPetEntities)
            {
                if (Server.TickCount % _plugin.Config.perTicks != 0) continue;
                var player = kv.Key;
                if (player == null || !player.IsValid) continue;

                var playerPawn = player.PlayerPawn.Value;
                if (playerPawn == null) continue;

                var petModel = kv.Value.GetValueOrDefault(0);
                if (petModel == null || petModel.physbox == null || !petModel.physbox.IsValid)
                    continue;

                Vector diff = playerPawn.AbsOrigin! - petModel.physbox.AbsOrigin!;

                float distanceToPlayer = PluginUtilities.CalculateDistanceToPlayer(playerPawn.AbsOrigin!, petModel.physbox.AbsOrigin!);

                if (distanceToPlayer >= petModel.followDistance)
                {

                    if (_plugin.PetManager == null) continue;

                    _plugin.PetManager.ChangePetOrigin(petModel, playerPawn);

                    Server.NextFrame(() =>
                    {
                        float yaw = PluginUtilities.CalculateYaw(diff, petModel.rotationOffset);

                        petModel.entity!.Teleport(null, new QAngle(0, yaw, 0));
                        petModel.physbox.Teleport(petModel.physbox.AbsOrigin);

                        if (!petModel.isMoving)
                        {
                            if(playerPawn.LifeState == (byte)LifeState_t.LIFE_DEAD)
                            {
                                petModel.entity!.AcceptInput("SetAnimation", value: petModel.deathAnimation!);
                                return;
                            }   
                            petModel!.entity.AcceptInput("SetAnimation", value: petModel.runAnimation!);
                            petModel.isMoving = true;
                        }
                        petModel.physbox.AcceptInput("Open");
                    });
                }

            }
        }

        /* HOOKS */
        private HookResult OnFullyClosed(CEntityIOOutput output, string name, CEntityInstance activator, CEntityInstance caller, CVariant value, float delay)
        {
            var funcMover = caller.As<CFuncMoveLinear>();

            var playerSlot = funcMover.Entity!.Name.Split("_")[1];
            int slotNumber = Int32.Parse(playerSlot);
            var playerController = Utilities.GetPlayerFromSlot(slotNumber);
            var playerPawn = playerController!.PlayerPawn.Value;

            if (playerPawn == null || !playerPawn.IsValid) return HookResult.Continue;

            if (playerController != null && PetManager.PlayerPetEntities.TryGetValue(playerController, out var pet))
            {
                pet[0].isMoving = false;
                if (playerPawn.LifeState == (byte)LifeState_t.LIFE_DEAD)
                {
                    _plugin.PetManager!.RemovePetEntityOnPlayerDeath(playerController);
                }
                else
                {
                    pet[0].entity!.AcceptInput("SetAnimation", value: pet[0].idleAnimation!);
                }
            }

            return HookResult.Continue;
        }

        private HookResult OnFullyOpen(CEntityIOOutput output, string name, CEntityInstance activator, CEntityInstance caller, CVariant value, float delay)
        {
            var funcMover = caller.As<CFuncMoveLinear>();

            funcMover.Teleport(funcMover.AbsOrigin);

            var playerSlot = funcMover.Entity!.Name.Split("_")[1];
            int slotNumber = Int32.Parse(playerSlot);
            var playerController = Utilities.GetPlayerFromSlot(slotNumber);

            var playerPawn = playerController!.PlayerPawn.Value;

            if (playerController != null && PetManager.PlayerPetEntities.TryGetValue(playerController, out var pet))
            {
                float distanceToPlayer = PluginUtilities.CalculateDistanceToPlayer(playerPawn!.AbsOrigin!, pet[0].physbox!.AbsOrigin!);

                if (distanceToPlayer <= pet[0].stopDistance)
                {
                    funcMover.AcceptInput("Close");
                }
            }

            return HookResult.Continue;
        }

    }
}