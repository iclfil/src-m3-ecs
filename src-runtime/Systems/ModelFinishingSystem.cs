using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using MatchThree.ECS.Modules;
using MatchThree.Rack.Common;
using MatchThree.Rack.ECS.Events;
using SevenBoldPencil.EasyEvents;
using UnityEngine;

namespace MatchThree.Rack.ECS.Systems
{
    public class ModelFinishingSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsFilterInject<Inc<Swap>> f_swap = default;

        public EventsBus _bus { get; }
        private EcsWorld _world;

        public ModelFinishingSystem(EventsBus bus)
        {
            this._bus = bus;
        }

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in f_swap.Value)
                _world.DelEntity(entity);

            Debug.Log("Finish");
            ref StateFinishedEvent r = ref _bus.NewEventSingleton<StateFinishedEvent>();
            r.Name = NameModule.Finishing;
            r.Result = ModuleResult.Completed;
        }


    }
}