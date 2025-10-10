using System.Text.Json.Serialization;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace CS2_Poor_Pets.Models
{
    public class PetModel
    {
        public CDynamicProp? entity { get; set; }
        public CFuncMoveLinear? physbox { get; set; }
        public bool FlyingMode { get; set; }
        public Vector? targetPosition { get; set; }
        public Vector? currentPosition { get; set; }
        public Vector? LastPos { get; set; }
        public bool isMoving { get; set; }
        public bool justSpawned { get; set; }
        public float rotationOffset { get; set; }
        public float followDistance { get; set; }
        public float stopDistance { get; set; }
        public required float[] offset { get; set; }
        public string? idleAnimation { get; set; }
        public string? runAnimation { get; set; }
    }
}