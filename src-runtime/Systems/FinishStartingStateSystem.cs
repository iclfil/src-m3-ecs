using Leopotam.EcsLite;
using MatchThree.ECS.Modules;
using MatchThree.Rack.Common;
using MatchThree.Rack.ECS.Events;
using SevenBoldPencil.EasyEvents;

namespace MatchThree.Rack.ECS.Systems
{
    public class FinishStartingStateSystem : IEcsRunSystem
    {
        private readonly EventsBus _eventBus;

        public FinishStartingStateSystem(EventsBus eventsEvent)
        {
            _eventBus = eventsEvent;
        }

        public void Run(IEcsSystems systems)
        {
            ref StateFinishedEvent eventData = ref _eventBus.NewEventSingleton<StateFinishedEvent>();
            eventData.Name = NameModule.Starting;
            eventData.Result = ModuleResult.Completed;
        }
    }
}