using Leopotam.EcsLite;
using MatchThree.Rack.ECS.Events;
using SevenBoldPencil.EasyEvents;

namespace MatchThree.Rack.ECS.Systems.Input
{
    /// <summary>
    /// Взрывает бонусы по двойному клики
    /// </summary>
    public class DoubleClickHandlerSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly EventsBus _bus;
        private readonly IBoard _board;
        private  EcsWorld _world;
        public DoubleClickHandlerSystem(EventsBus bus, IBoard board)
        {
            _bus = bus;
            _board = board;
        }

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
        }

        public void Run(IEcsSystems systems)
        {
            if (_bus.HasEventSingleton<DoubleClickEvent>())
            {
                var clickedPos = _bus.GetEventBodySingleton<DoubleClickEvent>().ClickedItem;
                if (_board.TryGetLastItem(clickedPos, out var item))
                {
                    if (item.IsBonus)
                        ECSStaticUtils.MatchBonus(item, _world);
                }

                _bus.DestroyEventSingleton<DoubleClickEvent>();
            }
        }
    }
}