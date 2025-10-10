using System.Text.Json.Serialization;

namespace CS2_Poor_Pets.Models
{
    public class DBConfigModel
    {
        [JsonPropertyName("DB_HOST")]
        public string DBHost { get; set; } = "localhost";
        [JsonPropertyName("DB_Port")]
        public uint DBPort { get; set; } = 3306;
        [JsonPropertyName("DB_User")]
        public string DBUser { get; set; } = "root";
        [JsonPropertyName("DB_Name")]
        public string DBName { get; set; } = "db_";
        [JsonPropertyName("DB_Password")]
        public string DBPass { get; set; } = "123";
    }
    public class PetConfigModel
    {
        [JsonPropertyName("petName")]
        public string petName { get; set; } = "";
        [JsonPropertyName("isVipOnly")]
        public bool isVipOnly { get; set; } = false;
        [JsonPropertyName("petModel")]
        public string PetModel { get; set; } = "";
        [JsonPropertyName("idleAnimation")]
        public string idleAnimation { get; set; } = "@courier_idle";
        [JsonPropertyName("runAnimation")]
        public string runAnimation { get; set; } = "@courier_run";
        [JsonPropertyName("spawnAnimation")]
        public string spawnAnimation { get; set; } = "@courier_spawn";
        [JsonPropertyName("isFlying")]
        public bool isFlying { get; set; } = true;
        [JsonPropertyName("moveSpeed")]
        public float moveSpeed { get; set; }
        [JsonPropertyName("rotationOffset")]
        public float rotationOffset { get; set; }
        [JsonPropertyName("followDistance")]
        public float followDistance { get; set; }
        [JsonPropertyName("stopDistance")]
        public float stopDistance { get; set; }
        [JsonPropertyName("offset")]
        public float[]? offset { get; set; }

    }
}