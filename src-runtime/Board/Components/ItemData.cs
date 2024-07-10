using MatchThree.Rack;
using Leopotam.EcsLite;

namespace Client.MatchThree.Board.ECS.Components
{
    public struct ItemData
    {
        public IItem Item;
        public EcsPackedEntity Entity;
    }
}