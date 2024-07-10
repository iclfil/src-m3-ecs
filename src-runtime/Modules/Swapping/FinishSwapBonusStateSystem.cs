using Leopotam.EcsLite;
using MatchThree.Rack.Common;
using SevenBoldPencil.EasyEvents;

namespace MatchThree.Rack.ECS.Systems
{
    public class FinishSwapBonusStateSystem : IEcsRunSystem
    {
        private readonly EventsBus _bus;

        public FinishSwapBonusStateSystem(EventsBus bus)
        {
            _bus = bus;
        }

        public void Run(IEcsSystems systems)
        {

            //if (_bus.HasEventSingleton<GoToFallingEvent>())
            //{
            //    ECSStaticUtils.StateFinish(RackState.SwapBonus, ModuleResult.GoToFalling);
            //    _bus.DestroyEventSingleton<GoToFallingEvent>();
            //    return;
            //}

            //if (_bus.HasEventSingleton<GoToMatchesEvent>())
            //{
            //    ECSStaticUtils.StateFinish(RackState.SwapBonus, ModuleResult.GoToMatches);
            //    _bus.DestroyEventSingleton<GoToMatchesEvent>();
            //    return;
            //}

            //if (_bus.HasEventSingleton<GoToDestroyingEvent>())
            //{
            //    ECSStaticUtils.StateFinish(RackState.SwapBonus, ModuleResult.GoToDestroying);
            //    _bus.DestroyEventSingleton<GoToDestroyingEvent>();
            //    return;
            //}

            //ECSStaticUtils.StateFinish(RackState.SwapBonus);
        }
    }

    public struct GoToDestroyingEvent : IEventSingleton
    {
    }

    public struct GoToMatchesEvent : IEventSingleton
    {
    }

    public struct GoToFallingEvent : IEventSingleton
    {

    }
}