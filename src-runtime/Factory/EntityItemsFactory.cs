using MatchThree.Rack;
using MatchThree.Rack.ECS;
using Leopotam.EcsLite;

namespace MatchThree.Configs
{
    public abstract class EntityItemsFactory
    {
        public virtual int CreateEntityByItem(IItem item, EcsWorld world)
        {
            var entity = world.NewEntity();
            ref var itemData = ref world.GetPool<ItemData>().Add(entity);

            itemData.Item = item;
            itemData.Entity = world.PackEntity(entity);
            return entity;
        }
    }
}
