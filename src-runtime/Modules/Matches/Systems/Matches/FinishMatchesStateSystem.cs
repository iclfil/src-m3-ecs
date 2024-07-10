using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using MatchThree.ECS.Modules;
using MatchThree.Rack.Common;
using MatchThree.Rack.ECS.Events;
using SevenBoldPencil.EasyEvents;

namespace MatchThree.Rack.ECS.Systems
{
    public class FinishMatchesStateSystem : IEcsRunSystem
    {
        private readonly EventsBus _bus;
        private readonly EcsFilterInject<Inc<FinishMatches>> __state = default;

        public FinishMatchesStateSystem(EventsBus bus)
        {
            _bus = bus;
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var _Enty in __state.Value)
            {
                ref StateFinishedEvent eventData = ref _bus.NewEventSingleton<StateFinishedEvent>();
                eventData.Name = NameModule.Matches;
                eventData.Result = ModuleResult.Completed;
                systems.GetWorld().DelEntity(_Enty);
            }
        }
    }
}