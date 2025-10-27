using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using CS2_Poor_Pets.Core;
using CS2_Poor_Pets.Models;

namespace CS2_Poor_Pets
{
    public class PetManager(CS2_Poor_PetsPlugin plugin)
    {
        private readonly CS2_Poor_PetsPlugin _plugin = plugin;

        public static readonly Dictionary<CCSPlayerController, int> PlayerChosenPet = [];
        public static readonly Dictionary<CCSPlayerController, Dictionary<int, PetModel>> PlayerPetEntities = [];
        public PetModel? CreateSimplePet(CCSPlayerController player, PetConfigModel petConfig)
        {
            var pawn = player.PlayerPawn.Value;
            if (pawn == null) return null;

            var origin = pawn.AbsOrigin;
            var offset = new Vector(-10, -10, 0);
            Vector startPos = origin! + offset!;

            var physbox = Utilities.CreateEntityByName<CFuncMoveLinear>("func_movelinear");

            if (physbox == null) return null;

            physbox.Entity!.Name = $"mover_{player.Slot}";
            physbox.Speed = petConfig.moveSpeed;
            physbox.StartPosition = 1;

            physbox.Entity.Name = $"funcplayer_{player.Slot}";

            physbox.AbsOrigin!.X = startPos.X;
            physbox.AbsOrigin.Y = startPos.Y;

            if (petConfig.isFlying)
            {
                physbox.AbsOrigin!.Z = startPos!.Z + petConfig.offset![2];
            }
            else
            {
                physbox.AbsOrigin!.Z = startPos!.Z;
            }

            physbox!.DispatchSpawn();

            physbox.Teleport(startPos, QAngle.Zero, Vector.Zero);

            physbox.AcceptInput("Open");

            var entity = Utilities.CreateEntityByName<CDynamicProp>("prop_dynamic");
            if (entity == null) return null;

            entity.CBodyComponent!.SceneNode!.Owner!.Entity!.Flags &= ~(uint)(1 << 2);
            entity.SetModel(petConfig.PetModel);
            entity.DispatchSpawn();

            if (!string.IsNullOrEmpty(petConfig.spawnAnimation))
            {
                entity.AcceptInput("SetAnimation", value: petConfig.spawnAnimation);
            }
            else if (!string.IsNullOrEmpty(petConfig.idleAnimation))
            {
                entity.AcceptInput("SetAnimation", value: petConfig.idleAnimation);
            }

            _plugin.AddTimer(1.08f, () =>
            {
                entity.AcceptInput("SetAnimation", value: petConfig.idleAnimation);
            });

            entity.AcceptInput("Start");
            entity.AcceptInput("FollowEntity", physbox, entity, "!activator");


            entity.Teleport(startPos, new QAngle(0, 0, 0), new Vector(0, 0, 0));

            _plugin.DebugLog($"Creating pet for player {player.PlayerName} with model {petConfig.PetModel} at: {startPos}");

            return new PetModel
            {
                entity = entity,
                physbox = physbox,
                FlyingMode = petConfig.isFlying,
                currentPosition = startPos,
                targetPosition = startPos,
                LastPos = startPos,
                isMoving = false,
                justSpawned = true,
                rotationOffset = petConfig.rotationOffset,
                followDistance = petConfig.followDistance,
                stopDistance = petConfig.stopDistance,
                offset = petConfig.offset!,
                idleAnimation = petConfig.idleAnimation,
                runAnimation = petConfig.runAnimation,
                deathAnimation = petConfig.deathAnimation,
                ownerDead = false
            };
        }

