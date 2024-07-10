using MatchThree.Rack;
using MatchThree.Rack.ECS.Behaviors;
using MatchThree.Rack.ECS.Tags;
using Leopotam.EcsLite;

namespace MatchThree.Configs
{
    public class ChipFactory : EntityItemsFactory
    {
        public override int CreateEntityByItem(IItem item, EcsWorld world)
        {
            var entity = base.CreateEntityByItem(item, world);

            world.GetPool<ChipTag>().Add(entity);

            world.GetPool<MatchBehavior>().Add(entity);
            world.GetPool<FallBehavior>().Add(entity);
            world.GetPool<SwapBehavior>().Add(entity);

            return entity;
        }
    }
}
