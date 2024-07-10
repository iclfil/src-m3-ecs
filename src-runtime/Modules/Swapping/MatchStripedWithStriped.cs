using System.Collections.Generic;
using Client.Game.Utils;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using MatchThree.Rack.Common;
using SevenBoldPencil.EasyEvents;

namespace MatchThree.Rack.ECS.Systems
{
    public struct MatchStripedWithStripedEvent : IEventSingleton
    {
        public IItem Striped_1;
        public IItem Striped_2;
    }

    public class MatchStripedWithStriped : IEcsRunSystem
    {
        private readonly EventsBus eventsBus;
        private readonly IBoardFacade _board;
        private readonly OnMatchStripedWithStriped callback;
        private readonly EcsFilterInject<Inc<Swap>> _swap = default;

        public MatchStripedWithStriped(EventsBus eventsBus, IBoardFacade board, OnMatchStripedWithStriped callback)
        {
            this.eventsBus = eventsBus;
            _board = board;
            this.callback = callback;
        }

        public void Run(IEcsSystems systems)
        {
            if (eventsBus.HasEventSingleton<MatchStripedWithStripedEvent>() == false)
                return;

            var data = eventsBus.GetEventBodySingleton<MatchStripedWithStripedEvent>();

            var verticals = DeleteVerticalLine(data.Striped_1);
            var horizontals = DeleteHorizontalLine(data.Striped_2);
            _board.DeleteIncludeItem(data.Striped_1);
            _board.DeleteIncludeItem(data.Striped_2);

            var matchedItems = new List<IItem>();
            matchedItems.AddRange(verticals);
            matchedItems.AddRange(horizontals);

            callback?.Invoke(data.Striped_1, data.Striped_2, matchedItems);
            eventsBus.DestroyEventSingleton<MatchStripedWithStripedEvent>();
        }
        //TODO - вынести эту логику в некий сервис с интерфейсом, и в случае чего можно будет менять логику удаления через подмену реазизаций
        private List<IItem> DeleteVerticalLine(IItem bonus)
        {
            var matchedItems = new List<IItem>();

            Int2 bonusPos = bonus.Position;
            bonus.IsMatched = true;

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
        private List<IItem> DeleteHorizontalLine(IItem bonus)
        {
            Int2 bonusPos = bonus.Position;

            int color = bonus.Color;

            var matchedItems = new List<IItem>();

            bonus.IsMatched = true;

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