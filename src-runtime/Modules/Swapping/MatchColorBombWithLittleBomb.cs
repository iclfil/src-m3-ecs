using System.Collections.Generic;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using MatchThree.Rack;
using MatchThree.Rack.Common;
using MatchThree.Rack.ECS.Behaviors;
using MatchThree.Rack.ECS.Tags;
using MatchThree.Rack.Services;
using MatchThree.Rack.Utils;
using SevenBoldPencil.EasyEvents;

namespace MatchThree.Rack.ECS.Systems
{
    public class MatchColorBombWithLittleBomb : IEcsRunSystem, IEcsInitSystem
    {
        private readonly EcsFilterInject<Inc<ItemData, MatchBehavior, ChipTag>> f_chips = "Board";
        private readonly EcsCustomInject<MatchesService> _customInject;
        private readonly EventsBus _bus;
        private readonly IBoardFacade _board;
        private readonly OnMatchColorBombWithLittleBomb _finishCallback;
        private readonly OnMatchBonusCombo _matchBonusCombo;

        private IMatchesService _matchesService;

        public MatchColorBombWithLittleBomb(EventsBus bus, IBoardFacade board, OnMatchColorBombWithLittleBomb finishCallback)
        {
            _bus = bus;
            _board = board;
            _finishCallback = finishCallback;
        }

        public void Init(IEcsSystems systems)
        {
            _matchesService = _customInject.Value;
        }

        public void Run(IEcsSystems systems)
        {
            if (_bus.HasEventSingleton<MatchColorBombWithLittleBombEvent>() == false)
                return;

            var data = _bus.GetEventBodySingleton<MatchColorBombWithLittleBombEvent>();
            var color = data.LittleBomb.Color;

            _board.DeleteIncludeItem(data.ColorBomb);
            _board.DeleteIncludeItem(data.LittleBomb);

            var colorsItems = GetColorsItems(color);
            var explosions = new List<StripedExplosion>();
            var deleteItems = new List<int>();

            foreach (var item in colorsItems)
            {
                var pos = item.Position;

                var matchedItems = _matchesService.RectExplosion(pos, 1, _board);

                foreach (var matchedItem in matchedItems)
                {
                    if (_board.Has(matchedItem))
                        _board.DeleteIncludeItem(matchedItem);

                }

                explosions.Add(new StripedExplosion()
                {
                    Color = color,
                    Position = pos,
                    Type = TypeItems.Bomb,
                    MatchedItems = matchedItems,
                });
            }

            foreach (var item in colorsItems)
            {
                if (_board.Has(item))
                    _board.DeleteIncludeItem(item);
                deleteItems.Add(item.GetID());
            }


            _finishCallback?.Invoke(data.ColorBomb.GetID(), data.LittleBomb.GetID(), explosions, deleteItems);
            _bus.DestroyEventSingleton<MatchColorBombWithLittleBombEvent>();
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
}

public struct MatchColorBombWithLittleBombEvent : IEventSingleton
{
    public IItem ColorBomb;
    public IItem LittleBomb;
}
