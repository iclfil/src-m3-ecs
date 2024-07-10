using System.Collections.Generic;
using FilConsole;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using MatchThree.Rack.ECS.Behaviors;
using MatchThree.Rack.ECS.Tags;
using MatchThree.Rack.Utils;
using SevenBoldPencil.EasyEvents;

namespace MatchThree.Rack.ECS.Systems
{
    public class MatchColorBombWithChip : IEcsRunSystem, IEcsInitSystem
    {
        private readonly EcsFilterInject<Inc<ItemData, MatchBehavior, ChipTag>> _chips = "Board";
        private EcsWorld _world;
        private readonly EventsBus eventsBus;
        private readonly IBoardFacade _board;
        private readonly OnMatchColorBombWithChip _callback;

        public MatchColorBombWithChip(EventsBus eventsBus, IBoardFacade board, OnMatchColorBombWithChip callback)
        {
            this.eventsBus = eventsBus;
            _board = board;
            _callback = callback;
        }

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
        }

        public void Run(IEcsSystems systems)
        {
            if (eventsBus.HasEventSingleton<MatchColorBombWithChipEvent>() == false)
                return;

            var data = eventsBus.GetEventBodySingleton<MatchColorBombWithChipEvent>();

            var matchedItems = ColorBombHandler(data.ColorBomb, data.Chip);

            _callback?.Invoke(data.ColorBomb, data.Chip, matchedItems);

            eventsBus.DestroyEventSingleton<MatchColorBombWithChipEvent>();
        }

        private List<IItem> ColorBombHandler(IItem colorBomb, IItem secondItem)
        {
            IItem chip = null;
            IItem bonus = null;

            if (ItemsProperties.HasColor(colorBomb.Type))
            {
                chip = colorBomb;
                bonus = secondItem;
            }
            else
            {
                chip = secondItem;
                bonus = colorBomb;
            }

            var matchItems = new List<IItem>();
            var findedColor = chip.Color;
            var items = GetColorsItems(findedColor);

            foreach (var item in items)
            {
                matchItems.Add(item);
                _board.DeleteIncludeItem(item);
            }

            _board.DeleteIncludeItem(bonus);

            return matchItems;
        }

        private HashSet<IItem> GetColorsItems(int color)
        {
            var items = new HashSet<IItem>();
            foreach (var e_chip in _chips.Value)
            {
                if (_chips.Pools.Inc1.Get(e_chip).Item.Color == color)
                {
                    var item = _chips.Pools.Inc1.Get(e_chip).Item;
                    items.Add(item);
                }
            }

            return items;
        }
    }

    public struct MatchColorBombWithChipEvent : IEventSingleton
    {
        public IItem ColorBomb;
        public IItem Chip;
    }
}