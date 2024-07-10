using MatchThree.Rack;
using Leopotam.EcsLite;

namespace MatchThree.Configs
{
    public class DefaultSquareFactory : EntityItemsFactory
    {
        public override int CreateEntityByItem(IItem item, EcsWorld world)
        {
            var entity = base.CreateEntityByItem(item, world);
            world.GetPool<DefaultSquare>().Add(entity);
            return entity;
        }
    }
}
