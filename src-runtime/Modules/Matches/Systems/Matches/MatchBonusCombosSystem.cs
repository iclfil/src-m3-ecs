using System.Collections.Generic;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using MatchThree.Rack.Common;


namespace MatchThree.Rack.ECS.Systems
{
    public class MatchBonusCombosSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly EcsFilterInject<Inc<StripedBomb, ECS.Items, DestroyTag>> __stripeds = default;
        private readonly EcsFilterInject<Inc<LittleBomb, ECS.Items, DestroyTag>> __littleBombs = default;
        private readonly EcsFilterInject<Inc<ColorBomb, ECS.Items, DestroyTag>> __colorBombs = default;
        private readonly EcsFilterInject<Inc<FlyBomb, ECS.Items, DestroyTag>> __flyBombs = default;


        private readonly IBoardFacade _board;
        private readonly OnMatchBonus _matchBonus;
        private EcsWorld _world;

        public MatchBonusCombosSystem(IBoardFacade board, OnMatchBonus matchBonus)
        {
            _board = board;
            _matchBonus = matchBonus;
        }
        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var striped in __stripeds.Value)
            {
                var items = __stripeds.Pools.Inc2.Get(striped).MatchedItems;
                var bonus = __stripeds.Pools.Inc1.Get(striped).Bonus;

                ComboType type = ComboType.None;

                if (bonus.Type == TypeItems.VerticalBomb)
                    type = ComboType.VerticalBomb;

                if (bonus.Type == TypeItems.HorizontalBomb)
                    type = ComboType.HorizontalBomb;

                DestroyCombo(type, items, bonus);

                DeleteEnty(striped);
            }
            foreach (var color in __colorBombs.Value)
            {
                var items = __colorBombs.Pools.Inc2.Get(color).MatchedItems;
                var bonus = __colorBombs.Pools.Inc1.Get(color).Bonus;
                ComboType type = ComboType.ColorBomb;
                DestroyCombo(type, items, bonus);
                DeleteEnty(color);

            }
            foreach (var bomb in __littleBombs.Value)
            {
                var items = __littleBombs.Pools.Inc2.Get(bomb).MatchedItems;
                var bonus = __littleBombs.Pools.Inc1.Get(bomb).Bonus;
                ComboType type = ComboType.Bomb;
                DestroyCombo(type, items, bonus);
                DeleteEnty(bomb);
            }

            foreach (var bomb in __flyBombs.Value)
            {
                var items = __flyBombs.Pools.Inc2.Get(bomb).MatchedItems;
                var bonus = __flyBombs.Pools.Inc1.Get(bomb).Bonus;
                ComboType type = ComboType.FlyBomb;
                DestroyCombo(type, items, bonus);
                DeleteEnty(bomb);
            }
        }

        private void DestroyCombo(ComboType comboType, List<IItem> items, IItem bonus)
        {
            if (_board.Has(bonus) == false)
                return;

            items.Remove(bonus);

            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                if (_board.Has(item) == false)
                {
                    //корректируем список для отправки во View
                    items.Remove(item);
                    continue;
                }

                _board.DeleteIncludeItem(item);
            }

            _board.DeleteIncludeItem(bonus);
            _matchBonus?.Invoke(bonus, items);
        }

        private void DeleteEnty(int _enty)
        {
            _world.DelEntity(_enty);
        }
    }
}