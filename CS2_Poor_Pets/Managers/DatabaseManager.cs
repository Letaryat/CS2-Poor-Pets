using Dapper;
using CS2_Poor_Pets.Core;
using MySqlConnector;

namespace CS2_Poor_Pets
{
    public class DatabaseManager(CS2_Poor_PetsPlugin plugin)
    {
        private readonly CS2_Poor_PetsPlugin _plugin = plugin;
        private string _connectionString = string.Empty;

        public void InitializeConnection()
        {
            var config = _plugin.Config.DatabaseConfig;
            if (string.IsNullOrEmpty(config!.DBHost) || string.IsNullOrEmpty(config!.DBName) || string.IsNullOrEmpty(config!.DBPass) || config!.DBPort == 0)
            {
                _plugin.DebugLog("MySQL database configuration is empty or incomplete");
                return;
            }

            MySqlConnectionStringBuilder builder = new()
            {
                Server = config.DBHost,
                UserID = config.DBUser,
                Port = config.DBPort,
                Password = config.DBPass,
                Database = config.DBName,
            };

            _connectionString = builder.ConnectionString;

            try
            {
                using var connection = new MySqlConnection(_connectionString);
                connection.Open();
                _plugin.DebugLog("Connected to the database!");

                string createTablesQuery = @"
                CREATE TABLE IF NOT EXISTS PoorPets_Players(
                SteamID VARCHAR(255),
                PetID INT
                );
                ";

                connection.Execute(createTablesQuery);
            }
            catch (Exception error)
            {
                _plugin.DebugLog($"Problem with connecting to the DB {error}");
            }
        }

        public async Task<int?> GetPlayerPet(ulong steamId)
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                _plugin.DebugLog("There was a problem with connectionString. Probably not connected to the database?");
                return null;
            }
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync();
                var petId = await connection.QueryFirstOrDefaultAsync<int?>(
                  "SELECT PetID FROM `PoorPets_Players` WHERE SteamID = @steamId", new { steamId });

                if (petId == null)
                {
                    return null;
                }

                return petId;
            }
            catch (Exception error)
            {
                _plugin.DebugLog($"Problem with fetching player data {error}");
                return null;
            }
        }

        private async Task<bool> CheckIfPetExistAsync(MySqlConnection connection, ulong steamId)
        {
            try
            {
                string sql = "SELECT COUNT(1) FROM `PoorPets_Players` WHERE SteamID = @steamId;";
                var exists = await connection.ExecuteScalarAsync<bool>(sql, new { steamId });
                return exists;
            }
            catch (Exception error)
            {
                _plugin.DebugLog($"Error checking if player pet Exist {error}");
                return false;
            }
        }

        public async Task SavePlayerPetAsync(ulong steamId, int petId)
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                _plugin.DebugLog("There was a problem with connectionString. Probably not connected to the database?");
                return;
            }
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync();

                var petExist = await CheckIfPetExistAsync(connection, steamId);

                if (petExist)
                {
                    await connection.ExecuteAsync(
                        "UPDATE `PoorPets_Players` SET PetID = @petId WHERE SteamID = @steamId", new { petId, steamId });
                }
                else
                {
                    await connection.ExecuteAsync(
                        "INSERT INTO `PoorPets_Players` (SteamID, PetID) VALUES (@steamId, @petId)", new { steamId, petId });
                }
            }
            catch (Exception error)
            {
                _plugin.DebugLog($"Problem with saving player data {error}");
                return;
            }
        }

        public async Task RemovePlayerPetAsync(ulong steamId)
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                _plugin.DebugLog("There was a problem with connectionString. Probably not connected to the database?");
                return;
            }
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync();

                var petExist = await CheckIfPetExistAsync(connection, steamId);

                if (petExist)
                {
                    await connection.ExecuteAsync(
                        "DELETE FROM `PoorPets_Players` WHERE SteamID = @steamId", new { steamId });
                }
            }
            catch (Exception error)
            {
                _plugin.DebugLog($"Problem with removing pet {error}");
                return;
            }
        }

    }
}