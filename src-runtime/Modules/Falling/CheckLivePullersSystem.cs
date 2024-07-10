using MatchThree.Rack;
using FilConsole;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using MatchThree.Rack.ECS.Systems.Falling.Components;

namespace MatchThree.Rack.ECS.Systems.Falling
{
    public class CheckLivePullersSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsFilterInject<Inc<Puller>> _pullers = default;
        private EcsWorld _world;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
        }

        public void Run(IEcsSystems systems)
        {
            if (_pullers.Value.GetEntitiesCount() == 0)
            {
                MyLog.Log("System/Green","No Lived Pullers. Finish Falling", GetType());
                int tr = _world.NewEntity();
                ref FinishFalling f = ref _world.GetPool<FinishFalling>().Add(tr);
            }
        }
    }
}