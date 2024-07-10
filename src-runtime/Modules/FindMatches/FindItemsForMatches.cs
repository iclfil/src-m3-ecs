using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Collections.Generic;
using FilConsole;
using MatchThree.Rack;
using MatchThree.Rack.ECS;
using MatchThree.Rack.ECS.Behaviors;

namespace MatchThree.ECS.Modules.FindMatches
{
    // Создает Данные у которых есть цвет и все нужные объекты.
    public class FindItemsForMatches : IEcsInitSystem, IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<ColoredItems>> __coloredItems = default;
        private readonly EcsFilterInject<Inc<ItemData, MatchBehavior>> _chips = "Board";

        private readonly EcsPoolInject<ColoredItems> _pool = default;

        private EcsWorld _world;

        private IBoardFacade _board;
        public FindItemsForMatches(IBoardFacade board)
        {
            _board = board;
        }

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
        }

        public void Run(IEcsSystems systems)
        {
            if (__coloredItems.Value.GetEntitiesCount() > 0)
                return;

            for (int i = 0; i < _board.Colors.Length; i++)
            {
                var color = _board.Colors[i];
                var items = GetMatchesItems(color);
                var entity = _world.NewEntity();
                ref var data = ref _pool.Value.Add(entity);
                data.Color = color;
                data.Items = items;
                //MyLog.Log("System", $"Colors Items on Board {items.Count}", GetType());
            }
        }

        private List<IItem> GetMatchesItems(int color)
        {
            var items = new List<IItem>();
            foreach (var e_chip in _chips.Value)
            {
                if (color == -1)
                {
                    var item = _chips.Pools.Inc1.Get(e_chip).Item;
                    items.Add(item);
                    continue;
                }

                if (_chips.Pools.Inc1.Get(e_chip).Item.Color == color)
                {
                    var item = _chips.Pools.Inc1.Get(e_chip).Item;
                    items.Add(item);
                }
            }

            return items;
        }
    }
}