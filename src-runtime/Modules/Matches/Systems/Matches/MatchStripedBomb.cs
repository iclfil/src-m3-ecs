using System.Collections.Generic;
using MatchThree.Rack;
using MatchThree.Rack.ECS;
using Client.Game.Utils;
using FilConsole;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using MatchThree.Rack.Common;
using MatchThree.Rack.CoreECS.Tags;
using MatchThree.Rack.Services;

namespace MatchThree.Rack.ECS.Systems
{
    public class MatchStripedBomb : IEcsRunSystem, IEcsInitSystem
    {
        private readonly EcsFilterInject<Inc<StripedBomb, MatchesTag>> _bonuses = default;
        private readonly EcsCustomInject<IMatchesService> _injectContext;

        private readonly IBoardFacade _board;
        private EcsWorld _world;
        private IMatchesService _matchesService;

        public MatchStripedBomb(IBoardFacade board)
        {
            _board = board;
        }

        public void Init(IEcsSystems systems)
        {
            _matchesService = _injectContext.Value;
            _world = systems.GetWorld();
        }

        public void Run(IEcsSystems systems)
        {
            OnceBonusHandler();
        }

        private void OnceBonusHandler()
        {
            foreach (var _Enty in _bonuses.Value)
            {
                var bonus = _bonuses.Pools.Inc1.Get(_Enty).Bonus;

                if (bonus.Type == TypeItems.VerticalBomb)
                {
                    var matches = _matchesService.DeleteVerticalLine(bonus, _board);
                    CreateMatchedBonus(bonus, matches);
                }

                if (bonus.Type == TypeItems.HorizontalBomb)
                {
                    var matches = _matchesService.DeleteHorizontalLine(bonus, _board);
                    CreateMatchedBonus(bonus, matches);
                }

                _world.DelEntity(_Enty);
            }
        }

        private void DeleteVerticalLine(IItem bonus)
        {
            Int2 bonusPos = bonus.Position;

            int color = bonus.Color;

            var matchedItems = new List<IItem>();

            bonus.IsMatched = true;

            for (int y = 0; y < _board.Height; y++)
            {
                var deletePos = new Int2(bonusPos.X, y);

                if (deletePos == bonusPos)
                    continue;

                if (_board.TryGetLastItem(deletePos, out var item))
                {
                    if(item.Type == TypeItems.DefaultSquare)
                        continue;

                    if (item.IsMatched)
                        continue;

                    if (item.IsBonus == false)
                        item.IsMatched = true;

                    matchedItems.Add(item);
                }
            }

        }
        private void DeleteHorizontalLine(IItem bonus)
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

                if (_board.TryGetLastItem(deletePos, out var item))
                {
                    if (item.Type == TypeItems.DefaultSquare)
                        continue;

                    if (item.IsMatched)
                        continue;

                    if (item.IsBonus == false)
                        item.IsMatched = true;

                    matchedItems.Add(item);
                }
            }

            CreateMatchedBonus(bonus, matchedItems);
        }

        //Создаем бонус, который сработал. Но список с его фишками, должен снова провериться на бонусы.
        private void CreateMatchedBonus(IItem bonus, List<IItem> matchedItems)
        {
            var entity = _world.NewEntity();
            var pool1 = _world.GetPool<StripedBomb>();
            ref StripedBomb data = ref pool1.Add(entity);
            data.Bonus = bonus;

            var pool2 = _world.GetPool<ECS.Items>();
            ref ECS.Items itemsData = ref pool2.Add(entity);
            itemsData.MatchedItems = new List<IItem>(matchedItems);

            var pool3 = _world.GetPool<MatchedTag>();
            pool3.Add(entity);

            var pool4 = _world.GetPool<CheckBonusesTag>();
            pool4.Add(entity);

            MyLog.Log("System/Green", $"Create Matches Bonus {bonus.Type} {bonus.Position} CountItems {matchedItems.Count} ", GetType());
        }
    }
}