using CounterStrikeSharp.API.Core;
using CS2_Poor_Pets.Models;
using System.Text.Json.Serialization;

namespace CS2_Poor_Pets.Core
{
    public class PluginConfig : BasePluginConfig
    {
        [JsonPropertyName("DatabaseConfig")]
        public DBConfigModel DatabaseConfig { get; set; } = new();
        [JsonPropertyName("VipFlag")]
        public string vipFlag { get; set; } = "@pets/vip";

        [JsonPropertyName("UpdatePerTicks")]
        public float perTicks { get; set; } = 35;

        [JsonPropertyName("Pets")]
        public List<PetConfigModel> Pets { get; set; } = new()
        {
            new PetConfigModel
            {
                petName = "Beaver",
                isVipOnly = false,
                PetModel = "models/pets/cskull/dota2/beaverknight/beaverknight.vmdl",
                idleAnimation = "@courier_idle",
                runAnimation = "@courier_run",
                spawnAnimation = "@courier_spawn",
                isFlying = false,
                moveSpeed = 200,
                rotationOffset = -90,
                followDistance = 100,
                stopDistance = 70,
                offset = [-30, -20, 0]
            },
            new PetConfigModel
            {
                petName = "Snowl",
                isVipOnly = false,
                PetModel = "models/pets/cskull/dota2/snowl/snowl_flying.vmdl",
                idleAnimation = "@courier_idle",
                runAnimation = "@courier_run",
                spawnAnimation = "@courier_spawn",
                isFlying = true,
                moveSpeed = 200,
                rotationOffset = -90,
                followDistance = 100,
                stopDistance = 70,
                offset = [-30, -20, 50]
            },
            new PetConfigModel
            {
                petName = "Llama",
                isVipOnly = true,
                PetModel = "models/pets/cskull/dota2/livery_llama_courier/livery_llama_courier.vmdl",
                idleAnimation = "@courier_idle",
                runAnimation = "@courier_run",
                spawnAnimation = "@courier_spawn",
                isFlying = false,
                moveSpeed = 200,
                rotationOffset = -90,
                followDistance = 100,
                stopDistance = 70,
                offset = [-30, -20, 0]
            }
        };
    }
}