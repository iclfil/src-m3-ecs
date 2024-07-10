using MatchThree.Rack.ECS;
using MatchThree.Rack;
using MatchThree.Configs;
using Leopotam.EcsLite;
using MatchThree.Rack.ECS.Behaviors;

namespace MatchThree.Rack.Items
{
    public class IngredientFactory : EntityItemsFactory
    {
        public override int CreateEntityByItem(IItem item, EcsWorld world)
        {
            var entity = base.CreateEntityByItem(item, world);

            ref var p = ref world.GetPool<Ingredient>().Add(entity);
            p.Item = item;
            world.GetPool<FallBehavior>().Add(entity);
            world.GetPool<SwapBehavior>().Add(entity);
            return entity;
        }
    }
}