using System.Collections.Generic;
using Client.Game.Utils;
using FilConsole;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using MatchThree.Rack.CoreECS.Tags;

namespace MatchThree.Rack.ECS.Systems
{
    public class MatchLittleBomb : IEcsRunSystem, IEcsInitSystem
    {
        private readonly EcsFilterInject<Inc<LittleBomb, MatchesTag>> _bonusesMatches = default;

        private readonly IBoardFacade _board;
        private EcsWorld _world;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
        }
        public MatchLittleBomb(IBoardFacade board)
        {
            _board = board;
        }
        public void Run(IEcsSystems systems)
        {
            foreach (var _Enty in _bonusesMatches.Value)
            {
                var bonus = _bonusesMatches.Pools.Inc1.Get(_Enty).Bonus;
                var range = _bonusesMatches.Pools.Inc1.Get(_Enty).Range;
                Explosion(bonus, range);
                _world.DelEntity(_Enty);
            }
        }
        private void Explosion(IItem bomb, int range = 1)
        {
            bomb.IsMatched = true;
            var bonusPos = bomb.Position;
            var color = bomb.Color;

            var matchedItems = new List<IItem>();

            for (int x = bonusPos.X - range; x <= bonusPos.X + range; x++)
            {
                for (int y = bonusPos.Y - range; y <= bonusPos.Y + range; y++)
                {
                    var deletePos = new Int2(x, y);

                    if (deletePos == bonusPos)
                        continue;

                    if (_board.HasPosition(deletePos) == false)
                        continue;

                    if (_board.TryGetLastItem(deletePos, out var item))
                    {
                        if (item.IsMatched)
                            continue;

                        if (item.IsBonus == false)
                            item.IsMatched = true;

                        matchedItems.Add(item);
                    }
                }
            }

            MatchedBonus(bomb, matchedItems);
        }
        private void MatchedBonus(IItem bonus, List<IItem> matchedItems)
        {
            var entity = _world.NewEntity();
            var pool1 = _world.GetPool<LittleBomb>();
            ref LittleBomb data = ref pool1.Add(entity);
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
