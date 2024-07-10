using MatchThree.Rack;
using MatchThree.Rack.ECS;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using MatchThree.Rack.CoreECS.Tags;
#if UNITY_EDITOR

#endif

namespace MatchThree.Rack.ECS.Systems
{
    public class CheckFinishMatchesSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<ECS.Items, CheckBonusesTag>> __hasItems = default;

        public void Run(IEcsSystems systems)
        {
            if (__hasItems.Value.GetEntitiesCount() == 0)
            {
                int e = systems.GetWorld().NewEntity();
                var pool = systems.GetWorld().GetPool<FinishMatches>();
                pool.Add(e);
            }
        }
    }
}