using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using MatchThree.Rack.Common;
using SevenBoldPencil.EasyEvents;

namespace MatchThree.Rack.ECS.Systems
{
    public class FinishWaitingStateSystem : IEcsRunSystem
    {
        private readonly EventsBus _eventsBus;

        private readonly EcsFilterInject<Inc<Swap>> _swap = default;


        readonly EcsFilterInject<Inc<WaitingFinish>> __waitingState = default;

        public FinishWaitingStateSystem(EventsBus eventsBus)
        {
            _eventsBus = eventsBus;
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _swap.Value)
            {
                var data = _swap.Pools.Inc1.Get(entity);
                var result = GetFinish(data.SwapType);
                //ECSStaticUtils.StateFinish(RackState.Waiting, result);
            }
        }

        private ModuleResult GetFinish(SwapType swapType)
        {
            if (swapType == SwapType.Simple)
                return ModuleResult.HasSwap;

            if (swapType == SwapType.NoSwap)
                return ModuleResult.NoSwap;

            return ModuleResult.HasBonusSwap;
        }
    }
}