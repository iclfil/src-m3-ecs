using System.Collections.Generic;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using MatchThree.Rack.ECS.Behaviors;
using MatchThree.Rack.ECS.Tags;
using SevenBoldPencil.EasyEvents;

namespace MatchThree.Rack.ECS.Systems
{
    public class MatchColorBombWithColorBomb : IEcsInitSystem, IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<ItemData, MatchBehavior, ChipTag>> f_chips = "Board";

        private readonly EventsBus _bus;
        private readonly IBoardFacade _board;
        private readonly OnMatchColorBombWithColorBomb _finishCallback;
        private EcsWorld _world;

        public MatchColorBombWithColorBomb(EventsBus bus, IBoardFacade board, OnMatchColorBombWithColorBomb finishCallback)
        {
            _bus = bus;
            _board = board;
            _finishCallback = finishCallback;
        }

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
        }

        public void Run(IEcsSystems systems)
        {
            if (_bus.HasEventSingleton<MatchColorBombWithColorBombEvent>() == false)
                return;

            var data = _bus.GetEventBodySingleton<MatchColorBombWithColorBombEvent>();

            _board.DeleteIncludeItem(data.ColorBomb_1);
            _board.DeleteIncludeItem(data.ColorBomb_2);

            var matchedItems = new List<int>();

            foreach (var entity in f_chips.Value)
            {
                var item = f_chips.Pools.Inc1.Get(entity).Item;
                _board.DeleteIncludeItem(item);
                matchedItems.Add(item.GetID());
            }

            _finishCallback?.Invoke(data.ColorBomb_1.GetID(), data.ColorBomb_2.GetID(), matchedItems);
            _bus.DestroyEventSingleton<MatchColorBombWithColorBombEvent>();
        }

        private HashSet<IItem> GetColorsItems(int color)
        {
            var items = new HashSet<IItem>();
            foreach (var e_chip in f_chips.Value)
            {
                if (color == -1)
                {
                    var item = f_chips.Pools.Inc1.Get(e_chip).Item;
                    items.Add(item);
                    continue;
                }

                if (f_chips.Pools.Inc1.Get(e_chip).Item.Color == color)
                {
                    var item = f_chips.Pools.Inc1.Get(e_chip).Item;
                    items.Add(item);
                }
            }

            return items;
        }
    }

    public struct MatchColorBombWithColorBombEvent : IEventSingleton
    {
        public IItem ColorBomb_1;
        public IItem ColorBomb_2;
    }
}