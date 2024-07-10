using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using MatchThree.ECS.Modules;
using MatchThree.Rack.Common;
using MatchThree.Rack.ECS.Events;
using MatchThree.Rack.ECS.SharedDatas;
using SevenBoldPencil.EasyEvents;

namespace MatchThree.Rack.ECS.Systems
{
    public class FinishFallingStateSystem : IEcsRunSystem
    {
        private readonly OnCollapseItems _collapseItems;
        private readonly EventsBus _bus;
        readonly EcsFilterInject<Inc<FinishFalling>> __state = default;

        public FinishFallingStateSystem(OnCollapseItems collapseItems, EventsBus bus)
        {
            _bus = bus;
            _collapseItems = collapseItems;
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var _Enty in __state.Value)
            {
                var s = systems.GetShared<SharedData>();

                _collapseItems?.Invoke(s.PullerItems);
                s.PullerItems.Clear();
                ref StateFinishedEvent data = ref _bus.NewEventSingleton<StateFinishedEvent>();
                data.Name = NameModule.Collapse;
                data.Result = ModuleResult.Completed;
                systems.GetWorld().DelEntity(_Enty);
            }
        }
    }
}