using MatchThree.Rack;
using MatchThree.Rack.ECS.Behaviors;
using MatchThree.Rack.ECS;
using Leopotam.EcsLite;

namespace MatchThree.Configs
{
    public class StripedBombFactory : EntityItemsFactory
    {
        public override int CreateEntityByItem(IItem item, EcsWorld world)
        {
            var entity =  base.CreateEntityByItem(item, world);

            ref var data = ref world.GetPool<StripedBomb>().Add(entity);
            data.Bonus = item;

            world.GetPool<MatchBehavior>().Add(entity);
            world.GetPool<FallBehavior>().Add(entity);
            world.GetPool<SwapBehavior>().Add(entity);
            return entity;
        }
    }
}
