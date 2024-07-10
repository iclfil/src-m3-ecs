using MatchThree.Rack;
using Client.Game.Utils;

namespace MatchThree.Rack.ECS.Systems.Falling.Components
{
    /// <summary>
    /// ��������� �������� ������� Item, � ������ �� � ����.
    /// </summary>
    public struct Puller
    {
        public IItem DroppedItem;
        public Int2 PositionPuller;
        public PullerDirection Direction;
    }
}

