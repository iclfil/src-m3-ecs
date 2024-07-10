using Leopotam.EcsLite;
using MatchThree.ECS.Modules;
using MatchThree.Rack.Common;
using MatchThree.Rack.ECS.Events;
using SevenBoldPencil.EasyEvents;
using UnityEngine;

namespace MatchThree.Rack.ECS.Systems
{
    public class FinishNoMatchesSystem : IEcsRunSystem
    {
        private readonly EventsBus _bus;

        public FinishNoMatchesSystem(EventsBus bus)
        {
            _bus = bus;
        }

        public void Run(IEcsSystems systems)
        {
            ref StateFinishedEvent eventData = ref _bus.NewEventSingleton<StateFinishedEvent>();
            eventData.Name = NameModule.NoMatches;
            eventData.Result = ModuleResult.Completed;
        }
    }
}