using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using SevenBoldPencil.EasyEvents;
using UnityEngine;

namespace MatchThree.Rack.ECS.Systems
{
    public class UnSwappingSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly OnSwapItems _swapItems;
        private readonly EcsFilterInject<Inc<Swap>> f_swap = default;


        private EventsBus _bus;

        private EcsWorld _world;
        private IBoardFacade _board;


        public UnSwappingSystem(IBoardFacade board, EventsBus bus, OnSwapItems swapItems)
        {
            _bus = bus;
            _swapItems = swapItems;
            _board = board;
        }

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
        }

        public void Run(IEcsSystems systems)
        {
            if (!_bus.HasEventSingleton<UnSwapEvent>()) return;

            foreach (var entity in f_swap.Value)
            {
                var type = f_swap.Pools.Inc1.Get(entity).SwapType;

                if (type == SwapType.Simple)
                {
                    var firstItem = f_swap.Pools.Inc1.Get(entity).FirstItem;
                    var secondItem = f_swap.Pools.Inc1.Get(entity).SecondItem;
                    _board.Swap(secondItem, firstItem); //возвращаем на позиции
                }

                _world.DelEntity(entity);
            }

            _bus.DestroyEventSingleton<UnSwapEvent>();
        }
    }

    public struct UnSwapEvent : IEventSingleton
    {

    }
}