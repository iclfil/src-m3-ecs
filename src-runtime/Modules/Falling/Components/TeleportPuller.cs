using MatchThree.Rack;
using Client.Game.Utils;

namespace MatchThree.Rack.ECS.Systems.Falling.Components
{
    public struct TeleportPuller
    {
        public IItem DroppedItem;
        public Int2 SourceTeleportPosition;
        public Int2 PositionPuller;
    }
}