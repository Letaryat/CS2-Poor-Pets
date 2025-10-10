using CounterStrikeSharp.API.Modules.Utils;

namespace CS2_Poor_Pets.Utils
{
    public static class PluginUtilities
    {
        public static Vector NormalizeVector(Vector vector)
        {
            float length = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);
            if (length == 0) return new Vector(0, 0, 0);

            return new Vector(
                vector.X / length,
                vector.Y / length,
                vector.Z / length
            );
        }

        public static float CalculateDistanceToPlayer(Vector playerPos, Vector entityPos)
        {
            Vector flatPlayerPos = new Vector(playerPos.X, playerPos.Y, 0);
            Vector flatPetPos = new Vector(entityPos.X, entityPos.Y, 0);
            float distance = (flatPlayerPos - flatPetPos).Length();
            return distance;
        }

        public static float CalculateYaw(Vector diff, float rotationOffset)
        {
            Vector lookDirection = NormalizeVector(diff);
            float yaw = (float)(Math.Atan2(lookDirection.Y, lookDirection.X) * 180.0 / Math.PI) + rotationOffset;
            return yaw;
        }

    }
}