        public void ChangePetOrigin(PetModel petModel, CCSPlayerPawn playerPawn)
        {
            if (playerPawn == null || !playerPawn.IsValid || playerPawn.AbsOrigin == null) return;
            if (petModel == null || petModel.physbox == null || !petModel.physbox.IsValid || petModel.physbox.AbsOrigin == null) return;

            petModel.physbox.AbsOrigin!.X = playerPawn.AbsOrigin.X - petModel.offset![0];
            petModel.physbox.AbsOrigin.Y = playerPawn.AbsOrigin.Y - petModel.offset![1];


            petModel.physbox.Position1.X = playerPawn.AbsOrigin.X - petModel.offset![0];
            petModel.physbox.Position1.Y = playerPawn.AbsOrigin.Y - petModel.offset![1];


            petModel.physbox.Position2.X = playerPawn.AbsOrigin.X - petModel.offset![0] - 5;
            petModel.physbox.Position2.Y = playerPawn.AbsOrigin.Y - petModel.offset![1] - 5;

            if (petModel.FlyingMode)
            {
                petModel.physbox.AbsOrigin.Z = playerPawn.AbsOrigin.Z + petModel.offset![2];
                petModel.physbox.Position1.Z = playerPawn.AbsOrigin.Z + petModel.offset![2];
                petModel.physbox.Position2.Z = playerPawn.AbsOrigin.Z + petModel.offset![2];
            }
            else
            {
                petModel.physbox.AbsOrigin.Z = playerPawn.AbsOrigin.Z;
                petModel.physbox.Position1.Z = playerPawn.AbsOrigin.Z;
                petModel.physbox.Position2.Z = playerPawn.AbsOrigin.Z;
            }
        }

        public void CreatePetForPlayer(CCSPlayerController player, int petId)
        {
            PlayerChosenPet[player] = petId;

            var pet = _plugin.PetManager!.CreateSimplePet(player, _plugin.Config.Pets[PlayerChosenPet[player]]);
            if (pet != null)
            {
                if (!PlayerPetEntities.ContainsKey(player))
                {
                    PlayerPetEntities[player] = new Dictionary<int, PetModel>();
                }

                if (PlayerPetEntities[player].ContainsKey(0))
                {
                    PlayerPetEntities[player][0].entity?.Remove();
                }

                PlayerPetEntities[player][0] = pet;
            }
        }

        public void RemovePlayerPet(CCSPlayerController player)
        {
            try
            {
                var steamId = player.SteamID;

                if (PlayerChosenPet.ContainsKey(player))
                {
                    PlayerChosenPet.Remove(player);
                }
                if (PlayerPetEntities.ContainsKey(player))
                {
                    PlayerPetEntities[player][0].physbox!.Remove();
                    PlayerPetEntities[player][0].entity!.Remove();
                    Server.NextFrame(() =>
                    {
                        PlayerPetEntities[player].Clear();
                    });
                }
                Task.Run(async () =>
                {
                    try
                    {
                        await _plugin.DatabaseManager!.RemovePlayerPetAsync(steamId);
                    }
                    catch (Exception error)
                    {
                        _plugin.DebugLog($"Error removing pet from DB: {error}");
                    }
                });

                player.PrintToChat(_plugin.Localizer["RemovedPetPlayerNotification"]);

                return;
            }
            catch (Exception error)
            {
                _plugin.DebugLog($"Error with deleting pet: {error}");
                player.PrintToChat(_plugin.Localizer["RemovedPetPlayerError"]);
                return;
            }
        }

        public void SaveAllPlayersPetsAsync()
        {
            try
            {
                foreach (var p in PlayerChosenPet)
                {
                    var steamid = p.Key.SteamID;
                    Task.Run(async () =>
                    {
                        await _plugin.DatabaseManager!.SavePlayerPetAsync(steamid, p.Value);
                    });
                }
            }
            catch (Exception error)
            {
                _plugin.DebugLog($"Error saving all players: {error}");
            }
        }

        public void RemovePetEntityOnPlayerDeath(CCSPlayerController player)
        {
            if (player == null) return;
            if (PlayerPetEntities.ContainsKey(player))
            {
                Server.NextFrame(() =>
                {
                    var pet = PlayerPetEntities[player][0];
                    pet.ownerDead = true;
                    pet.entity!.AcceptInput("SetAnimation", value: pet.deathAnimation!);
                    _plugin.AddTimer(_plugin.Config.timeAfterDeathToDeletePet, () =>
                    {
                        pet.physbox!.Remove();
                        pet.entity!.Remove();
                    });
                });
            }
        }
        public void ClearPetsCache()
        {
            if (PlayerChosenPet != null)
            {
                PlayerChosenPet.Clear();
            }
            if (PlayerPetEntities != null)
            {
                PlayerPetEntities.Clear();
            }
        }

    }
}