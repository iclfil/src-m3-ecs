using System.Collections.Generic;
using Client.Game.Utils;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using MatchThree.Rack.Common;
using MatchThree.Rack.ECS.Behaviors;
using MatchThree.Rack.ECS.Tags;
using MatchThree.Rack.Utils;
using SevenBoldPencil.EasyEvents;

namespace MatchThree.Rack.ECS.Systems
{
    public struct MatchColorBombWithStripedEvent : IEventSingleton
    {
        public IItem ColorBomb;
        public IItem StripedBomb;
    }

    public class MatchColorBombWithStriped : IEcsRunSystem, IEcsInitSystem
    {
        public EventsBus Bus { get; }
        public OnMatchColorBombWithStriped FinishCallback { get; }

        private readonly EcsFilterInject<Inc<ItemData, MatchBehavior, ChipTag>> _chips = "Board";

        private readonly IBoardFacade _board;
        private readonly OnMatchBonusCombo _matchBonusCombo;

        private EcsWorld _world;

        public MatchColorBombWithStriped(EventsBus bus, IBoardFacade board, OnMatchColorBombWithStriped finishCallback)
        {
            Bus = bus;
            FinishCallback = finishCallback;
            _board = board;
        }

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
        }

        public void Run(IEcsSystems systems)
        {
            //Берет цвет бомбы
            //находит все элементы с этим цветом
            //удаляет их и 

            if (Bus.HasEventSingleton<MatchColorBombWithStripedEvent>() == false)
                return;

            var data = Bus.GetEventBodySingleton<MatchColorBombWithStripedEvent>();
            var color = data.StripedBomb.Color;
            var items = GetColorsItems(color);

            var deleteItems = new List<IItem>();
            var stripedExplosions = new List<StripedExplosion>();

            foreach (var item in items)
            {
                if (_board.Has(item))
                {
                    _board.DeleteIncludeItem(item);
                    deleteItems.Add(item);
                }
            }

            foreach (var item in deleteItems)
            {
                var type = GetRandomVerticalOrHorizontal();
                var pos = item.Position;

                var matchedItems = new List<IItem>();

                if (type == TypeItems.VerticalBomb)
                    matchedItems = DeleteVerticalLine(pos);

                if (type == TypeItems.HorizontalBomb)
                    matchedItems = DeleteHorizontalLine(pos);

                stripedExplosions.Add(new StripedExplosion()
                {
                    Color = color,
                    Position = pos,
                    Type = type,
                    MatchedItems = matchedItems,
                });
            }

            _board.DeleteIncludeItem(data.ColorBomb);
            _board.DeleteIncludeItem(data.StripedBomb);


            Bus.DestroyEventSingleton<MatchColorBombWithStripedEvent>();

            FinishCallback?.Invoke(data.ColorBomb, data.StripedBomb, stripedExplosions, deleteItems);
        }

        private TypeItems GetRandomVerticalOrHorizontal()
        {
            var r = UnityEngine.Random.Range(0, 2);
            if (r == 0)
                return TypeItems.HorizontalBomb;
            else
                return TypeItems.VerticalBomb;
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

        private List<IItem> DeleteVerticalLine(Int2 pos)
        {
            var matchedItems = new List<IItem>();

            Int2 bonusPos = pos;

            for (int y = 0; y < _board.Height; y++)
            {
                var deletePos = new Int2(bonusPos.X, y);

                if (deletePos == bonusPos)
                    continue;

                if (_board.TryGetLastItem(deletePos, out var item) == false)
                    continue;

                if (item.Type != TypeItems.Chip)
                    continue;

                if (item.IsMatched)
                    continue;

                _board.DeleteIncludeItem(item);
                matchedItems.Add(item);
            }

            return matchedItems;
        }
        private List<IItem> DeleteHorizontalLine(Int2 pos)
        {
            Int2 bonusPos = pos;
            var matchedItems = new List<IItem>();

            for (int x = 0; x < _board.Width; x++)
            {
                var deletePos = new Int2(x, bonusPos.Y);

                if (deletePos == bonusPos)
                    continue;

                if (_board.TryGetLastItem(deletePos, out var item) == false)
                    continue;

                if (item.Type != TypeItems.Chip)
                    continue;

                if (item.IsMatched)
                    continue;

                _board.DeleteIncludeItem(item);
                matchedItems.Add(item);
            }

            return matchedItems;
        }
    }


}