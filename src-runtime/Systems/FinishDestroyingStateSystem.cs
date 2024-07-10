using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using MatchThree.ECS.Modules;
using MatchThree.Rack.Common;
using MatchThree.Rack.ECS.Events;
using SevenBoldPencil.EasyEvents;

namespace MatchThree.Rack.ECS.Systems
{
    public class FinishDestroyingStateSystem : IEcsRunSystem
    {
        private readonly EventsBus _bus;

        public FinishDestroyingStateSystem(EventsBus bus)
        {
            _bus = bus;
        }

        public void Run(IEcsSystems systems)
        {
            ref StateFinishedEvent data = ref _bus.NewEventSingleton<StateFinishedEvent>();
            data.Name = NameModule.Destroying;
            data.Result = ModuleResult.Completed;
        }
    }
}