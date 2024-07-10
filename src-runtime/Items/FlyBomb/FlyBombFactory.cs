using MatchThree.Rack;
using MatchThree.Rack.ECS.Behaviors;
using MatchThree.Rack.ECS;
using Leopotam.EcsLite;

namespace MatchThree.Configs
{
    public class FlyBombFactory : EntityItemsFactory
    {
        public override int CreateEntityByItem(IItem item, EcsWorld world)
        {
            var entity = base.CreateEntityByItem(item, world);

            ref var p = ref world.GetPool<FlyBomb>().Add(entity);
            p.Bonus = item;

            world.GetPool<FallBehavior>().Add(entity);
            world.GetPool<MatchBehavior>().Add(entity);
            world.GetPool<SwapBehavior>().Add(entity);
            return entity;
        }
    }
}